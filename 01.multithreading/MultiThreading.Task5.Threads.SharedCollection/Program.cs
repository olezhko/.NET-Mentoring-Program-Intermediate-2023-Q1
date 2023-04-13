/*
 * 5. Write a program which creates two threads and a shared collection:
 * the first one should add 10 elements into the collection and the second should print all elements
 * in the collection after each adding.
 * Use Thread, ThreadPool or Task classes for thread creation and any kind of synchronization constructions.
 */
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading.Task5.Threads.SharedCollection
{
    class Program
    {
        static object lockObject = new object();
        static List<int> collection = new List<int>();

        private static SemaphoreSlim semaphore1 = new SemaphoreSlim(initialCount:0);
        private static SemaphoreSlim semaphore2 = new SemaphoreSlim(initialCount:0);
        static async Task Main(string[] args)
        {
            Console.WriteLine("5. Write a program which creates two threads and a shared collection:");
            Console.WriteLine("the first one should add 10 elements into the collection and the second should print all elements in the collection after each adding.");
            Console.WriteLine("Use Thread, ThreadPool or Task classes for thread creation and any kind of synchronization constructions.");
            Console.WriteLine();

            var thread1 = new Thread(() =>
            {
                for (int i = 1; i <= 10; i++)
                {
                    semaphore1.Wait();
                    lock (lockObject)
                    {
                        collection.Add(i);
                    }
                    semaphore2.Release();
                }
            });

            var thread2 = new Thread(() =>
            {
                while (collection.Count != 10)
                {
                    semaphore2.Wait();
                    lock (lockObject)
                    {
                        Console.WriteLine($"[{string.Join(", ", collection)}]");
                    }
                    semaphore1.Release();
                }
            });

            thread1.Start();
            thread2.Start();

            semaphore1.Release(); // Start with semaphore 1 released

            thread1.Join();
            thread2.Join();

            Console.ReadLine();
        }
    }
}
