using System;
using System.IO;
using Mono.Options;
using Paku.Models;

namespace Paku
{
    // TODO:
    //
    // ~~1. Logging (NLog)~~
    // 2. Filter: Cap to top N
    // 3. Paku: PGP encrypt then delete 
    // 4. Paku: Upload to Azure blob storage 
    class Program
    {
        static void Main(string[] args)
        {
            PakuArguments arguments = new PakuArguments();
            bool parseSuccess = arguments.Parse(args);

            if (arguments.Directory == null)
            {
                arguments.Directory = Directory.GetCurrentDirectory();
            }

            if (parseSuccess)
            {
                Console.WriteLine($"Removing files from: {arguments.Directory}");
                Console.WriteLine();

                Pipeline pipeline = new Pipeline(arguments.SelectionStrategy.Item1, arguments.FilterStrategy.Item1, arguments.PakuStrategy.Item1);
                pipeline.Execute(arguments.Directory, arguments.SelectionStrategy.Item2, arguments.FilterStrategy.Item2, arguments.LoggingEnabled);
            }
        }
    }
}
