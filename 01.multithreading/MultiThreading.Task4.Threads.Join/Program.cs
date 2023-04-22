/*
 * 4.	Write a program which recursively creates 10 threads.
 * Each thread should be with the same body and receive a state with integer number, decrement it,
 * print and pass as a state into the newly created thread.
 * Use Thread class for this task and Join for waiting threads.
 * 
 * Implement all of the following options:
 * - a) Use Thread class for this task and Join for waiting threads.
 * - b) ThreadPool class for this task and Semaphore for waiting threads.
 */

using System;
using System.Threading;

namespace MultiThreading.Task4.Threads.Join
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("4.	Write a program which recursively creates 10 threads.");
            Console.WriteLine("Each thread should be with the same body and receive a state with integer number, decrement it, print and pass as a state into the newly created thread.");
            Console.WriteLine("Implement all of the following options:");
            Console.WriteLine();
            Console.WriteLine("- a) Use Thread class for this task and Join for waiting threads.");
            Console.WriteLine("- b) ThreadPool class for this task and Semaphore for waiting threads.");

            Console.WriteLine();

            ThreadJoinApproach();
            ThreadPoolApproach();
            Console.ReadLine();
        }

        static Semaphore semaphore = new Semaphore(0, 1);
        private static void ThreadPoolApproach()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadPoolProc), 10);

            semaphore.WaitOne();
        }

        static void ThreadPoolProc(Object valueInfo)
        {
            int value = (int)valueInfo;
            Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} with value {value}");

            if (value == 0)
            {
                semaphore.Release();
                return;
            }

            ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadPoolProc), value - 1);
        }

        private static void ThreadJoinApproach()
        {
            int initialState = 10;
            Thread rootThread = new Thread(DecrementAndPrintState);
            rootThread.Start(initialState);
            rootThread.Join();
        }

        private static void DecrementAndPrintState(object valueInfo)
        {
            int value = (int)valueInfo;
            if (value == 0)
            {
                return;
            }

            Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} with value : {value}");

            value--;
            Thread rootThread = new Thread(DecrementAndPrintState);
            rootThread.Start(value);
            rootThread.Join();
        }
    }
}
