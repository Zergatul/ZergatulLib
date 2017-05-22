using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTest2
{
    class Program
    {
        static void Main(string[] args)
        {
            var listener = new TcpListener(IPAddress.Any, 32028);
            listener.Start();
            var cert = new X509Certificate2("test.p12", "hh87$-Jqo");
            while (true)
            {
                var client = listener.AcceptTcpClient();
                SslStream stream = new SslStream(client.GetStream());
                try
                {
                    stream.AuthenticateAsServer(cert, false, System.Security.Authentication.SslProtocols.Tls12, false);
                    stream.Write(Encoding.ASCII.GetBytes("Hello World!"));
                    stream.Close();
                    client.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    if (ex.InnerException != null)
                        Console.WriteLine("> " + ex.InnerException.Message);
                }
            }
        }
    }
}
