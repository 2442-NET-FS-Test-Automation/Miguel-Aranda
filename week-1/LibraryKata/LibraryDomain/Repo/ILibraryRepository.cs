namespace LibraryDomain;
public interface ILibraryRepository
{
    // This is an abstraction over an actual repository class (concrete implementation)
    // Lets Think of things we want to be able to do against our Library's store of information

    // At minimum we probably want to provide for basic CRUD

    // Create new items in my library
    void Add(LibraryItem item);

    // Read/get library items
    LibraryItem GetById(int id); // throws ItemNotFoundException if the item doesn't exist at all
    List<LibraryItem> GetAll();

    // Update library items

    // Delete items in my library
    bool Remove(int id); // takes an item id of item to delete.
}