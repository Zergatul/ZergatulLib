using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network.Tls;

namespace ConsoleTest2
{
    class Program
    {
        static void Main(string[] args)
        {
            var cert = new X509Certificate2("test.p12", "hh87$-Jqo");
            RunServer(useDotNetTls: false);
            //RunClient();
        }

        static void RunServer(bool useDotNetTls)
        {
            ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => true;

            var listener = new TcpListener(IPAddress.Any, 32028);
            listener.Start();
            var cert = new X509Certificate2("test.p12", "hh87$-Jqo");
            while (true)
            {
                var client = listener.AcceptTcpClient();

                SslStream sslStream = null;
                TlsStream tlsStream = null;
                if (useDotNetTls)
                    sslStream = new SslStream(client.GetStream());
                else
                    tlsStream = new TlsStream(client.GetStream());

                try
                {
                    if (useDotNetTls)
                    {
                        sslStream.AuthenticateAsServer(cert, false, System.Security.Authentication.SslProtocols.Tls12, false);
                        sslStream.Write(Encoding.ASCII.GetBytes("Hello World!"));
                        sslStream.Close();
                    }
                    else
                    {
                        tlsStream.Settings = new TlsStreamSettings
                        {
                            CipherSuites = new CipherSuite[]
                            {
                                CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA,
                                CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA256,
                                CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA,
                                CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA384,
                            }
                        };
                        tlsStream.AuthenticateAsServer("localhost", new Zergatul.Cryptography.Certificate.X509Certificate(cert.RawData));
                        tlsStream.Write(Encoding.ASCII.GetBytes("Hello World!"));
                    }
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

        static void RunClient()
        {
            var client = new TcpClient("localhost", 32028);
            var sslStream = new SslStream(client.GetStream(), false, (a, b, c, d) => true);
            sslStream.AuthenticateAsClient("localhost");

            byte[] buffer = new byte[5];
            sslStream.Read(buffer, 0, 5);

            Console.WriteLine(new string('*', 32));
            Console.WriteLine(Encoding.ASCII.GetString(buffer));
            Console.WriteLine(new string('*', 32));

            Console.ReadLine();
        }
    }
}
