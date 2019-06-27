using Microsoft.VisualStudio.TestTools.UnitTesting;
using Paku.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Paku.Tests
{
    [TestClass]
    public class IFilterStrategyTest
    {
        [TestMethod]
        public void AgeFilterStrategyParamsTest()
        {
            AgeFilterStrategyParams parms = new AgeFilterStrategyParams("cdate>8h");

            Assert.AreEqual(FileDateProperties.CreateDate, parms.FileDate);
            Assert.AreEqual(Operators.GreaterThan, parms.Operator);
            Assert.AreEqual(8, parms.UnitValue);
            Assert.AreEqual(TimeUnits.Hour, parms.TimeUnit);

            // should still work with mixed casing and a single space around the operator
            parms = new AgeFilterStrategyParams("MDate <= 90m");

            Assert.AreEqual(FileDateProperties.ModifiedDate, parms.FileDate);
            Assert.AreEqual(Operators.LessThanEquals, parms.Operator);
            Assert.AreEqual(90, parms.UnitValue);
            Assert.AreEqual(TimeUnits.Minute, parms.TimeUnit);
        }

        [TestMethod]
        public void AgeFilterStrategyTest()
        {
            List<VirtualFileInfo> files = new List<VirtualFileInfo>()
            {
                new VirtualFileInfo(@"C:\foo\bar.txt") { CreationTime = DateTime.Now, LastWriteTime = DateTime.Now },
                new VirtualFileInfo(@"C:\foo\baz.txt") { CreationTime = DateTime.Now.AddDays(-1), LastWriteTime = DateTime.Now },
                new VirtualFileInfo(@"C:\foo\buz.txt") { CreationTime = DateTime.Now.AddHours(-12).AddMilliseconds(-1), LastWriteTime = DateTime.Now }
            };

            AgeFilterStrategy filter = new AgeFilterStrategy();
            IList<VirtualFileInfo> results = filter.Filter(files, "cdate > 12h");

            // should match two older files based on create date
            Assert.AreEqual(2, results.Count);
            Assert.IsTrue(results.Contains(files[1]));
            Assert.IsTrue(results.Contains(files[2]));

            // should match all three using date last modified
            results = filter.Filter(files, "mdate<=10s");
            Assert.AreEqual(3, results.Count);
        }

        [TestMethod]
        public void AgeFilterStrategyTestRobust()
        {
            List<VirtualFileInfo> files = new List<VirtualFileInfo>()
            {
                new VirtualFileInfo(@"C:\foo\really_old.txt") { CreationTime = DateTime.Now.AddYears(-101), LastWriteTime = DateTime.Now.AddYears(-101) },
                new VirtualFileInfo(@"C:\foo\in_the_future.txt") { CreationTime = DateTime.Now.AddDays(3), LastWriteTime = DateTime.Now.AddDays(3) }
            };

            AgeFilterStrategy filter = new AgeFilterStrategy();

            // over 100 years old
            IList<VirtualFileInfo> results = filter.Filter(files, "cdate>36500d");

            Assert.AreEqual(1, results.Count);
            Assert.IsTrue(results.Contains(files[0]));

            // future-dated file: last modified by a DeLorean-riding scientist?
            results = filter.Filter(files, "mdate <= 3d");
            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        public void AllFilterStrategyTest()
        {
            List<VirtualFileInfo> files = new List<VirtualFileInfo>()
            {
                new VirtualFileInfo(@"C:\foo\bar.txt") { CreationTime = DateTime.Now, LastWriteTime = DateTime.Now },
                new VirtualFileInfo(@"C:\foo\baz.txt") { CreationTime = DateTime.Now.AddDays(-1), LastWriteTime = DateTime.Now },
                new VirtualFileInfo(@"C:\foo\buz.txt") { CreationTime = DateTime.Now.AddHours(-12).AddMilliseconds(-1), LastWriteTime = DateTime.Now }
            };

            AllFilterStrategy filter = new AllFilterStrategy();
            IList<VirtualFileInfo> results = filter.Filter(files, "");

            // all files should be returned
            Assert.AreEqual(files.Count, results.Count);

            for (int i = 0; i < files.Count; i++)
            {
                Assert.AreEqual(files[i], results[i]);
            }
        }

        [TestMethod]
        public void CapFilterStrategyTest()
        {
            List<VirtualFileInfo> files = new List<VirtualFileInfo>()
            {
                new VirtualFileInfo(@"C:\foo\abc.txt") { CreationTime = DateTime.Now, LastWriteTime = DateTime.Now },
                new VirtualFileInfo(@"C:\foo\bar.txt") { CreationTime = DateTime.Now.AddMinutes(-1), LastWriteTime = DateTime.Now },
                new VirtualFileInfo(@"C:\foo\baz.txt") { CreationTime = DateTime.Now.AddDays(-1), LastWriteTime = DateTime.Now },
                new VirtualFileInfo(@"C:\foo\buz.txt") { CreationTime = DateTime.Now.AddHours(-12).AddMilliseconds(-1), LastWriteTime = DateTime.Now }
            };

            CapFilterStategy filter = new CapFilterStategy();
            IList<VirtualFileInfo> results = filter.Filter(files, "2 name desc");

            // files should be sorted in reversed name order
            // returning the first two files by alpha sort
            Assert.AreEqual(2, results.Count);
            Assert.AreEqual(files[1], results[0]);
            Assert.AreEqual(files[0], results[1]);

            // repeat with file dates
            results = filter.Filter(files, "1 cdate");
            Assert.AreEqual(3, results.Count);
            Assert.AreEqual(files[3], results[0]);
        }
    }
}
