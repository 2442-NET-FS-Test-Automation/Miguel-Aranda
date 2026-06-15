namespace LibraryDomain;

public class Catalog
{
    // Backing our catalog is going to be a list.
    // List<T>: order, grow/shrink dynamically, accessable via index.
    // Your default collection - even above Arrays
    public List<LibraryItem> _items = [];

    // This method is technically redundant - this class basically just wraps the above list
    // BUT if we wanted to restrict people from Adding or Removing or even accessing via index
    // from other places in the code, we could wrap not only the list, but its instance methods
    // with our own wrapper methods and make them internal, private, protected, etc.
    public int Count => _items.Count;

    // Stack<T>: Last in first out - We will model the return cart. The most recently returned item
    // is re-shelved fist
    // Primary methods: Push(): puts an item at the top of the stack. Pop() - removes the top most item.
    public readonly Stack<LibraryItem> _returnCart = new();

    // Queue<T>: First in first out - modeling a hold queue, customers placing holds on books
    // Primary methods - Enqueue(): Join the back of the line, Dequeue(): removed from the front of the line.
    public readonly Queue<string> _holdQueue = new();

    // Reading list
    // LinkedList<T>: cheap inserts/removals anywhere in my list, but NO index access.
    public readonly LinkedList<LibraryItem> _readingList = new();
}