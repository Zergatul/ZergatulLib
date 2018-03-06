using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Ftp
{
    public interface IFtpListParser
    {
        FtpFileInfo[] Parse(string response);
    }
}