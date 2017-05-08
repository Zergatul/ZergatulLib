using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Ftp
{
    public class FtpFileInfo
    {
        public string Name { get; set; }
        public long Length { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsDirectory { get; set; }
    }
}