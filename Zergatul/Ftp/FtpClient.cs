using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Ftp
{
    public class FtpClient : IDisposable
    {
        protected FtpConnection _ftpConnection;

        public FtpClient()
        {
        }

        public void Connect(string hostname, int port = 21)
        {
        }

        public void LoginAsAnonymous()
        {
            Login("anonymous", "anonymous@anonymous.com");
        }

        public void Login(string user, string password, string account = null)
        {
        }

        #region Dispose pattern

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_ftpConnection != null)
                    _ftpConnection.Dispose();
            }
        }

        #endregion
    }
}