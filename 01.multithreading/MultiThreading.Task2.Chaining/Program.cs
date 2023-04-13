/*
 * 2.	Write a program, which creates a chain of four Tasks.
 * First Task – creates an array of 10 random integer.
 * Second Task – multiplies this array with another random integer.
 * Third Task – sorts this array by ascending.
 * Fourth Task – calculates the average value. All this tasks should print the values to console.
 */
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MultiThreading.Task2.Chaining
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(".Net Mentoring Program. MultiThreading V1 ");
            Console.WriteLine("2.	Write a program, which creates a chain of four Tasks.");
            Console.WriteLine("First Task – creates an array of 10 random integer.");
            Console.WriteLine("Second Task – multiplies this array with another random integer.");
            Console.WriteLine("Third Task – sorts this array by ascending.");
            Console.WriteLine("Fourth Task – calculates the average value. All this tasks should print the values to console");
            Console.WriteLine();

            Random random = new Random();

            Task<int[]> createArrayTask = Task.Run(() =>
            {
                int[] array = Enumerable.Range(1, 10)
                    .Select(_ => random.Next(1, 10000))
                    .ToArray();
                Console.WriteLine($"1. Created array: {string.Join(", ", array)}");
                return array;
            });

            Task<int[]> multiplyTask = createArrayTask.ContinueWith(task =>
            {
                int multiplier = random.Next(1, 10);
                int[] multipliedArray = task.Result.Select(x => x * multiplier).ToArray();
                Console.WriteLine($"2. Multiplied array with {multiplier}: {string.Join(", ", multipliedArray)}");
                return multipliedArray;
            });

            Task<int[]> sortTask = multiplyTask.ContinueWith(task =>
            {
                int[] sortedArray = task.Result;
                Array.Sort(sortedArray);
                Console.WriteLine($"3. Sorted array: {string.Join(", ", sortedArray)}");
                return sortedArray;
            });

            Task<double> averageTask = sortTask.ContinueWith(task =>
            {
                double average = task.Result.Average();
                Console.WriteLine($"4. Average value: {average}");
                return average;
            });

            Task.WaitAll(createArrayTask, multiplyTask, sortTask, averageTask);

            Console.ReadLine();
        }
    }
}
