/*
*  Create a Task and attach continuations to it according to the following criteria:
   a.    Continuation task should be executed regardless of the result of the parent task.
   b.    Continuation task should be executed when the parent task finished without success.
   c.    Continuation task should be executed when the parent task would be finished with fail and parent task thread should be reused for continuation
   d.    Continuation task should be executed outside of the thread pool when the parent task would be cancelled
   Demonstrate the work of the each case with console utility.
*/
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading.Task6.Continuation
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Create a Task and attach continuations to it according to the following criteria:");
            Console.WriteLine("a.    Continuation task should be executed regardless of the result of the parent task.");
            Console.WriteLine("b.    Continuation task should be executed when the parent task finished without success.");
            Console.WriteLine("c.    Continuation task should be executed when the parent task would be finished with fail and parent task thread should be reused for continuation.");
            Console.WriteLine("d.    Continuation task should be executed outside of the thread pool when the parent task would be cancelled.");
            Console.WriteLine("Demonstrate the work of the each case with console utility.");
            Console.WriteLine();

            // Continuation task should be executed regardless of the result of the parent task
            var parentTask1 = Task.Run(() => DoWork());
            parentTask1.ContinueWith(t => Console.WriteLine("Continuation 1"), TaskContinuationOptions.None);

            // Continuation task should be executed when the parent task finished without success
            var parentTask2 = Task.Run(() => throw new Exception());
            parentTask2.ContinueWith(t => Console.WriteLine("Continuation 2"), TaskContinuationOptions.NotOnRanToCompletion);

            // Continuation task should be executed when the parent task would be finished with fail and parent task thread should be reused for continuation.
            var parentTask3 = Task.Run(() => DoWork());
            parentTask3.ContinueWith(t => Console.WriteLine("Continuation 3"), TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously);

            // Continuation task should be executed outside of the thread pool when the parent task would be cancelled.
            var cts = new CancellationTokenSource();
            var parentTask4 = Task.Run(() => DoWork(cts.Token), cts.Token);
            parentTask4.ContinueWith(t => Console.WriteLine("Continuation 4"), TaskContinuationOptions.LongRunning | TaskContinuationOptions.OnlyOnCanceled);

            // Cancel parent task to trigger continuation
            cts.Cancel();

            // Wait for all tasks to complete
            Task.WhenAll(parentTask1, parentTask2, parentTask3, parentTask4);

            Console.ReadLine();
        }

        static void DoWork(CancellationToken token = default(CancellationToken))
        {
            for (int i = 0; i < 10; i++)
            {
                token.ThrowIfCancellationRequested();
                Thread.Sleep(100);
            }
        }
    }
}
