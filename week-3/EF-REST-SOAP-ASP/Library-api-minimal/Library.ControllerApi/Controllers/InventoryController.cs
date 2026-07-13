using AutoMapper;
using Library.ControllerApi.DTOs;
using Library.ControllerApi.Services;
using Library.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

[ApiController]
[Route("api/[controller]")] // pretty sure this will be localhos:5051/api/Inventory as the route base
public class InventoryController : ControllerBase
{
    // this will get deleted
    private readonly IInventoryService _service; 
    private readonly IMapper _mapper; // auto mapper object

    // Server side cache
    // One shared instance for the whole app - singleton
    private readonly IMemoryCache _cache; // server side cache
    private readonly ISupplierClient _supplier;

    public InventoryController(IInventoryService service, IMapper mapper, IMemoryCache cache, ISupplierClient supplier)
    {
        _service = service;
        _mapper = mapper;
        _cache = cache;
        _supplier = supplier;
    }

    // lets write our first GET endpoint
    [HttpGet] // IActionresult just represents possible HTTP response actions
    [ResponseCache(Duration = 30)] // adding response cache-ing, now that we've set it up in Program.cs
    public async Task<ActionResult<IEnumerable<InventoryDto>>> Get()
    { 
        // Lets add server side cache-ing - still straightforward but we have to think a little harder
        // We have to think about when/where to add the logic to add something to the cache - and also
        // when to invalidate it.

        // First - check the cache. If its there and valid, pull from it. Otherwise
        // we will add whatever we get during this method to the cache
        var dtos = await _cache.GetOrCreateAsync("inventory:all", async entry =>
        {
            // Setting things about our cache entry - like "expire no matter what after 2 minutes"
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2);

            // Actually get the items from DB
            var dtos = await _service.AllAsync();

            // Return to the front end (and also add to cache, since we're wrapped by _cache.GetOrCreateAsync)
            return _mapper.Map<List<InventoryDto>>(dtos);
        });

        return Ok(dtos);
        
        // var items = await _service.AllAsync();

        // var mappedItems = _mapper.Map<List<InventoryDto>>(items);

        // return Ok(mappedItems);

        // As is this creates an infinite loop when we try to serialize to JSON
        // return Ok(await _repo.GetAllASync());

        // The fix is using a DTO - Data Transfer Object. In general it is bad practice
        // to send models as returns (or take them as arguments) to/from controller methods
        // Models are for your API, not for the 
        // var items = await _repo.GetAllASync(); // get all items

        // // this is what we need to send back once we populate it
        // EntireInventoryDTO response = new();

        // // Now we need to map to those DTOs - 
        // foreach(var item in items)
        // {
        //     InventoryReturnDTO i = new InventoryReturnDTO
        //     {
        //         name = item.Product.Name,
        //         Sku = item.Product.Sku,
        //         currentStock = item.CurrentStock
        //     };

        //     response.EntireInventory.Add(i);
        // }
        // return Ok(response);
    }

    // localhost: 5137/api/Inventory/{sku} - sku is passed in by the user
    // We can add routing info right on the annotation
    [HttpGet("{sku}")] // I can parameterize the route itself
    public async Task<ActionResult<InventoryReturnDTO>> GetBySku(string sku)
    {
        var item = await _service.BySkuAsync(sku);
        if (item is null)
        {
            return NotFound();
        } else
        {
            var mappedItem = _mapper.Map<InventoryDto>(item);
            return Ok(mappedItem);
        }
        // var item = await _repo.GetInventoryItemSkuAsync(sku);

        // if(item is null) return NotFound(); // returns a 404 - Sku didnt exist in db

        // var response = new InventoryReturnDTO
        // {
        //         name = item.Product.Name,
        //         Sku = item.Product.Sku,
        //         currentStock = item.CurrentStock
        // };

        // // Then we check what to return based on item being null or not
       
        // return Ok(response); // 200 - found something 


    }
    [HttpPost]
    public async Task<ActionResult<InventoryDto>> Create(InventoryCreateDto newInv)
    {
        var created = await _service.AddAsync(newInv);
        var response = _mapper.Map<InventoryDto>(created);

        // CreatedAt (201) works a little differently from our ther response ActionResults
        // Created at needs to know how to find the newly created resource - so we tell it
        // Use the GetBySku controller method (literally the one above) and use the information
        // In response to build the URI string

        // Invalidating whatever is in cache - because DB state has changed
        _cache.Remove("inventory:all"); // remove all

        return CreatedAtAction(nameof(GetBySku), new {sku = response.Sku}, response);
    }

    [HttpDelete("{sku}")]
    public async Task<ActionResult> Delete(string sku)
    {
        bool isDeleted = await _service.RemoveAsync(sku);

        if (isDeleted)
        {
            _cache.Remove("inventory:all");
            return NoContent(); // 204 - No Content - it WAS here not anymore
        }

        return NotFound();
    }

    // New GET that uses that SupplierClient to call an outsite API
    // localhost:5173/api/Inventory/{sku}/supplier-price
    [HttpGet("{sku}/supplier-price")]
    public async Task<IActionResult> GetSupplierPrice(string sku)
    {
        // Call our supplier with the httpclient code
        var price = await _supplier.GetListPriceAsync(sku);

        if (price is null)
        {
            return NotFound(); // No price found
        }

        // Returning an inline object for now, no DTO
        return Ok(new {sku, supplierPrice = price});
    }
}