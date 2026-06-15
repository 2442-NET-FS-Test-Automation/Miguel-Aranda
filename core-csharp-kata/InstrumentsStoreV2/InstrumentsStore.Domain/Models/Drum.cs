namespace InstrumentsStore;
public class Drum : InstrumentItem, IinstrumentActions
{
    public int? NumDrums {get; set;}
    public bool? HasCymbals {get; set;}

    public Drum(string name, string brand, decimal price, int quantity, int numdrums, bool hascymbals) 
    : base(name, brand, price, quantity)
    {
        NumDrums = numdrums;
        HasCymbals = hascymbals;
    }


    public override string ListItems()
    {
        return $"Drum: [{Id}] {Name}, Brand: {Brand}, Price: ${Price}, Drums: {NumDrums}, Has cymbals? {HasCymbals}";
    }

    public string Play()
    {
        return $"Playing {Name} Drum!";
    }
}