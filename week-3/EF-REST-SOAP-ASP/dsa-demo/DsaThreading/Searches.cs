namespace DsaThreading;

public static class Searches
{
    // Linear search: O(n) - walk this array until we find what we want.
    // Sorted or unsorted doesn't really matter, unsorted OK
    public static int LinearSearch(int[] data, int target)
    {
        // We could probably use a foreach but that is itself abstraction
        for(int i=0; i < data.Length; i++)
        {
            if(data[i] == target) return i;
        }
        // if we don't find it return -1
        return -1;
    }

    // Binary search - halve the search space each space
    // O(Log n) - but we must be sorted we start
    // public static int BinarySearch(int[] sorted, int target)
    // {
        
    // }
}