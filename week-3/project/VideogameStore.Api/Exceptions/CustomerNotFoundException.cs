public class CustomerNotFoundException : Exception
{
    public string Email { get; }

    public CustomerNotFoundException(string email) 
        : base($"No customer found with email '{email}'.")
    {
        Email = email;
    }
}