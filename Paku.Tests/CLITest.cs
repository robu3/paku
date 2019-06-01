using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Paku.Models;
using Mono.Options;

namespace Paku.Tests
{
    [TestClass]
    public class CLITest
    {
        [TestMethod]
        public void ParseArguments()
        {
            PakuArguments args = new PakuArguments();
            OptionSet options = args.BuildOptionSet();

            Assert.IsNotNull(options);

            // 3 main options plus a help menu
            Assert.AreEqual(4, options.Count);

            // attempt to parse arguments and ensure that options were set correctly
            args = new PakuArguments();
            bool success = args.Parse(new string[] { "--select:regex=foo", "--filter:age=cdate>30m", "--paku:delete" });

            Assert.IsTrue(success);
            Assert.IsNotNull(args.SelectionStrategy);
            Assert.IsNotNull(args.FilterStrategy);
            Assert.IsNotNull(args.PakuStrategy);

            // missing arguments
            args = new PakuArguments();
            success = args.Parse(new string[] { "--selection:regex=foo" });
            Assert.IsFalse(success);
        }
    }
}
