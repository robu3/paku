using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace Paku.Models
{
    /// <summary>
    /// # PreviewPakuStrategy
    /// 
    /// Simply writes the full names of the files to be deleted to the console.
    /// Useful for testing selection and filtering strategies prior to removing files (^_^)
    /// </summary>
    [CommandAlias("preview")]
    [Description("Writes the names of the files to be removed to the console (useful for testing).")]
    public class PreviewPakuStrategy : IPakuStrategy
    {
        public PakuResult Eat(DirectoryInfo dir, IList<VirtualFileInfo> files, string parameters)
        {
            foreach (VirtualFileInfo fi in files)
            {
                Console.WriteLine(fi.FullName);
            }

            return new PakuResult();
        }
    }
}
