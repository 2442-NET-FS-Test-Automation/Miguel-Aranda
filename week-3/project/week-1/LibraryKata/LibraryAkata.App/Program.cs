// If I had code from another namespace I want to use here - I use a using statement
using LibraryDomain;
using Serilog;

namespace LibraryKata.App; // A namespace is like a bucket or logical container for different
// related code files.
public class Program
{
    
    // Now we are moving away from the Python file style Top-Level statements
    // So we need a class to hold our Main() method. The previous style with no class
    // or main - implicity had a Main() under the hood. 

    // public - accessible across the program
    // static - Main can be called upon without a Program object. It is a Static/class method. 
    // void - it doesn't return anything
    public static async Task Main()
    {   
        // Lests configure Serilog here before any code execution
        // Serilog works via singleton object. Its share globally
        // throughout the app, configure once use anywhere
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information() // Verbose > Debug > Info > Warning > Error > Faltal
            .WriteTo.Console() // Sink: Where do my logs go? COnsole, text file, database, etc?
            .CreateLogger(); // create the logger based on the config above


        // When I call dotnet run, it finds Main() and begins code execution at the first line of the 
        // main method. I wrote my code, inside DataTypesAndOperators() - a separate method. So if I want 
        // that code to run, I need to call it inside Main()
        Program.DataTypesAndOperators();
        ClassesExample();
        OopDemo();
        CollectionsDemo();

        // In case there are any lingering logs by the time we hit line 41 above
        // Don't just stop execution, write the logs to their sink THEN close the program
        Log.CloseAndFlush();
        ExceptionsDemo();
        AdvancedClassesDemo();
        await AsyncHttpDemo();
    }

    // private - accessible only within this class
    // static - it belongs to the class, not objects of the class
    // void - returns nothing
    private static void DataTypesAndOperators() // If I had arguments, or inputs for this method,
    { // they would go inside the parenthesis after the method name 
        Console.WriteLine("=== Data types and operators ==");

        // C# is a Strongly typed language
        // We cannot just create variables and shove whatever we want into them like JS or Python
        int copies = 3; // whole numbers
        double lateFee = 1; // floating point numbers (decimals)
        bool isMember = true; // true/false values
        char shelf = 'A'; // single character
        string title = "Clean Code"; // text, strings are reference types

        // Operators 
        string user = "Jon"; // Single = is the assignment operator. 
        int total = copies * 2; // example of an arithmetic operator, like + - * / 
        bool isEnough = total > 4; // comparison - This line compares the value in total to 4, if it is greater
        // than 4, isEnough will get 'true', otherwise it will get 'false'
        // >, <, >=, <= - comparison operators
        bool exactlySix = total == 6; // equality. Single equals is assignment, double equals is equality.
        // unlike JS there is NO === all equality in C# is Strict equality
        bool lendable = isMember && isEnough; //logical operators
        // && - and, || - or, ! - reverses the condition that follows, ^ logical XOR - returns true if ONLY one condition is true

        // This is the basic way to construct strings from other strings
        // String concat - it works! But it can be messy
        Console.WriteLine(title + " has been checked out by " + user);

        // We can create much cleaner formatted strings
        // using String Interpolation - a string with a $ before the opening quote
        Console.WriteLine($"{title} on shelf {shelf}: {copies} copies, fee {lateFee}"); 

        // C# has ALOT of shorthands and little shortcuts that you can find and use 
        // to make your code easier to write. For example, lets say I want to add 1 to the value of total
        // I could do something like
        // total = total + 1; - ORRR
        total += 1; // arithmetic shorthand for the same thing, also works for *= /= -=

    }

    private static void ControlFlow()
    {
        Console.WriteLine("\n== Control Flow ==");

        // if - else if - else
        int copiesAvailable = 0;
        bool isMember = true;
        if(copiesAvailable > 1 )
        {
            Console.WriteLine("Many available for checkout!");
        }
        else if (copiesAvailable == 1)
        {
            Console.WriteLine("Last copy!");
        } else
        {
            Console.WriteLine("Out of stock!");    
            Console.WriteLine("Check again later!");
        }

        // switch
        string genre = "Mystery";
        switch (genre)
        {
            case "Mystery":
                Console.WriteLine("Check section A!");
                break;
            case "Science":
                Console.WriteLine("Check section F!");
                break;
            default: // while optional, a default case to catch any edge cases is best practices
                Console.WriteLine("Uh oh");
                break;
        }

        // NEW in .NET 8, Switch expressions! You don't have to use these - they probably wont come up in QC
        // but they're used out in real world code, so here is an example. In a switch expression, we want
        // a return value from the switch - we can then use that value to print out a result

        string section = genre switch
        {
            // This is my expression body
            "Mystery" => "Section A",
            "Science" => "Section F",
            _ => "uh oh"
        };
        Console.WriteLine(section);
        
        }        
        private static void Loops()
        {
        // C# provides for loops as well, 
        // For, while, do-while, etc
        for(int day = 1; day <= 3; day++)
        {
            Console.WriteLine($"Reminder day{day}: fee so far{calculateFee(day)}");
        }

        int onShelf = 3;
        while(onShelf > 0)
        {
            Console.WriteLine($"{onShelf} copies on the shelf!");
            onShelf--; // quick decrement shorthand
        }
        Console.WriteLine("No copies on shelf!");

        string myString = "dog";

        myString = "cat";
    }

