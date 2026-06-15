using System.Diagnostics.Metrics;
using System.Numerics;

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
            Console.WriteLine($"{item.ListItems()} -> Stock: {item.Quantity}");
        }

        Console.WriteLine($"Choose an item to buy (numbers only)");
        int id = int.Parse(Console.ReadLine());

        // choose element by id
        InstrumentItem choosen = Inventory.FirstOrDefault(x => x.Id == id);

        // If id can't be found | doesn't exist
        if(choosen == null)
        {
            Console.WriteLine("Instrument cannot be found!\n");
            return;
        }

        if(choosen.Quantity <= 0)
        {
            Console.WriteLine("Item out of stock!\n");
            return;
        }

        // If client doesn't have enought money won't be able to buy the product
        if(Client.Balance < choosen.Price)
        {
            Console.WriteLine("You don't have enought money!\n");
            return;
        }

        // if bought then decrease stock by 1
        choosen.Quantity --;
        // Add instrument bought to your list
        Client.MyInstrument.Add(choosen);

        // Balance gets decreased
        Client.Balance -= choosen.Price;

        Console.WriteLine($"You bought: {choosen.Name}. You paid ${choosen.Price} \nRemaining stock: {choosen.Quantity}\n");

    }

    // Method for client to sell instruments
    public void Sell()
    {

        Console.WriteLine("-============- SELL -============-\n");

        foreach(var item in Client.MyInstrument)
        {
            Console.WriteLine($"{item.ListItems()}\n");
        }

        if(Client.MyInstrument.Count == 0)
        {
            Console.WriteLine("You don't have any instruments to SELL!\n");
            return;
        }

        Console.Write("What items would you like to sell\n");

        int id = int.Parse(Console.ReadLine());
        InstrumentItem choosen = Client.MyInstrument.FirstOrDefault(x => x.Id == id);

        // Add it to the store's stock
        choosen.Quantity++;
        
        // Sum instruments price once it is sold
        decimal MoneyReceived = choosen.Price;
        Client.Balance += choosen.Price;

        // Once sold remove it from client's list
        Client.MyInstrument.Remove(choosen);

        Console.WriteLine($"You sold {choosen.Name}. You earned: {MoneyReceived} \n Remaining stock in store {choosen.Quantity} \n");
    }
}