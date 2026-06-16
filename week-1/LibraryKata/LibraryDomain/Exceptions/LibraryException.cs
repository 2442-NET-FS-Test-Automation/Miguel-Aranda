namespace LibraryDomain;

// An Exception is any class that inherits from the base Exception class
public class LibraryException : Exception
{
    // The base class just contains a message
    public LibraryException(string message) : base(message){ }  
}