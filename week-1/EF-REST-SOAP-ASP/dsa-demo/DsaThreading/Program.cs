using System.Collections.Concurrent;
using System.Diagnostics;
namespace DsaThreading;

public class Program
{
    public static void Main()
    {
        ThreadingDemo();
    }
    

    static void ThreadingDemo()
    {
        // Lets take a look at how C# manages Threads (OS Threads not CPU threads.)
        // In C# Threads are an object - like everything else. Typically they're managed
        // By the runtime behind the scenes. For example, when this runs to print Hello World
        // a thread object is created to handle that work
        Console.WriteLine($"Main runs on thread #{Environment.CurrentManagedThreadId}");

        // We can create our own threads - using the Thread class. It's constructor takes on argument.
        // It takes a delegate (we can define with a lambda OR pass it some prewritten method) to run
        // inside the thread
        var workerThread = new Thread(() =>
        {

            Console.WriteLine($"Hello from Thread #{Environment.CurrentManagedThreadId}");
        });

        // Once we have a thread setup - we have to manually start it
        Console.WriteLine($"Before Start() call, isAlive = {workerThread.IsAlive}"); // Unstarted

        workerThread.Start(); // Thread is now running

        Console.WriteLine($"During thread delegate running, isAlive = {workerThread.IsAlive}"); 

        workerThread.Join(); // Our thread was called from the Main function's thread
        // Calling .Join() blocks the outer/caller thread similar to an await

        Console.WriteLine($"After Join() call, isAlive = {workerThread.IsAlive}"); // Stopped

        // Paralelism vs concurrency
        // Inverleaving - Below even the runtime the actual OS scheduler (the thing the kernel, uses to map
        // OS threads to CPU threads) interleaves the threads - switches them on and off CPU threads really fast
        // according to rules that we can't influence from our program - so our threads don't really complete
        // in the same order 100% of the time. This can make our code non-deterministic - which is the problem

        // Concurrency - tasks in progress (interleaved, even on one CPU core)
        // Parallelims - tasks executing at the same time (multiple cpu cores)

        // Threads give us concurrency, true parallelims depends on the hardware (and kernel)

        var threads = new List<Thread>(); // empty list of threads
        // Lets just use a loop to create a few really fast
        for (int i = 1; i <= 5; i++)
        {
            int id = i;
            var th = new Thread(() =>
            {
            Thread.Sleep(Random.Shared.Next(5,40)); // Simulating some work
            Console.WriteLine($"Worker {id} finished on thread #{Environment.CurrentManagedThreadId}"); 
            });
            threads.Add(th);
            th.Start();
        }
        foreach(Thread thread in threads) thread.Join(); // join call on each thread 

        // Thread safe collections

        // Ordinary collections are not optimized or built with multiple threads in mind - they would corrupt or
        // more likely throw runtime exceptions if two thread delegates accessed them concurrently
        // Thankfully there are thread safe version of common collections and methods
        var counts = new ConcurrentDictionary<int, int>();
        var threadPool = new List<Thread>();
        for (int i = 1; i <= 8; i++)
        {
            int id = i;
            var th = new Thread(() =>
            {
            for(int k = 0; k < 1000; k++)
                {
                    counts.AddOrUpdate(id, 1, (_, prev) => prev + 1);
                    // In the line above, AddOrUpdate takes the key, and a third argument
                    // a delegate to execute if the key already exists
                    // _ = C#
                    // delegate wont use it
                    // prev - the existing integer value currently stored for that key
                    // prev + 1 = increment that value giving us a new key to insert
                }
            });
            threads.Add(th);
            th.Start();
        }
        foreach (var th in threadPool) th.Join(); // join to block main's thread
        Console.WriteLine($"Recorded {counts.Values.Sum()} increments across {counts.Count} threads");

        // When working with Threads, it's common to not manually create the threads ourselves
        // For short work items like what we did above, we can use the ThreadPool.
        // The ThreadPool is just a runtime managed set of background threads that we don't have to 
        // create or destroy - they're already there we can just borrow one

        // lets make a ConcurrentQueue for FIFO work, we'll just have it store ints
        var done = new ConcurrentQueue<int>();

        for(int i = 0; i < 5; i++)
        {
            int n = i;

            // instead of creating a thread manually and starting it I can just ask for a thread from
            // the background ThreadPool and pass it some delegate 
            ThreadPool.QueueUserWorkItem(_ => done.Enqueue(n * n));
        }

        // Because we don't actually have the Threads themselves at our disposal - we'll
        // do like a crude await
        while (done.Count < 5) Thread.Sleep(5); // await but way dumber

        Console.WriteLine($"ThreadPool finished. {string.Join(", ", done.OrderBy(x => x))}");

        // Tasks. Creating Threads , Starting and Joining them manually works.
        // But its very low level. You manage each Thread, you can't return a value in a straightforward way,
        // etc. Thankfully we have the Task Parallel library. It's like a modern Layer on top.

        ParallelSum();
        static void ParallelSum()
        {
            // Just a big int array
            int[] data = Enumerable.Range(1, 800000).ToArray();

            // First lets do this tollay sequentially - one thread without tasks
            var sw = Stopwatch.StartNew(); // Using a Stopwatch object to track execution time
            long sequential = SumRange(data, 0, data.Length);
            sw.Stop();
            Console.WriteLine($"Sequential sum = {sequential}. {sw.ElapsedTicks} ticks, 1 thread");
            
            
            // Before we parallelize this, lets play with Tasks
            // Manually splitting the summing into two tasks, each gets half the total numbers
            Task<long> half1 = Task.Run(() => SumRange(data, 0, data.Length / 2));
            Task<long> half2 = Task.Run(() => SumRange(data, data.Length / 2, data.Length));

            long total = half1.Result + half2.Result; // Asking for the Result of a Task that is blocking
            Console.WriteLine($"Two task sum: {total}");

            // Lets parallelize this with Tasks and the TPL library
            long parallelTotal = 0;
            sw.Restart(); // restating my stopwatch back to 0 ticks - then begin counting

            Parallel.For(0, data.Length, 
                // After we give it start and end values for the loop - this is a for loop
                // We give it an accumulator
                () => 0L,
                // body for each loop iteration on a given thread do something
                // i is the loop index, _ discards the ParallelLoopState, local is the current
                // threads subtotal for the sum
                (i, _, local) => local + data[i],
                //localFinally : AFTER a thread finishes all its assigned items this is called
                // adds the Thread's local Sum (the thing that starts with a value of 0L (Long))
                // to the global parallelTotal
                local => Interlocked.Add(ref parallelTotal, local) // combine per Thread sums to the outer variable
            );
            sw.Stop();
            Console.WriteLine($"Sequential sum = {sequential}. {sw.ElapsedTicks} ticks, multi-thread");
        }

        static long SumRange(int[] a, int start, int end)
        {
            long sum = 0;
            for(int i= start; i < end; i++)
            {
                sum+= a[i];
            }
            return sum;
        }

        SafeDemo();

        static void SafeDemo()
        {
            var bank = new Bank();
            Parallel.For(0, 100000, _ => bank.DepositUnSafe(1)); // 100k threads worth of + 1
            Console.WriteLine($"safe balance = {bank.Balance} (expected 100000)");
            // Our balance is wrong every time - and it's a different wrong answer every time
            // This is the worst kind of bug. Because it's not deterministic.
        }

        InterLockedDemo();
        static void InterLockedDemo()
        {
            long counter = 0;
            Parallel.For(0, 100000, _ => Interlocked.Increment(ref counter));
            Console.WriteLine($"Interlocked = {counter} (Expected 100000)");
        }

        // Deadlocks and Starvation

        // Deadlock - If two tasks create locks on resources the other ends up needing
        // they can deadlock. In this case they never resolve - our console app
        // would be waiting forever

        // Starvation - A thread gets blocked by another threads work - and stays alive
        // but cannot progress. Different from a deadlock - because the other thread is able to resolve
        // This starved thread persists - potentially starving the ThreadPool

        // Cancellation tokens
        CancellationDemo();

        // Rather than abruptly killing a thread or having it die via some exception
        // potentially leading to data loss - we can use a cancellation token to ASK a thread to be ended
        static void CancellationDemo()
        {
            // Calling for a cancellationToken, having it auto-cancel after 100ms
            // Side not using: Once we exit the scope where the variable created with using
            // lives in - dispose of it
            using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));
            CancellationToken token = cts.Token;

            var work = Task.Run(() =>
            {
               for (long i = 0; ; i++)
                {
                    token.ThrowIfCancellationRequested();
                    if (i % 50000000 == 0){/* Some simulated work*/}
                }
            }, token);

            try
            {
                work.Wait(); // The task is going - we want to have our code wait for it here
            }
            catch (OperationCanceledException ex) when (ex.InnerException is OperationCanceledException)
            {
                Console.WriteLine("How you'd get here?");
            }
        }

        ExceptionDemo();

        static void ExceptionDemo()
        {
            var t = Task.Run(() => throw new InvalidOperationException("oops - but in a task"));

            // Counter-intuitively, an exception inside a task DOESN'T crash on the spot
            // We'd imagine that line 279 exception is thrown. It's actually
            // thrown during the t.Wait() below.
            // Our task starts up here when we call run...
            try
            {
                t.Wait();
            }
            catch (AggregateException ex)
            {   // Aggregate exceptions themselves are kind of weird
                // One tasks can have several faults - so they get thrown inside an AggregateException
                Console.WriteLine($"Caught: {ex.InnerException!.Message}");
            }
        }
    }
}
