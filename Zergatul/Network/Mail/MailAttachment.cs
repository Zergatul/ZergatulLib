using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Mail
{
    public class MailAttachment
    {
        public string Filename { get; set; }
        public byte[] Content { get; set; }
        public string ContentType { get; set; }
        public Encoding TextEncoding { get; set; } = Encoding.UTF8;
    }
}