using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Ftp
{
    public interface IFtpFile
    {
        Stream GetStream();
        long GetSize();
        DateTime GetModifiedDate();
    }
}