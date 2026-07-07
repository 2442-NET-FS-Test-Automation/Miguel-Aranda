using VideogameStore.Data.Entities;

namespace VideogameStore.Api.Fullfill;

public class BurstPlanner
{
    // Method to play fulfillment order
    public IReadOnlyList<int> OrderByPriority(IEnumerable<Sale> sales)
    {
        PriorityQueue<int, int> pq = new PriorityQueue<int, int>();
        
        foreach(Sale s in sales)
        {
            // Enqueue each order, if it's Priority is expedited, give it a 0 value, if normal give it 1.
            pq.Enqueue(s.SaleId, s.Priority == Priority.Expedited ? 0 : 1);
        }
        var orderedByPriority = new List<int>();

        // While our PriorityQueue has stuff in it - loop and add those things in the order they exit
        // to our orderedByPriority - uses out params
        while(pq.TryDequeue(out int id, out _))
        {
            orderedByPriority.Add(id);
        } 

        return orderedByPriority; // expedited ids should be first in the list
    }
}