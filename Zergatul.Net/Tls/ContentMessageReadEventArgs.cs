using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Net.Tls
{
    internal class ContentMessageReadEventArgs : EventArgs
    {
        public ContentMessage Message { get; private set; }

        public ContentMessageReadEventArgs(ContentMessage message)
        {
            this.Message = message;
        }
    }
}
