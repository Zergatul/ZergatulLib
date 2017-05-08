using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zergatul.Ftp;
using Zergatul.Math;
using Zergatul.Net.Proxy;
using Zergatul.Net.Tls;

namespace Test
{
    class Program
    {
        // TODO: test with slow connection
        // TODO: check ipv6
        // http proxy parse response
        // implicit/exp ftps
        // exception texts to resources
        // https://www.chilkatsoft.com/ftp-2-dotnet.asp
        // http://xceed.com/FTP_NET_Features.html
        // https://www.rebex.net/ftp-ssl.net/
        // http://www.dart.com/ftp-control-features.aspx

        static void Main(string[] args)
        {
            /*var tcp = new TcpClient("localhost", 2828);
            TlsStream stream = new TlsStream(tcp.GetStream());
            stream.AuthenticateAsClient("localhost");*/
            BigInteger bi = new BigInteger(new byte[] { 1, 0, 0, 0, 0, 0, 0, 0, 0 }, ByteOrder.BigEndian);
            Console.WriteLine(bi.ToString());
            Console.ReadLine();
        }

        static void tt()
        {
            /*var lines = plist.Split(new[] { Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 25; i < lines.Length; i++)
            {
                var line = lines[i];
                Console.WriteLine(line);
                var ss = line.Split(':');
                var ip = ss[0];
                var port = int.Parse(ss[1]);

                Exception ex = null;
                var t = new Thread(() =>
                    {
                        try
                        {
                            var p1 = new Socks5(IPAddress.Parse(ip), port);
                            //var p1 = new HttpProxy(IPAddress.Parse("175.182.229.2"), 80);
                            var tcp = p1.CreateConnection(IPAddress.Parse("1111"), 80);
                            var bytes = Encoding.ASCII.GetBytes("GET / HTTP/1.0" + Environment.NewLine + "Host: qwe" + Environment.NewLine + Environment.NewLine);
                            tcp.GetStream().Write(bytes, 0, bytes.Length);
                            var response = new StreamReader(tcp.GetStream()).ReadToEnd();
                            if (response.Contains("508 Loop Detected"))
                                Console.WriteLine("Blacklisted");
                            else
                                Console.WriteLine(response);
                            tcp.Close();
                        }
                        catch (Exception e)
                        {
                            ex = e;
                        }
                    });
                t.Start();
                if (t.Join(10000))
                {
                    if (ex != null)
                        Console.WriteLine("error");
                }
                else
                {
                    t.Abort();
                    Console.WriteLine("timeout");
                }

                Console.WriteLine();
            }*/
        }
    }
}