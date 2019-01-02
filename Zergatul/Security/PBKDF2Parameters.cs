using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Security
{
    public class PBKDF2Parameters : KDFParameters
    {
        public string MessageDigest;
        public int Iterations;
    }
}