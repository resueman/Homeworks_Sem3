using System;

namespace Lazy.Tests
{
    /// <summary>
    /// Class, which instances play role of objects with deffered creation in test classes
    /// </summary>
    public class GiantMatrix
    {
        public int[,] Elements { get; set; }

        public GiantMatrix(int lines, int columns)
        {
            Elements = new int[lines, columns];
            var random = new Random();
            for (var i = 0; i < lines; ++i)
            {
                for (var j = 0; j < columns; ++j)
                {
                    Elements[i, j] = random.Next(1000);
                }
            }
        }
    }
}
