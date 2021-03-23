using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleFTP.Tests
{
    public class TestCases
    {
        private static readonly object[] ListTestCases =
        {
            new object[]
            {
                "../../../TestFolder",
                6,
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
                -1,
                null
            },
            new object[]
            {
                "../../../TestFolder\\Subfolder1",
                1,
                new List<(string name, bool isDirectory)> { ("../../../TestFolder\\Subfolder1\\FolderWithoutSubfolders", true) }
            },
            new object[]
            {
                "../../../TestFolder\\FolderWithoutContent",
                0,
                null
            },
            new object[]
            {
                "../../../NotExistingFolder",
                -1,
                null
            },
            new object[]
            {
                "../../../TestFolder\\Subfolder1\\FolderWithoutSubfolders",
                3,
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
                30,
                "../../../Downloads\\notEmpty.txt",
                "../../../ExpectedDownloads\\notEmpty.txt"
            },
            new object[]
            {
                "../../../TestFolder\\empty.txt",
                0,
                "../../../Downloads\\empty.txt",
                "../../../ExpectedDownloads\\empty.txt"
            },
            new object[]
            {
                "../../../TestFolder\\SimpleFTP.Tests.runtimeconfig.dev.json",
                316,
                "../../../Downloads\\SimpleFTP.Tests.runtimeconfig.dev.json",
                "../../../ExpectedDownloads\\SimpleFTP.Tests.runtimeconfig.dev.json"
            }
        };
    }
}
