using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace SRTPluginProviderTEW
{
    /// <summary>
    /// SHA256 hashes for the TEW game executables.
    /// </summary>
    public static class GameHashes
    {
        private static readonly byte[] EvilWithinWW_20220105_1 = new byte[32] { 0x74, 0x3D, 0xF7, 0xA1, 0x26, 0xA6, 0x64, 0x70, 0xDB, 0xFC, 0x27, 0xB8, 0xC3, 0x03, 0x48, 0xEE, 0xA7, 0xD2, 0x9B, 0xF9, 0x48, 0xB8, 0x27, 0x05, 0xDA, 0x6E, 0x7B, 0xAB, 0xC6, 0xE6, 0x74, 0x37 };

        public static GameVersion DetectVersion(string filePath)
        {
            byte[] checksum;
            using (SHA256 hashFunc = SHA256.Create())
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
                checksum = hashFunc.ComputeHash(fs);

            if (checksum.SequenceEqual(EvilWithinWW_20220105_1))
            {
                Console.WriteLine("Latest Release");
                return GameVersion.EvilWithinWW_20220105_1;
            }

            Console.WriteLine("Unknown Version");
            return GameVersion.Unknown;
        }
    }
}