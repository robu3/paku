using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using Org.BouncyCastle;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Security;
using System.ComponentModel;

namespace Paku.Models
{
    [CommandAlias("pgp")]
    [Description("Adds the file(s) to a PGP-encrypted zip archive. Parameters: public_key.asc|prefix (optional).")]
    public class PgpPakuStrategy : IPakuStrategy
    {
        public PakuResult Eat(DirectoryInfo dir, IList<VirtualFileInfo> files, string parameters)
        {
            PakuResult result = new PakuResult();

            try
            {
                PgpPakuStrategyParams parms = new PgpPakuStrategyParams(parameters);

                // zip all filtered files before encryption
                ZipPakuStrategy zipper = new ZipPakuStrategy();
                PakuResult zipResult = zipper.Eat(dir, files, parms.OutputFilePrefix);
                
                if (zipResult.Success)
                {
                    VirtualFileInfo zipFile = zipResult.CreatedFiles[0];

                    // then encrypt
                    VirtualFileInfo pgpFile = Encrypt(zipFile, zipFile.FullName + ".pgp", parms.PgpPublicKeyPath);

                    // and remove the temporary zip file generated
                    File.Delete(zipFile.FullName);

                    result.CreatedFiles = new List<VirtualFileInfo>() { pgpFile };
                    result.RemovedFiles = zipResult.RemovedFiles;
                }
                else
                {
                    result.Error = zipResult.Error;
                }
            }
            catch (Exception ex)
            {
                result.Error = ex;
            }

            return result;
        }

        /// <summary>
        /// ## LoadPublicKey
        /// 
        /// Loads a PGP public key from disk.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public PgpPublicKey LoadPublicKey(string path)
        {
            using (Stream fs = File.OpenRead(path))
            {
                using (Stream decoderStream = PgpUtilities.GetDecoderStream(fs))
                {
                    PgpPublicKeyRing keyRing = new PgpPublicKeyRing(decoderStream);
                    decoderStream.Close();
                    fs.Close();

                    return keyRing.GetPublicKey();
                }
            }
        }

        /// <summary>
        /// ## Encrypt
        /// 
        /// Encrypts the specified file using the public key provided.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="outputFilePath"></param>
        /// <param name="publicKeyPath"></param>
        public VirtualFileInfo Encrypt(VirtualFileInfo file, string outputFilePath, string publicKeyPath)
        {
            // load public key and encrypt the file
            PgpEncryptedDataGenerator encryption = new PgpEncryptedDataGenerator(SymmetricKeyAlgorithmTag.Aes256, true, new SecureRandom());
            PgpPublicKey publicKey = LoadPublicKey(publicKeyPath);
            encryption.AddMethod(publicKey);

            using (Stream fsOut = File.Create(outputFilePath))
            {
                using (MemoryStream bOut = new MemoryStream())
                {
                    PgpUtilities.WriteFileToLiteralData(bOut, PgpLiteralData.Binary, new FileInfo(file.FullName));
                    byte[] data = bOut.ToArray();

                    using (Stream encryptOut = encryption.Open(fsOut, data.Length))
                    {
                        encryptOut.Write(data, 0, data.Length);
                        encryptOut.Close();
                    }

                    bOut.Close();
                }
                fsOut.Close();
            }

            encryption.Close();
            return new VirtualFileInfo(outputFilePath);
        }
    }
}
