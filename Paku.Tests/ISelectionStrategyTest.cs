using Microsoft.VisualStudio.TestTools.UnitTesting;
using Paku.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace Paku.Tests
{
    [TestClass]
    public class ISelectionStrategyTest
    {
        [TestMethod]
        public void RegexSelectionStrategyTest()
        {
            ISelectionStrategy selector = new RegexSelectionStrategy();
            DirectoryInfo di = new DirectoryInfo(@"Props");

            IList<VirtualFileInfo> selected = selector.Select(di, @"\.txt$");
            Assert.AreEqual(2, selected.Count);
            Assert.AreEqual(selected.Count, selected.Count(x => x.Name.EndsWith(".txt")));
        }

        [TestMethod]
        public void PatternSelectionStrategyTest()
        {
            ISelectionStrategy selector = new PatternSelectionStrategy();
            DirectoryInfo di = new DirectoryInfo(@"Props");

            IList<VirtualFileInfo> selected = selector.Select(di, "*.txt");
            Assert.AreEqual(2, selected.Count);
            Assert.AreEqual(selected.Count, selected.Count(x => x.Name.EndsWith(".txt")));
        }
    }
}
