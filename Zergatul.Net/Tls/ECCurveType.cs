using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Net.Tls
{
    internal enum ECCurveType : byte
    {
        ExplicitPrime = 1,
        ExplicitChar2 = 2,
        NamedCurve = 3
    }
}
