using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace Paku.Models
{
    /// <summary>
    /// # DeletePakuStrategy
    /// 
    /// Eats files by deleting them. Yum!
    /// </summary>
    [CommandAlias("delete")]
    [Description("Deletes the file(s).")]
    public class DeletePakuStrategy : IPakuStrategy
    {
        /// <summary>
        /// ## Eat
        /// 
        /// Deletes the specified files.
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public PakuResult Eat(DirectoryInfo dir, IList<VirtualFileInfo> files, string parameters)
        {
            // attempt to delete the files, tracking which ones we could delete
            PakuResult result = new PakuResult();

            foreach (VirtualFileInfo vfi in files)
            {
                FileInfo fi = vfi.ToFileInfo();

                if (fi.Exists)
                {
                    try
                    {
                        fi.Delete();
                        result.RemovedFiles.Add(vfi);
                    }
                    catch (Exception ex)
                    {
                        result.Error = ex;
                        break;
                    }
                }
                else
                {
                    result.Error = new ArgumentException($"File {vfi.FullName} does not exist.");
                    break;
                }
            }

            return result;
        }
    }
}
