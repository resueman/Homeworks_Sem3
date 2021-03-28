using System.Collections.Generic;

namespace SimpleFTP.Tests
{
    /// <summary>
    /// Test cases for SimpleFTPTests class
    /// </summary>
    public static class SimpleFTPTestsTestCases
    {
        public static readonly object[] ListTestCases =
        {
            new object[]
            {
                "../../../TestFolder",
                new List<(string name, bool isDirectory)>
                {
                    ("../../../TestFolder\\FolderWithoutContent", true),
                    ("../../../TestFolder\\Subfolder1", true),
                    ("../../../TestFolder\\empty.txt", false),
                    ("../../../TestFolder\\notEmpty.txt", false),
                    ("../../../TestFolder\\SimpleFTP.Tests.pdb", false),
                    ("../../../TestFolder\\SimpleFTP.Tests.runtimeconfig.dev.json", false)
                }
            },
            new object[]
            {
                "../../../TestFolder\\notEmpty.txt",
                null
            },
            new object[]
            {
                "../../../TestFolder\\Subfolder1",
                new List<(string name, bool isDirectory)> { ("../../../TestFolder\\Subfolder1\\FolderWithoutSubfolders", true) }
            },
            new object[]
            {
                "../../../TestFolder\\FolderWithoutContent",
                null
            },
            new object[]
            {
                "../../../NotExistingFolder",
                null
            },
            new object[]
            {
                "../../../TestFolder\\Subfolder1\\FolderWithoutSubfolders",
                new List<(string name, bool isDirectory)> 
                {
                    ("../../../TestFolder\\Subfolder1\\FolderWithoutSubfolders\\file1.txt", false), 
                    ("../../../TestFolder\\Subfolder1\\FolderWithoutSubfolders\\file2.txt", false), 
                    ("../../../TestFolder\\Subfolder1\\FolderWithoutSubfolders\\file3.txt", false)
                }
            }
        };

        private static readonly object[] GetTestCases =
        {
            new object[]
            {
                "../../../TestFolder\\notEmpty.txt",
                30
            },
            new object[]
            {
                "../../../TestFolder\\empty.txt",
                0
            },
            new object[]
            {
                "../../../TestFolder\\SimpleFTP.Tests.runtimeconfig.dev.json",
                316
            }
        };
    }
}
