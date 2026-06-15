// using InstrumentsStore.Domain;
namespace InstrumentsStore.App;
public class Program
{

    // I stole this idea from github, now I see it looks nicer
    public static void PrintMenu()
    {
        Console.WriteLine("-===============--===============--===============--===============-");
        Console.WriteLine("Welcome to the InstrumentsStore! please, select an option");
        Console.WriteLine("-===============--===============--===============--===============-\n");
        Console.WriteLine("Option 1: BUY AN INSTRUMENT");
        Console.WriteLine("Option 2: LIST YOUR INSTRUMENTS");
        Console.WriteLine("Option 3: PLAY INSTRUMENT");
        Console.WriteLine("Option 4: CHECK BALANCE");
        Console.WriteLine("Option 0: Exit program\n");
        Console.Write("please choose: ");
    }

    public static void Main()
    {
        // Creating a client
        Client client = new Client("Miguel",4000m);
        // Letting shop know what client is going to buy an instrument
        Sshop shop = new Sshop(client);
        bool start = true;

        // Always start at the beginning and never stop until start != true
        while(start == true)
        {
        // display menu
        PrintMenu();
        int option = int.Parse(Console.ReadLine());
        switch (option)
            {
                case 1:
                    shop.Buy();
                    break;

                case 2:

                    if (client.MyInstrument.Count() == 0)
                        {
                            Console.WriteLine("\nYou haven't bought any instruments yet!\n");
                            break;
                        }

                    foreach(var item in client.MyInstrument)
                    {
                        Console.WriteLine($"{item.ListItems()}\n");
                    }
                    break;

                case 3:
                    if(client.MyInstrument.Count() == 0)
                    {
                        Console.WriteLine($"\nAll you can play now is an Air Guitar. Go buy something!\n");
                        break;
                    }

                    Console.WriteLine($"\n-===Your instruments===-");

                    foreach(var item in client.MyInstrument)
                    {
                        Console.WriteLine($"{item.ListItems()}\n");
                    }

                    Console.WriteLine($"\nWhat instrument would you like to play? \n");
                    Console.Write("ENTER ID:");

                    int id = int.Parse(Console.ReadLine());

                    // choose element by id
                    InstrumentItem choosen = client.MyInstrument.FirstOrDefault(x => x.Id == id);

                    if(choosen == null)
                    {
                        Console.WriteLine("Instrument cannot be found!");
                        break;
                    }

                    if(choosen is IinstrumentActions playable)
                        Console.WriteLine(playable.Play());
                    
                    break;

                case 4:
                    Console.WriteLine($"\nYour current balance: {client.Balance}");
                    break;

                case 0:
                    Console.WriteLine($"\n Have a good day!\n");
                    start = false;
                    break;

                default:
                    Console.WriteLine("\nThat option doesn't exist\n");
                    break;
            }
        }
    }
}
