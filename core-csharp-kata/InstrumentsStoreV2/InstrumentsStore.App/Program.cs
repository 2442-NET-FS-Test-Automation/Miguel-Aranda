// using InstrumentsStore.Domain;
namespace InstrumentsStore.App;
public class Program
{

    // I stole this idea from github, now I see it looks nicer
    public static void PrintMenu()
    {
        Console.WriteLine("-===============-\nWelcome to the InstrumentsStore! please, select an option \n-===============-");
        Console.WriteLine("Option 1: BUY AN INSTRUMENT");
        Console.WriteLine("Option 2: LIST YOUR INSTRUMENTS");
        Console.WriteLine("Option 3: PLAY INSTRUMENT");
        Console.WriteLine("Option 4: CHECK BALANCE");
        Console.WriteLine("Option 5: Exit program\n");
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
                            Console.WriteLine("You haven't bought any instruments yet!\n");
                            break;
                        }

                    foreach(var item in client.MyInstrument)
                    {
                        Console.WriteLine($"{item.ListItems()}\n");
                    }
                    break;
                case 3:
                    // logic here
                    break;
                case 4:
                    Console.WriteLine($"Your current balance: {client.Balance}");
                    break;
                case 5:
                    start = false;
                    break;
                default:
                    Console.WriteLine("That option doesn't exist\n");
                    break;
            }
        }
    }
}
