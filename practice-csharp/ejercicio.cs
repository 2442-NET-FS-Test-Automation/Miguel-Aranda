public class ejercicio
{
    public int[] Resolver(int[] a, int[] b)
    {
        var inB = new HashSet<int>(b);
        var result = new List<int>();

        foreach (var element in a)
        {
            if (!inB.Contains(element))
            {
                result.Add(element);
            }
        }

        return result.ToArray();
    }

    
}