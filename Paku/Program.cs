using System;
using System.IO;
using Mono.Options;
using Paku.Models;

namespace Paku
{
    // TODO:
    //
    // 1. Add argument to specify directory
    // 2. Logging?
    // 3. Add "preview" paku strategy to test prior to execution
    class Program
    {
        static void Main(string[] args)
        {
            PakuArguments arguments = new PakuArguments();
            bool parseSuccess = arguments.Parse(args);

            if (parseSuccess)
            {
                Pipeline pipeline = new Pipeline(arguments.SelectionStrategy.Item1, arguments.FilterStrategy.Item1, arguments.PakuStrategy.Item1);
                pipeline.Execute(Directory.GetCurrentDirectory(), arguments.SelectionStrategy.Item2, arguments.FilterStrategy.Item2);
            }
        }
    }
}
