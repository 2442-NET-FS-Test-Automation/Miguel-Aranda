using System.Text.Json; //Library for working JSON - written by microsoft
using Serilog;

namespace LibraryDomain;

public class OpenLibraryClient{
    // We are going to create an use one HTTPClient for the entire proccess
    // If you use one per call, you are going to leak sockets - eventually trigger a socketException
    private static readonly HttpClient client = new();

    // We aer going to write an async method. An async method is ANY method that calls async code
    // So, if you use something like .FindAsync() OR "await" method a method within a method body
    // the surronding method MUST be declared as async

    // A Task in C# is like a promise in JS - it is a placeholder in memory telling the runtime
    // I expect there to be libraryItem (or whatever Task is 'wrapping' with it's brackets)
    // - when this method resolves. I have no idea what that is, so for now - hold that place with a Task
    // we are also going account for the possibility of a null - because my HTTP call could fail for a number of reasons
    // I could be rate limited  
    public async Task<LibraryItem> FetchByIsbnAsync(string isbn)
    {
        // string to hold the url i'm targeting
        string url = $"https://openlibrary.org/search.json?q=isbn:{isbn}&fields=title,author_name&limit=1";

        // we are going to try to get a json formatted string from a api
        try
        {
            // Whenever we call upon an async method , we must await the call
            string jsonResponse = await client.GetStringAsync(url);
            
            // We are going to write our own parsing Logic in a method called Parse()
            return Parse(jsonResponse);

        } catch (HttpRequestException ex)
        {
            Log.Warning("Network fetch failed for {isbn}: {Message}", isbn, ex.Message);
            return null;
        } catch (Exception ex)
        {
            Log.Warning("FetchByIsbnAsync failed {Message}", ex.Message);
            return null;
        }
    }

    // We are going to write our own parsing Logic in a method called Parse()

    public static LibraryItem? Parse(string json)
    {
        // the search API within OpenLibrary returns a JSON object, and inside that object, among other fields.
        
        Dictionary<string, JsonElement>? resp = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
        if(resp is null || !resp.TryGetValue("doc", out JsonElement docs) || docs.GetArrayLength() == 0)
        {
            return null;    
        }

        JsonElement foundBook = docs[0]; // If we get something back, we should only get one thing. We search by isbn - they're unique

        // now we unpack things about this foundbook

        // we are using the ?? null colleccing operator
        // If something is there: return the value resulting in the left of the ?? operator
        // ?? returns a default value here
        string title = foundBook.GetProperty("title").GetString() ?? "Untitled";

        string author = "Unknown";

        // Checking to see if we have the author array an if its there grab the first author
        if(foundBook.TryGetProperty("author_name", out JsonElement authors) && authors.GetArrayLength() > 0)
        {
            author = authors[0].GetString() ?? "Unknown";
        }
        return LibraryItemFactory.Create(ItemKind.Book, title, author);
    }

}