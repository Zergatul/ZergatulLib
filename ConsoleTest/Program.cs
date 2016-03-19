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
using Zergatul.Net.Proxy;

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

        static void tmp()
        {
            var ftp = new FtpConnection();
            ftp.CertificateValidationCallback = delegate { return true; };
            ftp.Log = Console.Out;
            ftp.Connect("", 21);
            ftp.AuthTls();
            ftp.Login("", "");

            ftp.SetTransferMode(FtpTransferMode.Stream);
            ftp.SetRepresentationType(FtpRepresentation.Type.ASCII, FtpRepresentation.Param.NonPrint);

            /*ftp.ProtectionBufferSize(0);
            ftp.DataChannelProtectionLevel(FtpDataChannelProtectionLevel.Private);

            ftp.EnterPassiveModeEx();
            string list = ftp.List("");*/

            ftp.NameList();

            ftp.Quit();
        }

        static void CheckMailRu()
        {
            //var p1 = new Socks5(IPAddress.Parse("127.0.0.1"), 1080);
            //var p2 = new Socks5(IPAddress.Parse("46.165.223.90"), 15745);
            var p3 = new Socks5("5.35.105.211", 27888);
            //var chain = new ChainProxy(p1, p2);

            //var tcp = p3.CreateConnection(new IPAddress(new byte[] { 217, 69, 139, 202 }), 80);
            var tcp = p3.CreateConnection("steamcommunity.com", 80);
            var bytes = Encoding.ASCII.GetBytes("GET / HTTP/1.0" + Environment.NewLine + "Host: steamcommunity.com" + Environment.NewLine + Environment.NewLine);
            tcp.GetStream().Write(bytes, 0, bytes.Length);
            Console.WriteLine(new StreamReader(tcp.GetStream()).ReadToEnd());
            tcp.Close();
        }

        static void Main(string[] args)
        {
            tt();
            Console.ReadLine();
            return;
            /*var t = new TcpClient("mail.ru", 80);

            /*CheckMailRu();
            Console.ReadLine();
            return;*/

            //System.Net.ServicePointManager.ServerCertificateValidationCallback += (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => { return true; };

            var ftp = new FtpConnection();
            ftp.CertificateValidationCallback = delegate { return true; };
            ftp.Log = Console.Out;
            //ftp.Proxy = new Socks5("180.153.87.22", 10080);
            string[] workingFTP = new[] { "ftp.smsc.com", "www.pageville.com", "icarus.com", "milum.com", "patches.sgi.com", "psg.com", "qosient.com", "server1.supportwizard.com", "softwest.com", "sources.redhat.com", "sustworks.com", "tcl.activestate.com", "testcase.boulder.ibm.com" };
            ftp.Connect(workingFTP[12]/*"localhost"*/, 21);
            ftp.Login("anonymous", "anonymous@gmail.com");
            ftp.System();
            ftp.SetTransferMode(FtpTransferMode.Stream);
            ftp.SetRepresentationType(FtpRepresentation.Type.ASCII, FtpRepresentation.Param.NonPrint);


            /*ftp.ProtectionBufferSize(0);
            ftp.DataChannelProtectionLevel(FtpDataChannelProtectionLevel.Private);*/

            //ftp.Features();

            //ftp.ChangeWorkingDirectory("");
            //ftp.DeleteFile("1.txt");
            //ftp.MakeDirectory("test2");
            //ftp.RemoveDirectory("test2");

            ftp.EnterPassiveModeEx();
            //ftp.StoreFile("1.txt", Encoding.ASCII.GetBytes("privet!!!"));
            //ftp.AppendFile("1.txt", Encoding.ASCII.GetBytes(Environment.NewLine + "trololo!!!" + Environment.NewLine));
            /*ftp.ChangeWorkingDirectory("pub");
            ftp.ChangeWorkingDirectory("usb");*/
            //ftp.EnterActiveModeEx(IPAddress.Parse("0:0:0:0:0:0:0:1"), 50050);
            string list = ftp.List("");
            var files = new DefaultFtpListParser().Parse(list);
            foreach (var file in files)
                if (file.IsDirectory)
                    Console.WriteLine("<DIR> {0}   ({1})", file.Name, file.ModifiedDate);
                else
                    Console.WriteLine("{0} -- {1} bytes   ({2})", file.Name, file.Length, file.ModifiedDate);

            //var ms = new MemoryStream();
            //ftp.RetrieveFile("readme.txt", 15, ms);
            //Console.WriteLine(ms.Length);
            /*ftp.ChangeWorkingDirectory("pub");
            ftp.RenameFile("upload1.txt", "qq.txt");*/
            //ftp.DeleteFile("upload1.txt");
            //ftp.AppendFile("upload1.txt", new MemoryStream(Encoding.ASCII.GetBytes(Environment.NewLine + "new line")));

            //ftp.MachineListingMany();

            /*for (int i = 0; i < 3; i++)
            {
                ftp.EnterPassiveModeEx();
                var stream = new MemoryStream();
                var cts = new CancellationTokenSource(500);
                var progress = new Progress<long>((p) => Console.WriteLine("Downloaded {0} bytes...", p));
                var task = ftp.RetrieveFileAsync("test.pdf", stream, cts.Token, progress);
                try
                {
                    task.Wait();
                }
                catch (AggregateException ex)
                {
                    Console.WriteLine(ex.InnerExceptions[0].Message);
                }
            }*/

            /*for (int i = 0; i < 3; i++)
            {
                ftp.EnterPassiveModeEx();
                var stream = new MemoryStream();
                for (int j = 1000; j < 2000; j++)
                {
                    var bytes = Encoding.ASCII.GetBytes(j.ToString() + Environment.NewLine);
                    stream.Write(bytes, 0, bytes.Length);
                }
                stream.Position = 0;
                var cts = new CancellationTokenSource(500);
                var progress = new Progress<long>((p) => Console.WriteLine("Uploaded {0} bytes...", p));
                var task = ftp.AppendFileAsync("numbers" + (i + 1) + ".txt", stream, cts.Token, progress);
                try
                {
                    task.Wait();
                }
                catch (AggregateException ex)
                {
                    Console.WriteLine(ex.InnerExceptions[0].Message);
                }
            }*/

            //ftp.EnterPassiveModeEx();
            //ftp.StoreFile("qwerty.txt", 3, new MemoryStream(Encoding.ASCII.GetBytes("789+++")));
            
            //ftp.List();
            //var bytes = ftp.RetrieveFile("12.png");
            ftp.Quit();

            /*var ftp = new FtpConnection();
            ftp.Log = Console.Out;
            ftp.Proxy = new HttpProxy("200.84.4.202", 8080);
            ftp.Connect("ftp.cad.ntu-kpi.kiev.ua", 21);
            ftp.Login("anonymous", "anonymous@gmail.com");
            ftp.System();
            ftp.EnterPassiveMode();
            ftp.List();
            ftp.Logout();*/

            Console.ReadLine();
        }

        #region proxy

        static string plist = @"";

        #endregion

        static void tt()
        {
            var lines = plist.Split(new[] { Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
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
            }
            

            /*var p1 = new Socks5(IPAddress.Parse("98.191.62.242"), 16902);
            //var p1 = new HttpProxy(IPAddress.Parse("175.182.229.2"), 80);
            var tcp = p1.CreateConnection(IPAddress.Parse("1111"), 80);
            var bytes = Encoding.ASCII.GetBytes("GET / HTTP/1.0" + Environment.NewLine + "Host: qwe" + Environment.NewLine + Environment.NewLine);
            tcp.GetStream().Write(bytes, 0, bytes.Length);
            Console.WriteLine(new StreamReader(tcp.GetStream()).ReadToEnd());
            tcp.Close();*/
        }
    }
}