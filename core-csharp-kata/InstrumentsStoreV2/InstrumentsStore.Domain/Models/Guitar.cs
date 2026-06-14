namespace InstrumentsStore;
public class Guitar : InstrumentItem, IinstrumentActions
{
    public int? NumStrings {get; set;}
    public bool? IsAcoustic {get; set;}

    public Guitar(string name, string brand, decimal price, int quantity, int numstrings, bool isacoustic) 
    : base(name, brand, price, quantity)
    {
        NumStrings = numstrings;
        IsAcoustic = isacoustic;
    }


    public override string ListItems()
    {
        return $"Guitar: {Name}, Brand: {Brand}, Price: ${Price}, Strings: {NumStrings}, Is Acoustic?: {IsAcoustic}";
    }

    public string Play()
    {
        return $"Playing {Name} guitar!";
    }
}