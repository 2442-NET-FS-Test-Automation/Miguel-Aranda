using InstrumentsStore;

public class Client
{
    public string? Name {get; set;}
    public decimal Balance{get; set;}
    public List<InstrumentItem> MyInstrument {get; set;} = new List<InstrumentItem>();

    public Client(string name, decimal balance)
    {
        Name = name;
        Balance = balance;
    }

    public decimal CheckBalance(){
        return Balance;
    }
}