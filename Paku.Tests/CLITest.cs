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

            // 5 main options plus a help menu
            Assert.AreEqual(6, options.Count);

            // attempt to parse arguments and ensure that options were set correctly
            args = new PakuArguments();
            bool success = args.Parse(new string[] { @"--dir=C:\foo", "--select:regex=foo", "--filter:age=cdate>30m", "--paku:delete=bar", "--log" });

            Assert.IsTrue(success);
            Assert.IsNotNull(args.Directory);
            Assert.IsNotNull(args.SelectionStrategy);
            Assert.IsNotNull(args.FilterStrategy);
            Assert.IsNotNull(args.PakuStrategy);
            Assert.IsTrue(args.LoggingEnabled);

            // missing arguments
            args = new PakuArguments();
            success = args.Parse(new string[] { "--selection:regex=foo" });
            Assert.IsFalse(success);
        }
    }
}
