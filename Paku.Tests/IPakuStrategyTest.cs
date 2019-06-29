using Microsoft.VisualStudio.TestTools.UnitTesting;
using Paku.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.IO.Compression;
using Newtonsoft.Json;
using Paku.Models.Config;

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
            DirectoryInfo dir = new DirectoryInfo(@"C:\foo");

            List<FileInfo> files = new List<FileInfo>();
            files.Add(CreateTestFile("DeletePakuStrategyTest1.txt", "test1"));
            files.Add(CreateTestFile("DeletePakuStrategyTest2.txt", "test2"));

            List<VirtualFileInfo> virtualFiles = files.Select(x => new VirtualFileInfo(x)).ToList();

            IPakuStrategy strategy = new DeletePakuStrategy();
            PakuResult result = strategy.Eat(dir, virtualFiles, null);

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
            DirectoryInfo dir = new DirectoryInfo(@"C:\foo");

            List<FileInfo> files = new List<FileInfo>();
            files.Add(CreateTestFile("DeletePakuStrategyTestError1.txt", "test1"));
            files.Add(CreateTestFile("DeletePakuStrategyTestError2.txt", "test2"));

            List<VirtualFileInfo> virtualFiles = files.Select(x => new VirtualFileInfo(x)).ToList();

            // insert a non-existent file in the middle
            virtualFiles.Insert(1, new VirtualFileInfo(@"C:\DeletePakuStrategyTestError.notreal"));

            IPakuStrategy strategy = new DeletePakuStrategy();
            PakuResult result = strategy.Eat(dir, virtualFiles, null);
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
            DirectoryInfo dir = new DirectoryInfo(Directory.GetCurrentDirectory());

            List<FileInfo> files = new List<FileInfo>();
            files.Add(CreateTestFile("ZipPakuStrategyTest1.txt", "test1"));
            files.Add(CreateTestFile("ZipPakuStrategyTest2.txt", "test2"));

            List<VirtualFileInfo> virtualFiles = files.Select(x => new VirtualFileInfo(x)).ToList();

            IPakuStrategy strategy = new ZipPakuStrategy();
            PakuResult result = strategy.Eat(dir, virtualFiles, "foo");

            // all of the files should have been deleted successfully
            Assert.IsTrue(result.Success);
            Assert.AreEqual(virtualFiles.Count, result.RemovedFiles.Count);

            // and a zip file should have been created
            Assert.AreEqual(1, result.CreatedFiles.Count);
            FileInfo zipFile = result.CreatedFiles[0].ToFileInfo();
            Assert.IsTrue(zipFile.Exists);
            Assert.AreEqual("foo", zipFile.Name.Substring(0, 3));

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
            DirectoryInfo dir = new DirectoryInfo(Directory.GetCurrentDirectory());

            List<FileInfo> files = new List<FileInfo>();
            files.Add(CreateTestFile("ZipPakuStrategyTest1.txt", "test1"));
            files.Add(CreateTestFile("ZipPakuStrategyTest2.txt", "test2"));

            List<VirtualFileInfo> virtualFiles = files.Select(x => new VirtualFileInfo(x)).ToList();

            // insert a non-existent file in the middle
            virtualFiles.Insert(1, new VirtualFileInfo(@"C:\ZipPakuStrategyTestError.notreal"));

            IPakuStrategy strategy = new ZipPakuStrategy();
            PakuResult result = strategy.Eat(dir, virtualFiles, null);

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

        [TestMethod]
        public void PgpPakuStrategyTest()
        {
            // create some files
            DirectoryInfo dir = new DirectoryInfo(Directory.GetCurrentDirectory());

            List<FileInfo> files = new List<FileInfo>();
            files.Add(CreateTestFile("PgpPakuStrategyTest1.txt", "test1"));
            files.Add(CreateTestFile("PgpPakuStrategyTest2.txt", "test2"));

            List<VirtualFileInfo> virtualFiles = files.Select(x => new VirtualFileInfo(x)).ToList();

            IPakuStrategy strategy = new PgpPakuStrategy();
            PakuResult result = strategy.Eat(dir, virtualFiles, @"Props\UnitTestPublicKey.asc|foo");

            // all of the files should have been deleted successfully
            Assert.IsTrue(result.Success);
            Assert.AreEqual(virtualFiles.Count, result.RemovedFiles.Count);

            foreach (VirtualFileInfo vfi in virtualFiles)
            {
                FileInfo fi = vfi.ToFileInfo();
                Assert.IsFalse(fi.Exists);
            }

            // and one file should have been created
            Assert.AreEqual(1, result.CreatedFiles.Count);
            Assert.AreEqual("foo", result.CreatedFiles[0].Name.Substring(0, 3));
        }

        [TestMethod]
        public void AzureBlobPakuStrategyUploadTest()
        {
            AzureBlobPakuStrategy strategy = new AzureBlobPakuStrategy();
            AzurePakuConfig config = JsonConvert.DeserializeObject<AzurePakuConfig>(File.ReadAllText(@"Props\azure.json"));

            File.WriteAllText("AzureBlobTest.txt", "test file");
            VirtualFileInfo fi = new VirtualFileInfo(new FileInfo("AzureBlobTest.txt"));
            List<VirtualFileInfo> files = new List<VirtualFileInfo>() { fi };

            strategy.Upload(config.ConnectionString, config.Container, files);
        }

        [TestMethod]
        public void AzureBlobPakuStrategyTest()
        {
            // create test files
            List<VirtualFileInfo> files = new List<VirtualFileInfo>();
            for (int i = 0; i < 3; i++)
            {
                string fname = $"AzureBlobTest_{i}.txt";
                File.WriteAllText(fname, $"test file {i}");
                VirtualFileInfo fi = new VirtualFileInfo(new FileInfo(fname));
                files.Add(fi);
            }

            AzureBlobPakuStrategy strategy = new AzureBlobPakuStrategy();
            PakuResult result = strategy.Eat(new DirectoryInfo(Directory.GetCurrentDirectory()), files, @"Props\azure.json");

            Assert.IsTrue(result.Success);
            Assert.AreEqual(3, result.RemovedFiles.Count);
        }
    }
}
