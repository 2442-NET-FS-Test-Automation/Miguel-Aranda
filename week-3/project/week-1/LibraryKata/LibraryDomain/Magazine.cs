namespace LibraryDomain;

// Selead is pretty simple, it means this class is not inheritable
// Nobody can be a child of Magazine
public sealed class Magazine: LibraryItem, ILendable
{
    public int CirculationCopies {get; private set;}
    public string Publisher{get; private set;}

    public Magazine(string title, string author, int circulationCopies, string publisher)
    :base(title, author)
    {
        CirculationCopies = circulationCopies;
        Publisher = publisher;
    }

    public override string Describe()
    {
        return $"{Title} magazine, published by {Author}";
    }

    // Providing implementation via new instead of override - has implementations for later
    // This is technically Method Hiding - depends on the reference type
    // Calling this method in an object instantiated like this:
    // LibraryItem sportsIlustrated = new Magazine(...) - calls LibraryItems's ShelfLabel
    // new vs override - very different behaviour
    public new string ShelfLabel()
    {
        return $"MAG-{Id} {Title}";
    }

        public bool CheckOut()
    {
        // Attempt to checkout a book - if copies is already 0, return false
        if(CirculationCopies == 0)
            return false;

            // Otherwise, we pass over the above code block
            // We can decrement the available copies and return true
            CirculationCopies--;
            return true;
    }

    // Providing for return behaviour
    public void Return() => CirculationCopies++;
}