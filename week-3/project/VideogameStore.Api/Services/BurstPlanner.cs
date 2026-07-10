using VideogameStore.Data.Entities;

namespace VideogameStore.Api.Services;

public class BurstPlanner
{
    // Method to play fulfillment order
    public IReadOnlyList<int> OrderByPriority(IEnumerable<Sale> sales)
    {
        // we use a tuple (int, DateTime) as queue priority
        var pq = new PriorityQueue<int, (int PriorityOrder, DateTime SaleDate)>();
        
        foreach(Sale s in sales)
        {
            int priorityScore = s.Priority == Priority.Expedited ? 0 : 1;
            // the queue will order first by 0 (0 before 1)
            // if are equal, order them by the oldest SaleDate (First In, First Out)
            pq.Enqueue(s.SaleId, (priorityScore, s.SaleDate));
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