    // I can use this for one line methods
    private static decimal calculateFee(int daysLate) => daysLate *2;


    private static void ArraysWork()
    {
        // C# provides for arrays as well as lists and other collections - we'll get to those later
        string[] books = {"Dune","Happy Potter","Percy Jackson","Lord of the Rings"};
        
        Console.WriteLine(books[2]); // I can access individual elements - keeping in mind we index at 0
        
        // C# allows for for-each loops
        foreach (string book in books)
        {
            Console.WriteLine(book);
        }
    }
    private static void ClassesExample()
    {
        Console.WriteLine("using our Domain Book class");

        // instantiating my first book, calling the constructor via "new" keyword
        OldBook dune = new OldBook("Dune","Frank Herbert",3);
        OldBook littlePrince = new OldBook("The Little Prince","Antoine",0);

        // If I want to print book info, I can just pass the book variable
        // It calls the toString() for me.
        Console.WriteLine(dune);
        Console.WriteLine(littlePrince.ToString());

        Console.WriteLine($"Checking out Dune: {dune.checkOut()}"); // true
        Console.WriteLine($"Checking out The Little Prince: {littlePrince.checkOut()}"); // false
    }

    public static void OopDemo()
    {
        Console.WriteLine("\n \n == OOP Demo stuff ==");

        LibraryItem[] catalog =
        {
          new Book("Dune", "Frank Herbert", 2),
          new ReferenceBook("C# Language Standards", "Microsoft", "Technology"),
          new Magazine("Sports Illustrated", "Francisco", 5, "Conde Naste")
        };

        foreach(LibraryItem item in catalog)
        {
            Console.WriteLine(item.Describe());
        }

        // We can even use interfaces as reference types
        foreach(LibraryItem item in catalog)
        {
            if (item is ILendable lendable)
            {
                Console.WriteLine($"{item.Title}: checkout -> {lendable.CheckOut()}");
            } else
            {
                Console.WriteLine($"{item.Title} Reference only.");
            }
        }

        // override vs new behaviour
        Magazine wired = new Magazine("Wired", "Luis", 3, "Conde Nast");
        LibraryItem baseMag = wired;

        Console.WriteLine("== Override vs new on the same object, different ref type");
        Console.WriteLine($"Magazine reference -> {wired.Describe()}");
        Console.WriteLine($"LibraryItem reference -> {baseMag.Describe()}");
    }

    // Collections demo stuff
    private static void CollectionsDemo()
    {
        Console.WriteLine("==== COLLECTIONS DEMO STUFF ====");
        // creating a catalog object
        // because this is backed by a list, it grows and shrinks for us
        Catalog catalog = new();

        // I could create my objects
        Book dune = new Book("Dune","Frank Herbert",3);

        // then add them
        catalog._items.Add(dune);

        // I can also just call a constructor inside the Add() method call
        // Methods having their arguments satisfied by the ruturn of other methods is a common pattern
        // and sometimes you'll get like 4-5 callbacks deep in the tools like ASP.net
        catalog._items.Add(new ReferenceBook("C# Language Specs", "MIcrosoft", "Technology"));
        catalog._items.Add(new Magazine("Nat Geo", "Charlie", 4, "Conde Naste"));

        Console.WriteLine($"Catalog holds {catalog._items.Count}; first is {catalog._items[0].Title}");

        // Enum + Struct use
        ItemKind kind = ItemKind.Magazine; // example of selecting an enum value
        ShelfLocation location = new ShelfLocation(3,12); // struct - looks a lot like a class, but it is a VALUE type
        Console.WriteLine($"{kind} sits at {location}");

        Book duneCopy = dune; // copies the reference
        // lets say I modify duneCopy, what happens to th data in dune?
        // all we copied was the pointer - these two things are not independent

        ShelfLocation location2 = location; // copies the data/fields
        // these are not linked in the same way, I can edit the data in one without touching the other
        
        // Generics: our own Shelf<T that can hold anything - though technically all the collections
        // we used thusfar have been generic classes themselves 
        Shelf<LibraryItem> shelf = new Shelf<LibraryItem>(10);
        Shelf<int> intShelf = new Shelf<int>(200);

        shelf.TryAdd(catalog._items[0]);
        shelf.TryAdd(catalog._items[1]);

        Console.WriteLine($"Trying to add a third thing in our catalog: {shelf.TryAdd(catalog._items[2])}");

    }
    public static void ExceptionsDemo()
    {
        Console.WriteLine("\n == Exceptions, patterns, logging ==");
        
        // By using Liskov Substitution from SOLID, if I later swap to
        // a SQLibraryRepo or whatever, this is the only line I have to change
        ILibraryRepository repo = new InMemoryLibraryRepository();

        // Injecting our existing repo object to stadisfy LibraryUnitOfWork's dependency
        IUnitOfWork libraryWork = new LibraryUnitOfWork(repo);

        // Create a book, but using our factory 
        LibraryItem dune = LibraryItemFactory.Create(ItemKind.Book, "Dune", "Frank Herbert", copies: 3);

        repo.Add(dune);

        repo.Add(LibraryItemFactory.Create(ItemKind.Magazine, "Wired", "Axel", copies: 2));

        // Pretend we're comitting changes to a DB or something
        libraryWork.Stage("added 2 items");
        libraryWork.Commit();

        // We went though the trouble of creating custom exceptions
        // Lets actually see them work for us. If you have code taht can potentially fail
        // wrap it in a try-catch (optional finally)
        try
        {
            // Potentially offending code goes here 
            LibraryItem missing = repo.GetById(99);
            Console.WriteLine(missing.Describe()); // we won't hit this I believe
        }
        catch (ItemNotFoundException ex)
        {
            // Your code can potentially throw more than one error
            // last to least
            // We stored the offending id on the exception itself, here we can ask for it for logging
            Log.Error("Lookup failed for id {Id}: {Message}", ex.Id, ex.Message);
        }
        catch (LibraryException ex)
        {
            Log.Error("Library error {Message}", ex.Message);
        }
        catch (Exception ex)
        {
            Log.Error("Non libray error: {Message}", ex.Message);
        }
        finally // Optional, but addidg a fianlly block adds code that runs
        { // whether an exception is catch or not
            Console.WriteLine("hit out fianlly block - lookup attempt done");
        }

        Book noCopies = new Book("Count of Montecristo","John", 0);

        try
        {
            Borrow(noCopies);
        }
        catch(ItemNotAvailableException ex)
        {
            Log.Warning("Borrow refused: {Message}", ex.Message);
        }
    }

