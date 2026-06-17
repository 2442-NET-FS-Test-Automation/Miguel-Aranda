using Serilog;
namespace LibraryDomain;

public class LibraryUnitOfWork : IUnitOfWork
{
    // This property is mandatory because its in my interface
    public ILibraryRepository Items {get;}
    // I want something to hold my list of staged changes
    // We will represent those as strings, this is a shallow demo example
    public readonly List<string> _staged = new();
    // We need a constructor
    // We are tecnically using Dependency injection here. We never instantiate the
    // ILibraryRepository object, we ask for an existing one.
    public LibraryUnitOfWork(ILibraryRepository items)
    {
        Items = items;
    }
    public int Commit()
    {
        // shallow commit implementation
        // we will just log how many things were staged + commited
        int count = _staged.Count;

        Log.Information("LibraryUnitOfWork committed {count} staged changes(s)", count);
        // Once you're done doing whatever work you needed to, clear the staging area
        // some logic as gift
        _staged.Clear();

        return count;
    }

    public void Stage(string change)
    {
        _staged.Add(change);
    }
}