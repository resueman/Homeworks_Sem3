using NUnit.Framework;
using System;

namespace MyThreadPool.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var myThreadPool = new MyThreadPool(10);
            var task = myThreadPool.QueueWorkItem(() => 20 + 8).ContinueWith(x => x.ToString()).ContinueWith(x => x + "yars");
            var task2 = myThreadPool.QueueWorkItem(() => { myThreadPool.Shutdown(); return 5; });
            var task3 = myThreadPool.QueueWorkItem(() => "hhh" + "hhh");
            
            var task4 = task2.ContinueWith(x => x * 100);
            var task5 = task2.ContinueWith(x => x + "years");
            if (task5.IsCompleted)
            {
                var a = task5.Result;
            }
            else
            {
                var blocked = task5.Result;
            }

            int x = int.Parse(Console.ReadLine());
            var task6 = myThreadPool.QueueWorkItem(() => 9 / x);
            try
            {
                var a = task6.Result;
            }
            catch (AggregateException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}