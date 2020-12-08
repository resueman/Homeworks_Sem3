using System;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Test2
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                MeasureSynchromousMD5Calculation();                
            }
            catch (IncorrectPathException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private async static void MeasureSynchromousMD5Calculation()
        {
            using var md5Hash = MD5.Create();
            var checkSumCalculator = new CheckSumCalculator(md5Hash);
            var path = Console.ReadLine();
            var timeTaken = await CalculateAverage(path, checkSumCalculator);
            Console.WriteLine("The average time taken: " + timeTaken.ToString(@"m\:ss\.fff"));

            var time = TimeSpan.Zero;
            var timer = new Stopwatch();
            for (var i = 0; i < 100; ++i)
            {
                timer.Start();
                await checkSumCalculator.Calculate(path);
                timer.Stop();
                // time.Add((timer.Elapsed.Subtract(timeTaken).Multiply());
            }
        }

        private  async static Task<TimeSpan> CalculateAverage(string path, CheckSumCalculator checkSumCalculator)
        {
            var timeTaken = TimeSpan.Zero;
            var timer = new Stopwatch();
            for (var i = 0; i < 100; ++i)
            {
                timer.Start();
                await checkSumCalculator.Calculate(path);
                timer.Stop();
                timeTaken.Add(timer.Elapsed);
            }
            return timeTaken.Divide(100);
        }
    }
}
