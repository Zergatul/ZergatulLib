using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Hash;

namespace Zergatul.Cryptography.Tests.Hash
{
    static class Helper
    {
        public static string Hash<T>(byte[] data) where T : AbstractHash, new()
        {
            var hash = new T();
            hash.Update(data);
            return BitHelper.BytesToHex(hash.ComputeHash());
        }

        public static string Hash<T>(string input) where T : AbstractHash, new()
        {
            return Hash<T>(Encoding.ASCII.GetBytes(input));
        }
    }
}
