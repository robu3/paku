using Microsoft.VisualStudio.TestTools.UnitTesting;
using Paku.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Paku.Tests
{
    [TestClass]
    public class PipelineTest
    {
        [TestMethod]
        public void ConstructFromStrings()
        {
            Pipeline p = new Pipeline("PatternSelectionStrategy", "AgeFilterStrategy", "DeletePakuStrategy");

            Assert.IsTrue(p.SelectionStrategy is PatternSelectionStrategy);
            Assert.IsTrue(p.FilterStrategy is AgeFilterStrategy);
            Assert.IsTrue(p.PakuStrategy is DeletePakuStrategy);
        }

        [TestMethod]
        public void GetStrategyImplementations()
        {
            List<TypeInfo> implementations = Pipeline.GetStrategyImplementations<ISelectionStrategy>();
            Assert.AreEqual(2, implementations.Count);
            Assert.IsTrue(implementations.All(x => x.Name.Contains("Selection")));
        }

        [TestMethod]
        public void GetStrategyAliasMap()
        {
            Dictionary<string, string> map = Pipeline.GetStrategyAliasMap<ISelectionStrategy>();
            List<TypeInfo> implementations = Pipeline.GetStrategyImplementations<ISelectionStrategy>();

            // we should always have at least as many aliases as strategy implementations
            Assert.IsTrue(map.Keys.Count >= implementations.Count);
        }

        [TestMethod]
        public void Execute()
        {
            // create dummy files to run our pipeline on
            DirectoryInfo dir = new DirectoryInfo("Props/PipelineTest");

            if (dir.Exists)
            {
                dir.Delete(true);
            }

            dir.Create();

            File.WriteAllText(Path.Combine(dir.FullName, "katsu.txt"), "katsu");
            File.WriteAllText(Path.Combine(dir.FullName, "tonkatsu.txt"), "tonkatsu");
            File.WriteAllText(Path.Combine(dir.FullName, "katsukare.txt"), "katsu");
            File.WriteAllText(Path.Combine(dir.FullName, "kare.txt"), "katsu");

            // execute pipeline
            Pipeline pipeline = new Pipeline("RegexSelectionStrategy", "AgeFilterStrategy", "DeletePakuStrategy");
            pipeline.Execute(dir.FullName, "katsu.txt$", "cdate < 1m", "");

            // confirm that files were deleted
            FileInfo[] files = dir.GetFiles();
            Assert.AreEqual(2, files.Length);
            Assert.IsTrue(files.Any(x => x.Name == "katsukare.txt"));
            Assert.IsTrue(files.Any(x => x.Name == "kare.txt"));
        }
    }
}
