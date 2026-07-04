namespace DsaThreading;

public class Bank
{
    public long Balance; // mutable state
    // private lets create a lock object
    private readonly object _gate = new();
    public void DepositUnSafe(long amount) => Balance += amount; // read-modify-write: NOT ATOMIC
    public void DepositSage(long amount)
    {
        lock (_gate) // only one thread can enter this code block at a time
        {
            Balance += amount;
        } 
    }
}