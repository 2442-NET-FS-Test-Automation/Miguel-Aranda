namespace InstrumentsStore;
public class Piano : InstrumentItem, IinstrumentActions
{
    public int? NumKeys {get; set;}
    public bool? IsDigital {get; set;}

    public Piano(string name, string brand, decimal price, int quantity, int numkeys, bool isdigital) 
    : base(name, brand, price, quantity)
    {
        NumKeys = numkeys;
        IsDigital = isdigital;
    }


    public override string ListItems()
    {
        return $"Bsttery: {Name}, Brand: {Brand}, Price: ${Price}, Keys: {NumKeys}, Is digital? {IsDigital}";
    }

    public string Play()
    {
        return $"Playing {Name} piano";
    }
}