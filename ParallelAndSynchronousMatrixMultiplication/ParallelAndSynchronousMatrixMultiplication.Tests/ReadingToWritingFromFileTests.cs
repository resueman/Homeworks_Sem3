using NUnit.Framework;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ParallelAndSynchronousMatrixMultiplication.Tests
{
    public class ReadingToWritingFromFileTests
    {
        private MatrixFileReader fileReader;
        private MatrixFileWriter fileWriter;

        [SetUp]
        public void Setup()
        {
            fileWriter = new MatrixFileWriter();
            fileReader = new MatrixFileReader();
        }

        [TestCase("i'm not exist.txt")]
        public void ReadingFromNotExistingFileTest(string path)
        {
            Assert.ThrowsAsync<FileNotFoundException>(async () => await fileReader.Read(path));
        }

        [TestCase(@"./matrices/matrix_reading/incorrect/empty.txt")]
        public void ReadingMatrixFromEmptyFile(string path)
        {
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await fileReader.Read(path));
        }

        [TestCase(@"./matrices/matrix_reading/incorrect/jagged_array.txt")]
        public void ReadingJaggedArrayTest(string path)
        {
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await fileReader.Read(path));
        }

        [TestCase(@"./matrices/matrix_reading/incorrect/incorrect1.txt")]
        [TestCase(@"./matrices/matrix_reading/incorrect/incorrect2.txt")]
        [TestCase(@"./matrices/matrix_reading/incorrect/incorrect3.txt")]
        [TestCase(@"./matrices/matrix_reading/incorrect/incorrect4.txt")]
        public void ReadingMatrixWithIncorrectSymbols(string path)
        {
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await fileReader.Read(path));
        }

        [TestCaseSource("IsCorrectReadingMatrixFromFileTestCases")]
        public async Task IsCorrectReadingMatrixFromFileTest(int[,] expected, string readingPath)
        {
            var actual = await fileReader.Read(readingPath);
            Assert.IsTrue(FunctionsOnMatrices.AreEqual(expected, actual));
        }

        [TestCaseSource("IsCorrectWritingMatrixToFileTestCases")]
        public async Task IsCorrectWritingMatrixToFileTest(int[,] writtenMatrix, string expectedFilePath)
        {
            var writingPath = "writing_test.txt";
            await fileWriter.Write(writtenMatrix, writingPath);

            string actual, expected;
            using (var streamReader = new StreamReader(writingPath, Encoding.Default))
            {
                actual = await streamReader.ReadToEndAsync();
                actual = Regex.Replace(actual, @"\s+", " ").Trim();
            }
            using (var streamReader = new StreamReader(expectedFilePath, Encoding.Default))
            {
                expected = await streamReader.ReadToEndAsync();
                expected = Regex.Replace(expected, @"\s+", " ").Trim();
            }
            Assert.AreEqual(expected, actual);
        }

        private static object[] IsCorrectWritingMatrixToFileTestCases =
        {        
            new object[]
            {
                Matrices.matrix1, @"./matrices/matrix_writing/expected1.txt"
            },
            new object[]
            {
                Matrices.matrix2, @"./matrices/matrix_writing/expected2.txt"
            },
            new object[] 
            {
                Matrices.matrix3, @"./matrices/matrix_writing/expected3.txt"
            },
            new object[]
            {
                Matrices.matrix4, @"./matrices/matrix_writing/expected4.txt"
            },
            new object[]
            {
                Matrices.matrix5, @"./matrices/matrix_writing/expected5.txt"
            }
        };

        private static object[] IsCorrectReadingMatrixFromFileTestCases =
        {
            new object[]
            {
                Matrices.matrix1, @"./matrices/matrix_reading/matrix1.txt"
            },
            new object[]
            {
                Matrices.matrix2, @"./matrices/matrix_reading/matrix2.txt"
            },
            new object[]
            {
                Matrices.matrix3, @"./matrices/matrix_reading/matrix3.txt"
            },
            new object[]
            {
                Matrices.matrix4, @"./matrices/matrix_reading/matrix4.txt"
            },
            new object[]
            {
                Matrices.matrix5, @"./matrices/matrix_reading/matrix5.txt"
            }
        };
    }
}