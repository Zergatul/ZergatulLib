using System.IO;

namespace Zergatul.Network.Ftp
{
    public interface IFtpFileSystemProvider
    {
        string GetCurrentDirectory();
        Stream GetFileStream(string filename);
    }
}