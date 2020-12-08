using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Test2
{
    /// <summary>
    /// Contains methods to calculate check sum of file or directory using hash algorithm
    /// </summary>
    public class CheckSumCalculator
    {
        private readonly HashAlgorithm hashAlgorithm; 

        /// <summary>
        /// Creates check sum calculator with specified hash algorithm
        /// </summary>
        /// <param name="hashAlgorithm">Algorithm using in check sum calculation</param>
        public CheckSumCalculator(HashAlgorithm hashAlgorithm)
        {
            this.hashAlgorithm = hashAlgorithm;
        }

        /// <summary>
        /// Computes check sum of directory or file using hash algorithm
        /// </summary>
        /// <param name="path">Path to directory or file</param>
        /// <returns>Check sum of directory or file</returns>
        public async Task<byte[]> Calculate(string path)
        {
            if (File.Exists(path))
            {
                return await CalculateFileCheckSum(path);
            }
            else if (Directory.Exists(path))
            {
                return await CalculateDirectoryCheckSum(path);
            }
            throw new IncorrectPathException("No such directory or file");
        }

        private async Task<byte[]> CalculateFileCheckSum(string path)
        {
            using var streamReader = new StreamReader(path);
            var fileContent = await streamReader.ReadToEndAsync();
            return GetHash(Encoding.UTF8.GetBytes(fileContent));
        }

        private async Task<byte[]> CalculateDirectoryCheckSum(string path)
        {
            var files = Directory.GetFiles(path);
            Array.Sort(files);

            var directories = Directory.GetDirectories(path);
            Array.Sort(directories);

            var content = Encoding.UTF8.GetBytes(Path.GetDirectoryName(path));
            foreach (var file in files)
            {
                var fileCheckSum = await CalculateFileCheckSum(file);
                content = Concat(content, fileCheckSum);
            }
            foreach (var directory in directories)
            {
                var directoryCheckSum = await CalculateDirectoryCheckSum(directory);
                content = Concat(content, directoryCheckSum);
            }
            return GetHash(content);
        }

        private byte[] GetHash(byte[] input) => hashAlgorithm.ComputeHash(input);

        private byte[] Concat(byte[] first, byte[] second)
        {
            var resultOfConcatenation = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, resultOfConcatenation, 0, first.Length);
            Buffer.BlockCopy(second, 0, resultOfConcatenation, first.Length, second.Length);
            return resultOfConcatenation;
        }

        /// <summary>
        /// Changes data type of check sum
        /// </summary>
        /// <param name="byteCheckSum">Check sum</param>
        /// <returns>Check sum in string format</returns>
        public static string ByteCheckSumToStringCheckSum(byte[] byteCheckSum)
        {
            var hex = new StringBuilder(byteCheckSum.Length * 2);
            foreach (byte @byte in byteCheckSum)
            {
                hex.AppendFormat("{0:x2}", @byte);
            }
            return hex.ToString();
        }
    }
}
