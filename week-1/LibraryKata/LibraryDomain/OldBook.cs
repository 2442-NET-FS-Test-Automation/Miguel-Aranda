namespace LibraryDomain;

public class OldBook
{
    // Things about a book we can model - what is the "shape" of book
    // Because i want to use a no-arg constructor, its best practice to make
    // my properties nullable.
    public string? Title{get; private set;}
    public string? Author{get; private set;}
    public int? CopiesAvailable {get; private set;}

    // The same way ew can have static methods (belong to the class)
    // we can have static properties/members
    private static int _nextId = 1; // By convention, static properties have an underscore

    private int Id {get;} // no setter, I don't want someone to reasign this.

    // Every class has a very very specific method within it
    // the constructor - you can have as many as you need/want
    public OldBook(string title, string author, int copiesAvailable)
    {
        Id = _nextId++; // get the value of _nextId, assing it, increment it
        Title = title;
        Author = author;
        CopiesAvailable = copiesAvailable;
    }

    // Our first instance method - no "static" keyword, just
    // a access modifier + return type
    public bool checkOut()
    {
        // Attempt to checkout a book - if copies is already 0, return false
        if(CopiesAvailable == 0)
            return false;

            // Otherwise, we pass over the above code block
            // We can decrement the available copies and return true
            CopiesAvailable--;
            return true;
    }

    // Providing for return behaviour
    public void Return() => CopiesAvailable++;

    // Overriding a toString
    public override string ToString()
    {
        // Commented out bleow is a call to base.ToString()
        // We can use the baes keyword to refer to the parent class of the class we are working in
        // Book's parent is object, so this is calling the default ToString()
        //return base.ToString();

        return $"{Title} by {Author}: {CopiesAvailable} available for checkout";
    }
}
