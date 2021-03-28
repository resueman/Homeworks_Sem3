using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SimpleFTP.Tests
{
    // Contains methods to test compliance with implemented file transport protocol
    public class SimpleFTPTests
    {
        private const string downloadFolderPath = "../../../Downloads";
        private const string expectedDownloadsFolderPath = "../../../ExpectedDownloads";
        private const string emptyTestFolder = "../../../TestFolder/FolderWithoutContent";

        private Server server;
        private Client client;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            if (!Directory.Exists(downloadFolderPath))
            {
                Directory.CreateDirectory(downloadFolderPath);
            }
            if (!Directory.Exists(emptyTestFolder))
            {
                Directory.CreateDirectory(emptyTestFolder);
            }
        }

        [SetUp]
        public void Setup()
        {
            server = new Server();
            server.Start();
            client = new Client();
            client.Connect();
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
        public async Task ListRequestShouldReturnContentOfFolderInCorrectFormatTest(string path, List<(string name, bool isDirectory)> expectedContent)
        {
            var content = await client.List(path);
            if (expectedContent is null)
            {
                Assert.IsNull(content);
                return;
            }
            CollectionAssert.AreEquivalent(expectedContent, content);
        }

        [Test]
        [TestCaseSource(typeof(SimpleFTPTestsTestCases), "GetTestCases")]
        public async Task GetRequestReturnsRightSizeOfContentTest(string fileToDownload, int expectedSize)
        {
            var pathToDownloaded = await client.Get(fileToDownload, downloadFolderPath);
            var actualSize = new FileInfo(pathToDownloaded).Length;
            Assert.AreEqual(expectedSize, actualSize);
        }

        [Test]
        [TestCaseSource(typeof(SimpleFTPTestsTestCases), "GetTestCases")]
        public async Task GetRequestReturnsExpectedFileContentTest(string fileToDownload, int expectedSize)
        {
            var fileName = Path.GetFileName(fileToDownload);
            var pathToDownloaded = await client.Get(fileToDownload, downloadFolderPath);
            using var readerExpected = new StreamReader($"{expectedDownloadsFolderPath}\\{fileName}");
            using var readerActual = new StreamReader(pathToDownloaded);
            Assert.AreEqual(readerExpected.ReadToEnd(), readerActual.ReadToEnd());
        }

        [Test]
        public async Task ServerSupportsSeveralClients()
        {
            var client1 = new Client();
            client1.Connect();
            var bigTask = client.Get("../../../bigFile.txt", downloadFolderPath);
            var content = await client1.List("../../../TestFolder");
            Assert.AreEqual(6, content.Count);
            Assert.IsFalse(bigTask.IsCompleted);
        }

        [Test]
        [TestCaseSource(typeof(SimpleFTPTestsTestCases), "ListTestCases")]
        public async Task ServerWorksAfterOneOfClientClosing(string path, List<(string, bool)> expectedContent)
        {
            var client1 = new Client();
            client1.Connect();
            _ = client1.Get("../../../TestFolder\\SimpleFTP.Tests.runtimeconfig.dev.json", downloadFolderPath);
            client1.Dispose();
            
            var content = await client.List(path);
            if (expectedContent is null)
            {
                Assert.IsNull(content);
                return;
            }
            CollectionAssert.AreEquivalent(expectedContent, content);
            
            var client2 = new Client();
            client2.Connect();
            content = await client2.List("../../../TestFolder");
            Assert.AreEqual(6, content.Count);
        }

        [Test]
        public void ClientThrowsExpectedExceptionWhenServerDoesNotStartedTest()
        {
            Assert.Throws<ConnectionToServerException>(() => new Client(port: 8889).Connect());
        }
    }
}
