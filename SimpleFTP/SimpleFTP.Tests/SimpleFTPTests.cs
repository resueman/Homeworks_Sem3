using NUnit.Framework;
using System;
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
            if (!server.IsStopped)
            {
                server.Stop();
            }
            client.Dispose();
        }

        [Test]
        [TestCaseSource(typeof(SimpleFTPTestsTestCases), "ListTestCases")]
        public async Task ListRequestReturnsRightSizeOfContentTest(string path, int expectedSize, List<(string, bool)> list)
        {
            var (size, _) = await client.List(path);
            Assert.AreEqual(expectedSize, size);
        }

        [Test]
        [TestCaseSource(typeof(SimpleFTPTestsTestCases), "ListTestCases")]
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
        [TestCaseSource(typeof(SimpleFTPTestsTestCases), "GetTestCases")]
        public async Task GetRequestReturnsRightSizeOfContentTest(string fileToDownload, int expectedSize, string pathToDownloadTo, string pathToExpected)
        {
            var (size, _) = await client.Get(fileToDownload);
            Assert.AreEqual(expectedSize, size);
        }

        [Test]
        [TestCaseSource(typeof(SimpleFTPTestsTestCases), "GetTestCases")]
        public async Task GetRequestReturnsExpectedFileContentTest(string fileToDownload, int expectedSize, string pathToDownloadTo, string pathToExpected)
        {
            var (_, _) = await client.Get(fileToDownload, pathToDownloadTo);
            var readerExpected = new StreamReader(pathToExpected);
            var readerActual = new StreamReader(pathToDownloadTo);
            Assert.AreEqual(readerExpected.ReadToEnd(), readerActual.ReadToEnd());
        }

        [Test]
        public async Task ServerSupportsSeveralClients()
        {
            var client1 = new Client();
            var bigTask = client.Get("../../../bigFile.txt");
            var (s, _) = await client1.List("../../../TestFolder");
            Assert.AreEqual(6, s);
            Assert.IsFalse(bigTask.IsCompleted);
        }

        [Test]
        [TestCaseSource(typeof(SimpleFTPTestsTestCases), "ListTestCases")]
        public async Task ServerWorksAfterOneOfClientClosing(string path, int expectedSize, List<(string, bool)> expectedContent)
        {
            var client1 = new Client();
            _ = client1.Get("../../../TestFolder\\SimpleFTP.Tests.runtimeconfig.dev.json");
            client1.Dispose();
            
            var (size, content) = await client.List(path);
            Assert.AreEqual(expectedSize, size);
            if (expectedContent is null)
            {
                Assert.IsNull(content);
                return;
            }
            CollectionAssert.AreEquivalent(expectedContent, content);
            
            var client2 = new Client();
            var (s, _) = await client2.List("../../../TestFolder");
            Assert.AreEqual(6, s);
        }

        [Test]
        public void ClientThrowsExpectedExceptionWhenServerDoesNotStartedTest()
        {
            Assert.Throws<ConnectionToServerException>(() => new Client(port: 8889));
        }
    }
}
