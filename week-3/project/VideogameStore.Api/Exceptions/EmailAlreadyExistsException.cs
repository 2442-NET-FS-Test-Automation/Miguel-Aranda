namespace VideogameStore.Api.Fullfill;

public sealed class EmailAlreadyExistsException : Exception
{
    public string Email {get;}
    public EmailAlreadyExistsException(string email) : base($"The [{email}] already exists!")
    {
        Email = email;
    }
}