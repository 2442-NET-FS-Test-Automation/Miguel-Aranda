using Library.Data.Entities;
namespace LibraryData;
public interface IInventoryRepository
{
    Task<IReadOnlyList<InventoryItem>> GetAllASync();
    Task<InventoryItem?> GetInventoryItemSkuAsync(string sku);
    Task<InventoryItem> AddInventoryItemAsync(string sku, string name, decimal price, int quantity);
    Task<bool> RemoveBySkuAsync(string sku);
}