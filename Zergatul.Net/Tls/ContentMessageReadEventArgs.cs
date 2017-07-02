using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Net.Tls
{
    internal class ContentMessageEventArgs : EventArgs
    {
        public ContentMessage Message { get; private set; }
        public bool WasRead { get; private set; }
        public bool WasWritten { get; private set; }
        public bool FromServer { get; private set; }
        public bool FromClient { get; private set; }

        public ContentMessageEventArgs(ContentMessage message, bool read, bool fromServer)
        {
            this.Message = message;
            this.WasRead = read;
            this.WasWritten = !read;
            this.FromServer = fromServer;
            this.FromClient = !fromServer;
        }
    }
}
