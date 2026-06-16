namespace LibraryDomain;

public class ItemNotAvailableException : LibraryException{
    public ItemNotAvailableException(string title) 
        : base($"{title} has no copies to available to borrow"){}
}