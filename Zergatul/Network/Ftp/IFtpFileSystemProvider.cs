using System.IO;

namespace Zergatul.Network.Ftp
{
    public interface IFtpFileSystemProvider
    {
        string GetCurrentDirectory();
        bool SetWorkingDirectory(string path);
        IFtpFile GetFile(string filename);
    }
}