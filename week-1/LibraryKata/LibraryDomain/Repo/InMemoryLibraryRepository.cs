// This class will be our actual library Catalog store of info
using Serilog; // bringing in that outside code we downloaded
namespace LibraryDomain;

public class InMemoryLibraryRepository : ILibraryRepository
{
    // Because we don't have outside store of info (Like a SQL database)
    // we are kind of forced to rely on a list. We will store info outside
    // of program execution - I promise
    private readonly List<LibraryItem> _items = new();
    public void Add(LibraryItem item)
    {
        _items.Add(item);
        // We just added 
        Log.Information("Added {Title} - id: {Id}", item.Title, item.Id);
    }

    public List<LibraryItem> GetAll()
    {
        // Don't want to accidentally pass a pointer to my real list
        // return of the list
        return _items.ToList();
    }

    public LibraryItem GetById(int id)
    {
        // In order to find...
        foreach (LibraryItem item in _items)
        {
            // loop through the list, check for an item with the given Id
            // If we don't find it, throw an exception
            if(item.Id == id)
            {
                return item;
            }
        }

        // If we make it here - we exited the foreach without finding an item for that id
        Log.Warning("Lookup failed for id {Id}", id);
        throw new ItemNotFoundException(id); // throwing our custom exception, with offending id

    }

    public bool Remove(int id)
    {
        foreach (LibraryItem item in _items)
        {
            if(item.Id == id)
            {
                _items.Remove(item);
                Log.Information("Removed item with id {Id}", id); // log the removal
                return true;
            }
        }
        Log.Information("Removal failed for item with id {id}", id);
        return false;
    }
}