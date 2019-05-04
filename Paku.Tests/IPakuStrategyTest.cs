using Microsoft.VisualStudio.TestTools.UnitTesting;
using Paku.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.IO.Compression;

namespace Paku.Tests
{
    [TestClass]
    public class IPakuStrategyTest
    {
        private FileInfo CreateTestFile(string name, string content)
        {
            File.WriteAllText(name, content);
            return new FileInfo(name);
        }

        [TestInitialize]
        public void Initialize()
        {
            // clear our test directory
            DirectoryInfo di = new DirectoryInfo(@".\");

            foreach (FileInfo fi in di.EnumerateFiles("*.txt"))
            {
                fi.Delete();
            }

            foreach (FileInfo fi in di.EnumerateFiles("*.zip"))
            {
                fi.Delete();
            }
        }

        [TestMethod]
        public void DeletePakuStrategyTest()
        {
            // create some files
            List<FileInfo> files = new List<FileInfo>();
            files.Add(CreateTestFile("DeletePakuStrategyTest1.txt", "test1"));
            files.Add(CreateTestFile("DeletePakuStrategyTest2.txt", "test2"));

            List<VirtualFileInfo> virtualFiles = files.Select(x => new VirtualFileInfo(x)).ToList();

            IPakuStrategy strategy = new DeletePakuStrategy();
            PakuResult result = strategy.Eat(virtualFiles);

            // all of the files should have been deleted successfully
            Assert.IsTrue(result.Success);
            Assert.AreEqual(virtualFiles.Count, result.RemovedFiles.Count);

            foreach (VirtualFileInfo vfi in virtualFiles)
            {
                FileInfo fi = vfi.ToFileInfo();
                Assert.IsFalse(fi.Exists);
            }

            // and none should have been created
            Assert.AreEqual(0, result.CreatedFiles.Count);
        }

        [TestMethod]
        public void DeletePakuStrategyTestError()
        {
            // create some files
            List<FileInfo> files = new List<FileInfo>();
            files.Add(CreateTestFile("DeletePakuStrategyTestError1.txt", "test1"));
            files.Add(CreateTestFile("DeletePakuStrategyTestError2.txt", "test2"));

            List<VirtualFileInfo> virtualFiles = files.Select(x => new VirtualFileInfo(x)).ToList();

            // insert a non-existent file in the middle
            virtualFiles.Insert(1, new VirtualFileInfo(@"C:\DeletePakuStrategyTestError.notreal"));

            IPakuStrategy strategy = new DeletePakuStrategy();
            PakuResult result = strategy.Eat(virtualFiles);
            // the operation should be marked as a failure
            // the first file should have been deleted
            Assert.IsFalse(result.Success);
            Assert.IsTrue(result.Error is ArgumentException);

            Assert.AreEqual(1, result.RemovedFiles.Count);
            Assert.IsFalse(result.RemovedFiles[0].ToFileInfo().Exists);

            // the last file should be still be on disk
            Assert.IsTrue(virtualFiles[2].ToFileInfo().Exists);
        }

        [TestMethod]
        public void ZipPakuStrategyTest()
        {
            // create some files
            List<FileInfo> files = new List<FileInfo>();
            files.Add(CreateTestFile("ZipPakuStrategyTest1.txt", "test1"));
            files.Add(CreateTestFile("ZipPakuStrategyTest2.txt", "test2"));

            List<VirtualFileInfo> virtualFiles = files.Select(x => new VirtualFileInfo(x)).ToList();

            IPakuStrategy strategy = new ZipPakuStrategy();
            PakuResult result = strategy.Eat(virtualFiles);

            // all of the files should have been deleted successfully
            Assert.IsTrue(result.Success);
            Assert.AreEqual(virtualFiles.Count, result.RemovedFiles.Count);

            // and a zip file should have been created
            Assert.AreEqual(1, result.CreatedFiles.Count);
            FileInfo zipFile = result.CreatedFiles[0].ToFileInfo();
            Assert.IsTrue(zipFile.Exists);

            // verify that zip file contains all removed files
            using (ZipArchive archive = ZipFile.Open(zipFile.FullName, ZipArchiveMode.Read))
            {
                Assert.AreEqual(result.RemovedFiles.Count, archive.Entries.Count);

                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    Assert.IsTrue(result.RemovedFiles.Any(x => x.Name == entry.Name));
                }
            }

            // clean up
            zipFile.Delete();
        }

        [TestMethod]
        public void ZipPakuStrategyTestError()
        {
            // create some files
            List<FileInfo> files = new List<FileInfo>();
            files.Add(CreateTestFile("ZipPakuStrategyTest1.txt", "test1"));
            files.Add(CreateTestFile("ZipPakuStrategyTest2.txt", "test2"));

            List<VirtualFileInfo> virtualFiles = files.Select(x => new VirtualFileInfo(x)).ToList();

            // insert a non-existent file in the middle
            virtualFiles.Insert(1, new VirtualFileInfo(@"C:\ZipPakuStrategyTestError.notreal"));

            IPakuStrategy strategy = new ZipPakuStrategy();
            PakuResult result = strategy.Eat(virtualFiles);

            // result should be marked as failure; only one file should be deleted
            Assert.IsFalse(result.Success);
            Assert.AreEqual(1, result.RemovedFiles.Count);

            // zip file should still have been created
            Assert.AreEqual(1, result.CreatedFiles.Count);
            FileInfo zipFile = result.CreatedFiles[0].ToFileInfo();
            Assert.IsTrue(zipFile.Exists);

            // clean up
            zipFile.Delete();
        }
    }
}
