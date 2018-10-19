using System.IO;

namespace Zergatul.Network.Ftp
{
    public interface IFtpFileSystemProvider
    {
        string GetCurrentDirectory();
        IFtpFile GetFile(string filename);
    }
}