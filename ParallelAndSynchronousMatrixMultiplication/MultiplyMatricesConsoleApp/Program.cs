using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using ParallelAndSynchronousMatrixMultiplication;

namespace MultiplyMatricesConsoleApp
{
    class Program
    {
        private static void CompareTimeOfWorkOnConstantMatrixSize()
        {
            Console.WriteLine("Generating matrices...");
            var generator = new MatrixGenerator();
            var left = generator.Generate(500, 500);
            var right = generator.Generate(500, 500);

            var cores = Environment.ProcessorCount;
            var threadCounts = new int[8] { 1, 2, 3, cores, cores + 1, cores * 2, cores * 3, cores * 4 };

            Console.WriteLine("Calculating the time of Synchronous Parallelizing Matrix Multiplier work...");
            var synchronousMultiplierTimestamps = new List<(string, TimeSpan)>
            {
                ("1", ElapseWorkTime(new SynchronousMatrixMultiplier(), left, right))
            };

            Console.WriteLine("Calculating the time of Parallel.For Matrix Multiplier work...");
            var parallelForMultiplierTimestamps = new List<(string, TimeSpan)>
            {
                ("Auto", ElapseWorkTime(new ParallelForMatrixMultiplier(), left, right))
            };

            Console.WriteLine("Calculating the time of Striped Parallelizing Matrix Multiplier work...");
            var strippedMultiplierTimestamps = new List<(string, TimeSpan)>();
            foreach (var threadCount in threadCounts)
            {                
                var strippedMultiplier = new StripedParallelizingMatrixMultiplier(threadCount);
                strippedMultiplierTimestamps.Add((threadCount.ToString(), ElapseWorkTime(strippedMultiplier, left, right)));
            }

            Console.WriteLine("Calculating the time of Sequentially Parallelizing Matrix Multiplier work...");
            var sequentialMultiplierTimestamps = new List<(string, TimeSpan)>();
            foreach (var threadCount in threadCounts)
            {
                var sequentialMultiplier = new SequentiallyParallelizingMatrixMultiplier(threadCount);
                sequentialMultiplierTimestamps.Add((threadCount.ToString(), ElapseWorkTime(sequentialMultiplier, left, right)));
            }

            var multipliersWorkTime = new List<(string Name, List<(string ThreadsCount, TimeSpan Timespan)> Timespans)>
            {
                ("SynchronousMatrixMultiplier", synchronousMultiplierTimestamps),
                ("ParallelForMatrixMultiplier", parallelForMultiplierTimestamps),
                ("SequentiallyParallelizingMatrixMultiplier", sequentialMultiplierTimestamps),
                ("StripedParallelizingMatrixMultiplier", strippedMultiplierTimestamps)
            };

            foreach(var (Name, Timespans) in multipliersWorkTime)
            {
                Console.WriteLine(Name);
                Console.WriteLine("Threads - Time");
                foreach(var (ThreadsCount, Timestamp) in Timespans)
                {
                    Console.WriteLine($"{ThreadsCount} - {FormatTimeSpan(Timestamp)}");
                }
                Console.WriteLine();
            }
        }

        private static void CompareTimeOfWorkDependingOnMatrixSize()
        {
            var multipliers = new List<(string Name, IMatrixMultiplier Multiplier)>
            {
                ("Synchronous Multiplier", new SynchronousMatrixMultiplier()),
                ("Parallel.For Multiplier", new ParallelForMatrixMultiplier()),
                ("Striped Parallelizing Multiplier", new StripedParallelizingMatrixMultiplier()),
                ("Sequentially Parallelizing Multiplier", new SequentiallyParallelizingMatrixMultiplier())
            };

            var generator = new MatrixGenerator();
            for (var i = 1; i < 1001; i += i < 100 ? 1 : i < 500 ? 10 : 100)
            {
                Console.WriteLine($"Size: {i} * {i}");
                var left = generator.Generate(i, i);
                var right = generator.Generate(i, i);
                foreach(var (Name, Multiplier) in multipliers)
                {
                    var timestamp = ElapseWorkTime(Multiplier, left, right);
                    Console.WriteLine($"{Name} - {timestamp.TotalMilliseconds}");
                }
                Console.WriteLine();
            }
        }

        private static string FormatTimeSpan(TimeSpan timeSpan)
            => string.Format("{0:00}:{1:000000000}", timeSpan.Seconds, timeSpan.Milliseconds);

        private static TimeSpan ElapseWorkTime(IMatrixMultiplier multiplier, int[,] left, int[,] right)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            multiplier.Multiply(left, right);
            stopwatch.Stop();
            return stopwatch.Elapsed; 
        }

        public static void Main()
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            Console.WriteLine("Please, choose option");
            Console.WriteLine("Press 1 to compare multipliers time of work on different number of threads and constant matrix size");
            Console.WriteLine("Press 2 to compare multipliers time of work on different matrix sizes");
            Console.WriteLine("Press any key to perform matrix multiplication");

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    CompareTimeOfWorkOnConstantMatrixSize();
                    break;
                case "2":
                    CompareTimeOfWorkDependingOnMatrixSize();
                    break;
            }

            Console.WriteLine("Enter path to file with left matrix");
            var pathLeft = Console.ReadLine();
            Console.WriteLine("Enter path to file with right matrix");
            var pathRight = Console.ReadLine();
            Console.WriteLine("Enter path to file, where result will be written");
            var resultPath = Console.ReadLine();

            int[,] left, right;
            try
            {
                var fileReader = new MatrixFileReader();
                left = await fileReader.ReadAsync(pathLeft);
                right = await fileReader.ReadAsync(pathRight);
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            
            IMatrixMultiplier multiplier;
            Console.WriteLine("\nChoose type of multiplier");
            Console.WriteLine("1 - Synchronous Matrix Multiplier");
            Console.WriteLine("2 - Parallel.For Matrix Multiplier");
            Console.WriteLine("3 - Striped Parallelizing Matrix Multiplier");
            Console.WriteLine("4 - Sequentially Parallelizing Matrix Multiplier");
            Console.WriteLine("Press any key to exit");
            choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    multiplier = new SynchronousMatrixMultiplier();
                    break;
                case "2":
                    multiplier = new ParallelForMatrixMultiplier();
                    break;
                case "3":
                    multiplier = new StripedParallelizingMatrixMultiplier();
                    break;
                case "4":
                    multiplier = new SequentiallyParallelizingMatrixMultiplier();
                    break;
                default:
                    return;
            }

            Console.WriteLine("Multiplying performs...");
            try 
            {
                var result = multiplier.Multiply(left, right);
                var fileWriter = new MatrixFileWriter();
                await fileWriter.WriteAsync(result, resultPath);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.WriteLine(e.Message);
                return;
            }            

            Console.WriteLine("Ready!");
        }
    }
}
