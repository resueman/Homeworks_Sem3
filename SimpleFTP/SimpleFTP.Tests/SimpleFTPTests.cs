using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SimpleFTP.Tests
{
    // Contains methods to test compliance цшер implemented file transport protocol
    public class SimpleFTPTests
    {
        private Server server;
        private Client client;

        [SetUp]
        public void Setup()
        {
            server = new Server();
            server.Start();
            client = new Client();
        }

        [TearDown]
        public void TearDown()
        {
            server.Stop();
            client.Dispose();
        }

        [Test]
        [TestCaseSource(typeof(TestCases), "ListTestCases")]
        public async Task ListRequestReturnsRightSizeOfContentTest(string path, int expectedSize, List<(string, bool)> list)
        {
            var (size, _) = await client.List(path);
            Assert.AreEqual(expectedSize, size);
        }

        [Test]
        [TestCaseSource(typeof(TestCases), "ListTestCases")]
        public async Task ListRequestShouldReturnContentOfFolderInCorrectFormatTest(string path, int expectedSize, List<(string name, bool isDirectory)> expectedContent)
        {
            var (_, content) = await client.List(path);
            if (expectedContent is null)
            {
                Assert.IsNull(content);
                return;
            }
            CollectionAssert.AreEquivalent(expectedContent, content);
        }

        [Test]
        [TestCaseSource(typeof(TestCases), "GetTestCases")]
        public async Task GetRequestReturnsRightSizeOfContentTest(string fileToDownload, int expectedSize, string pathToDownloadTo, string pathToExpected)
        {
            var (size, _) = await client.Get(fileToDownload);
            Assert.AreEqual(expectedSize, size);
        }

        [Test]
        [TestCaseSource(typeof(TestCases), "GetTestCases")]
        public async Task GetRequestReturnsExpectedFileContentTest(string fileToDownload, int expectedSize, string pathToDownloadTo, string pathToExpected)
        {
            var (_, _) = await client.Get(fileToDownload, pathToDownloadTo);
            var readerExpected = new StreamReader(pathToExpected);
            var readerActual = new StreamReader(pathToDownloadTo);
            Assert.AreEqual(readerExpected.ReadToEnd(), readerActual.ReadToEnd());
        }
    }
}
