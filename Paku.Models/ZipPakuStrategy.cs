using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Paku.Models
{
    /// <summary>
    /// # ZipPakuStrategy
    /// 
    /// Eats files by adding them to a zip archive and then deleting them.
    /// </summary>
    [CommandAlias("zip")]
    [Description("Zips up the file(s) and then deletes them.")]
    public class ZipPakuStrategy : IPakuStrategy
    {
        public string ZipFilePrefix { get; set; }

        /// <summary>
        /// ## Eat
        /// 
        /// Eats the specified list of files by adding them to a zip archive and then deleting them.
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public PakuResult Eat(DirectoryInfo dir, IList<VirtualFileInfo> files, string parameters)
        {
            // attempt to delete the files, tracking which ones we could delete
            PakuResult result = new PakuResult();
            string zipName = Path.Combine(dir.FullName, $"{ZipFilePrefix}_{DateTime.Now.ToString("yyyyMMdd_HH-mm-ss-fff")}.zip");

            using (var stream = new FileStream(zipName, FileMode.Create))
            {
                using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, true))
                {
                    // add files
                    foreach (VirtualFileInfo vfi in files)
                    {
                        FileInfo fi = vfi.ToFileInfo();

                        if (fi.Exists)
                        {
                            try
                            {
                                // add to archive and remove from disk
                                archive.CreateEntryFromFile(fi.FullName, fi.Name, CompressionLevel.Optimal);
                                fi.Delete();
                                result.RemovedFiles.Add(vfi);
                            }
                            catch (Exception ex)
                            {
                                // stop execution, but do not throw exception yet
                                // because we do not want to corrupt the zip file
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
                }

                stream.Close();
            }

            // add zip file to result
            result.CreatedFiles.Add(new VirtualFileInfo(new FileInfo(zipName)));

            return result;
        }

        public ZipPakuStrategy(string zipPrefix)
        {
            this.ZipFilePrefix = zipPrefix;
        }

        public ZipPakuStrategy()
        {
            this.ZipFilePrefix = "paku";
        }
    }
}
