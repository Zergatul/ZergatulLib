using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network
{
    public interface ITlsProvider
    {
        Stream AuthenticateAsClient(Stream innerStream, string host);
    }
}