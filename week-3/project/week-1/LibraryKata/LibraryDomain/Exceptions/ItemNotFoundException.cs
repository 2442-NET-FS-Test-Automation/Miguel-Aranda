namespace LibraryDomain;

public class ItemNotFoundException : LibraryException
{
    // We can hold the offending Id that triggered the exception
    // We will use this for logging later
    public int Id {get;}
    public ItemNotFoundException(int id) 
        : base($"No library item with Id {id}")
        {
            Id = id;
        }
}