    public static void Borrow(Book book)
    {
            if (!book.CheckOut())
            {
                throw new ItemNotAvailableException(book.Title);
            }
     }

     public static void AdvancedClassesDemo()
    {
        Console.WriteLine("\n == Advanced classes ==");
        // First, a quick detour, lets interact with the GC
        Console.WriteLine(GC.GetTotalMemory(forceFullCollection: false) / 1024);

        ILibraryRepository repo = new InMemoryLibraryRepository();

        // Create a book, but using our factory 
        LibraryItem dune = LibraryItemFactory.Create(ItemKind.Book, "Dune", "Frank Herbert", copies: 3);

        repo.Add(dune);

        repo.Add(LibraryItemFactory.Create(ItemKind.Magazine, "Wired", "Axel", copies: 2));
        repo.Add(LibraryItemFactory.Create(ItemKind.Book, "Dune Messiah", "Frank Herbert", copies: 3));
        repo.Add(LibraryItemFactory.Create(ItemKind.ReferenceBook, "C# Language Reference", "Microsoft", 1, section: "Technology"));

        Catalog catalog = new();

        foreach (LibraryItem item in repo.GetAll())
        {
            catalog.Add(item);
        }
        Console.WriteLine($"We have {catalog.Authors.Count} unique authors in our catalog");
        foreach(string author in catalog.Authors)
        {
            Console.WriteLine(author);
        }
        // Lets search our catalog now that it's locked by a dictionary
        // Lets use our find() method
        List<LibraryItem> byFrankHerbert = catalog.Find(item => item.Author == "Frank Herbert");
        Console.WriteLine($"There are {byFrankHerbert.Count} books by Frank Herbert");

        // Lets see how many items in the catalog are lendable
        Console.WriteLine("We have a mix of lendable and non-lendable items");

        foreach(LibraryItem item in catalog.Lendable())
        {
            Console.WriteLine($"{item.Title}");
        }
    }

     public static async Task AsyncHttpDemo()
    {
        // We wrote our client object lets use it
        OpenLibraryClient client = new();

        // Array to hold some isbns
        string[] isbns = { "9780132350884", "9780201633610"};

        // I want to fetch data from OpenLibrary for BOTH isbns
        Task<LibraryItem?>[] fetchedBooks = new Task<LibraryItem?>[isbns.Length];

        // Next we loop throught the array and call FetchByIdAsync

        for(int i=0; i<isbns.Length; i++)
        {
            fetchedBooks[i] = client.FetchByIsbnAsync(isbns[i]);
        }

        LibraryItem?[] foundBooks = await Task.WhenAll(fetchedBooks);

        LibraryItem? firstBookFound = foundBooks.Length > 0 ? foundBooks[0] : null;


        Console.WriteLine($"Fetched: {firstBookFound?.Describe() ?? "nothing"}");

        // Boxing and unboxing - mostly depricated , replaced by Generics
        // Sometimes we needed to store value types on the heap, think of adding an int to a list. Before generics (List<T>)
        // we had an arraylist to accomplish the same thing. 

        int toBeBoxed = 6;
        // We "box" it, by giving wrapping it in an object reference

        object boxed = toBeBoxed;

        int unboxed =(int)boxed;
    }
}


