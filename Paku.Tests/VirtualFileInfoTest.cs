using Microsoft.VisualStudio.TestTools.UnitTesting;
using Paku.Models;
using System;
using System.IO;

namespace Paku.Tests
{
    [TestClass]
    public class VirtualFileInfoTest
    {
        [TestMethod]
        public void CreateFromString()
        {
            // confirm that full name parts
            // were parsed and set correctly
            string fullName = @"C:\foo\bar.txt";
            VirtualFileInfo fi = new VirtualFileInfo(fullName);

            Assert.AreEqual(fullName, fi.FullName);
            Assert.AreEqual("bar.txt", fi.Name);
            Assert.AreEqual(@"C:\foo", fi.DirectoryName);

            // invalid full names should throw an exception
            ArgumentException exception = null;
            try
            {
                fullName = @"bar.txt";
                fi = new VirtualFileInfo(fullName);
            }
            catch (ArgumentException ex)
            {
                exception = ex;
            }

            Assert.IsNotNull(exception);
        }

        [TestMethod]
        public void CreateFromFileInfo()
        {
            FileInfo fi = new FileInfo(@"Props\foo.txt");
            VirtualFileInfo vfi = new VirtualFileInfo(fi);

            Assert.AreEqual(fi.FullName, vfi.FullName);
            Assert.AreEqual(fi.Name, vfi.Name);
            Assert.AreEqual(fi.DirectoryName, vfi.DirectoryName);
            Assert.AreEqual(fi.Length, vfi.Length);
            Assert.AreEqual(fi.CreationTime, vfi.CreationTime);
            Assert.AreEqual(fi.LastWriteTime, vfi.LastWriteTime);
        }
    }
}
