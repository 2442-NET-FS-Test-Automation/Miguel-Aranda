using System.Diagnostics.Metrics;

namespace InstrumentsStore;

public class Sshop : ISshop 
{
    public List<InstrumentItem> Inventory {get; set;} = new List<InstrumentItem>();
    public Client Client {get; set;}
    
    public Sshop (Client client)
    {
        Client = client;

        // Guitars in inventory
        Inventory.Add(new Guitar("Stratocaster", "Fender", 680.00m, 1, 6, false));
        Inventory.Add(new Guitar("Takamine GN20-NS", "Takamine", 300m, 3, 6, true));

        // Drums in inventory
        Inventory.Add(new Drum("Pearl Export EXX", "Pearl", 899.0m, 2, 10, true));
        Inventory.Add(new Drum("Tama LJK28S-AQB Club-JAM", "Tama", 699m, 4, 8, true));

        // Pianos in inventory
        Inventory.Add(new Piano("Yamaha P-145", "Yamaha", 399m,1,88,true));
        Inventory.Add(new Piano("Yamaha CLP-835B", "Yamaha", 2500m,3,88,true));
    }
    public void Buy()
    {
        Console.WriteLine("\n-====- Available items -====-");
        foreach(var item in Inventory)
        {
            Console.WriteLine($"[{item.Id}] {item.ListItems()} -> Stock: {item.Quantity}");
        }

        Console.WriteLine($"Choose an item to buy (numbers only)");
        int id = int.Parse(Console.ReadLine());

        // choose element by id
        InstrumentItem choosen = Inventory.FirstOrDefault(x => x.Id == id);

        // If id can't be found | doesn't exist
        if(choosen == null)
        {
            Console.WriteLine("Instrument cannot be found!");
            return;
        }

        if(choosen.Quantity <= 0)
        {
            Console.WriteLine("Item out of stock!");
            return;
        }

        // if bought then decrease stock by 1
        choosen.Quantity --;
        // Add instrument bought to your list
        Client.MyInstrument.Add(choosen);

        Console.WriteLine($"You bought: {choosen.Name}. Remaining stock: {choosen.Quantity}");

    }


    // there once was this method idea about to be implemented but the lazyness controlled me
    // public void Sell()
    // {
    //     Console.WriteLine("\n-====- SELL -====-\n What items would you like to sell?");
    // }
}