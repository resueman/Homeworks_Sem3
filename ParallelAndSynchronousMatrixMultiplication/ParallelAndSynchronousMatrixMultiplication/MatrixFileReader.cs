using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ParallelAndSynchronousMatrixMultiplication
{
    public class MatrixFileReader
    {
        private int[,] ParseMatrix(List<string[]> lines)
        {
            var columns = lines[0].Length;
            var matrix = new int[lines.Count, columns];
            for (var i = 0; i < lines.Count; ++i)
            {
                var line = lines[i];
                if (columns != line.Length)
                {
                    throw new ArgumentOutOfRangeException("All lines must be of constant length");
                }
                for (var j = 0; j < line.Length; ++j)
                {
                    var isNumber = int.TryParse(line[j], out var number);
                    if (!isNumber)
                    {
                        throw new ArgumentOutOfRangeException("Matrix must consist of integer numbers");
                    }
                    matrix[i, j] = number;
                }
            }

            return matrix;
        }

        public async Task<int[,]> Read(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("File with this name doesn't exist");
            }

            var lines = new List<string[]>();
            using var streamReader = new StreamReader(path, Encoding.Default);
            while (!streamReader.EndOfStream)
            {
                var line = await streamReader.ReadLineAsync();
                line = Regex.Replace(line, @"\s+", " ").Trim();
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }
                lines.Add(line.Split(' '));
            }

            if (lines.Count == 0)
            {
                throw new ArgumentOutOfRangeException("File doesn't contsin matrix");
            }

            return ParseMatrix(lines);
        }
    }
}
