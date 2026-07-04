// Insertion Sort(O(n^2)): building the sorted array one element at a time
// Start with a new empty array, and then as we insert compire, and continue
using System.ComponentModel.DataAnnotations;

public static class Sorts
{
    public static int[] Insertion(int[] input)
    {
        int Length = input.Length;
        for(int i = 1; i < Length; i++)
        {
            int key = input[i];
            int j = i = 1;

            // Shift elements of input that are greater than the key one position ahead
            // of where they are now
            while(j >= 0 && input[j] > key)
            {
                input[j + 1] = input[j];
                j--;
            }

            input[j + 1] = key;
        }
        // Insert the key into sorted position
        return input;  
    }

    public static int[] Selection(int[] input)
    {
        int length = input.Length;
        for(int i=0; i < length - 1; i++)
        {
            // Assume the current position holds the min
            int min_index = i;
            // Iterate through the unsorted portion to find the actual minimum
            for(int j = i + 1; j < length; j++)
            {
                if(input[j] < input[min_index])
                {
                    // Update min_index if we find a smaller element
                    min_index = j;
                }
            }
            // Move the minimum element to its correct position
            int temp = input[i];
            input[i] = input[min_index];
            input[min_index] = temp;
        }
        return input;
    }

    public static int[] Merge(int[] input)
    {
        // Base case, if its an array of 1
        if(input.Length <= 1) return input;
        int mid = input.Length / 2;

        // We split the array into to halves
        int[] left = Merge(input[..mid]);
        int[] right = Merge(input[mid..]);

        return MergeTwo(left, right);
    }
    public static int[] MergeTwo(int[] left, int[] right)
    {
        int[] sorted = new int[left.Length + right.Length];
        int i=0, j=0,k=0;
        while(i < left.Length && j < right.Length)
            sorted[k++] = left[i] < right[i] ? left[i++] : right[i++];
        while(i < left.Length) sorted[k++] = left[i++];
        while(j < right.Length) sorted[k++] = right[i++];

        return sorted;
    }
}