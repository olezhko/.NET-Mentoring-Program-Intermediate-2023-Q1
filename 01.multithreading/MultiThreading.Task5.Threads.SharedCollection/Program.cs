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
        static bool elementAdded = false;

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
                    lock (lockObject)
                    {
                        collection.Add(i);
                        elementAdded = true;
                        Monitor.Pulse(lockObject);
                    }
                    Thread.Sleep(100);
                }
            });

            var thread2 = new Thread(() =>
            {
                while (true)
                {
                    lock (lockObject)
                    {
                        while (!elementAdded)
                        {
                            Monitor.Wait(lockObject);
                        }

                        Console.WriteLine($"[{string.Join(", ", collection)}]");
                        elementAdded = false;
                    }
                }
            });

            thread1.Start();
            thread2.Start();

            thread1.Join();
            thread2.Join();


            Console.ReadLine();
        }
    }
}
