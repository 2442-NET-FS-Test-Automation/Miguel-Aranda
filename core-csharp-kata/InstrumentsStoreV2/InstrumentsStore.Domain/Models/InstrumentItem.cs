namespace InstrumentsStore;

// Parent class for all instruments {Guitar, Battery, Piano}
public abstract class InstrumentItem
{
    public int Id {get;}
    public string? Name {get; set;}
    public string? Brand {get; set;}
    public decimal Price {get; set;}
    public int? Quantity {get; set;}

    private static int _nextId = 1; 

    protected InstrumentItem(string name, string brand, decimal price, int quantity)
    {
        Id = _nextId++;
        Name = name;
        Brand = brand;
        Price = price;
        Quantity = quantity;
    } 

    public abstract string ListItems();

    public override string ToString() => ListItems();
}