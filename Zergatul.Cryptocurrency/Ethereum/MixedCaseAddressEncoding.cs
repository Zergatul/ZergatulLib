using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Zergatul.Cryptography.Hash;

namespace Zergatul.Cryptocurrency.Ethereum
{
    // https://github.com/ethereum/EIPs/blob/master/EIPS/eip-55.md
    public static class MixedCaseAddressEncoding
    {
        private static Regex _rawAddr = new Regex("^0x[0-9a-f]{40}$");
        private static Regex _encodedAddr = new Regex("^0x[0-9a-fA-F]{40}$");

        public static string Encode(string value)
        {
            if (value == null)
                throw new ArgumentNullException();
            if (!_rawAddr.IsMatch(value))
                throw new ArgumentException();

            var keccak = new Keccak256();
            keccak.Update(Encoding.ASCII.GetBytes(value.Substring(2)));
            byte[] hash = keccak.ComputeHash();

            var sb = new StringBuilder(42);
            sb.Append("0x");
            for (int i = 0; i < 40; i++)
                if (GetHexAtPos(hash, i) >= 8)
                    sb.Append(char.ToUpper(value[i + 2]));
                else
                    sb.Append(value[i + 2]);

            return sb.ToString();
        }

        public static bool Validate(string value)
        {
            if (value == null)
                throw new ArgumentNullException();
            if (!_encodedAddr.IsMatch(value))
                throw new ArgumentException();

            var keccak = new Keccak256();
            keccak.Update(Encoding.ASCII.GetBytes(value.ToLower().Substring(2)));
            byte[] hash = keccak.ComputeHash();

            for (int i = 0; i < 40; i++)
                if (char.IsLetter(value[i + 2]))
                {
                    if (GetHexAtPos(hash, i) >= 8)
                    {
                        if (char.IsLower(value[i + 2]))
                            return false;
                    }
                    else
                    {
                        if (char.IsUpper(value[i + 2]))
                            return false;
                    }
                }

            return true;
        }

        private static int GetHexAtPos(byte[] data, int index)
        {
            if (index % 2 == 0)
                return (data[index / 2] & 0xF0) >> 4;
            else
                return data[index / 2] & 0x0F;
        }
    }
}