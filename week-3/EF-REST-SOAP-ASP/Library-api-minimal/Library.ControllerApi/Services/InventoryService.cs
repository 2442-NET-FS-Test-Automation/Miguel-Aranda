using Library.ControllerApi.DTOs;
using Library.Data.Entities;
using LibraryData;

namespace Library.ControllerApi.Services;
public class InventoryService : IInventoryService
{
    // Our InventoryService is what will call repo layer methods, so it
    // gets that dependency. Not the controller layer.
    private readonly IInventoryRepository _repo;
    public InventoryService(IInventoryRepository repo)
    {
        _repo = repo;
    }

    public Task<IReadOnlyList<InventoryItem>> AllAsync()
    {
        return _repo.GetAllASync();
    }

    public Task<InventoryItem?> BySkuAsync(string sku)
    {
        return _repo.GetInventoryItemSkuAsync(sku);
    }

    public Task<InventoryItem> AddAsync(InventoryCreateDto dto)
    {
        // this is going to need a DTO - we'll return to this
        return _repo.AddInventoryItemAsync(dto.Sku, dto.Name, dto.Price, dto.CurrentStock);
    }

    public Task<bool> RemoveAsync(string sku)
    {
        return _repo.RemoveBySkuAsync(sku);
    }
}