using Org.BouncyCastle.Crypto.Tls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Zergatul;
using Zergatul.Cryptography.Certificate;
using Zergatul.Ftp;
using Zergatul.Math;
using Zergatul.Network;
using Zergatul.Network.Proxy;
using Zergatul.Network.Tls;

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
            //DownloadOIDs.Go("1.3.6.1.5.5.7.48", "1.txt");
            //return;

            //var obj = Asn1Object.FromByteArray(BitHelper.HexToBytes("3026302406082b060105050730018618687474703a2f2f6f6373702e64696769636572742e636f6d"));
            //return;

            //string cerfile1 = "1.cer";
            //string cerfile2 = "2.cer";
            //var cert1m = new Zergatul.Cryptography.Certificate.X509Certificate(cerfile1);
            //var cert2m = new Zergatul.Cryptography.Certificate.X509Certificate(cerfile2);
            //var cert1c = new X509Certificate2(cerfile1);

            //#region check base props

            //if (cert1m.SerialNumberString == cert1c.SerialNumber)
            //    Console.WriteLine("Serial number ok!");
            //else
            //    throw new Exception();

            //if (cert1m.NotBefore == cert1c.NotBefore)
            //    Console.WriteLine("NotBefore ok!");
            //else
            //    throw new Exception();

            //if (cert1m.NotAfter == cert1c.NotAfter)
            //    Console.WriteLine("NotAfter ok!");
            //else
            //    throw new Exception();

            //if (cert1m.Issuer == cert1c.Issuer)
            //    Console.WriteLine("Issuer ok!");
            //else
            //    throw new Exception();

            //if (cert1m.Subject == cert1c.Subject)
            //    Console.WriteLine("Subject ok!");
            //else
            //    throw new Exception();

            //if (cert1m.SignatureAlgorithm.DotNotation == cert1c.SignatureAlgorithm.Value)
            //    Console.WriteLine("SignatureAlgorithm ok!");
            //else
            //    throw new Exception();

            //#endregion

            //cert1m.PublicKey.ResolveAlgorithm();

            //Console.WriteLine("1:" + cert1m.Subject);
            //Console.WriteLine("1 SubjKey: " + BitHelper.BytesToHex(cert1m.Extensions.OfType<SubjectKeyIdentifier>().Single().KeyIdentifier));
            //Console.WriteLine("1 AuthKey: " + BitHelper.BytesToHex(cert1m.Extensions.OfType<AuthorityKeyIdentifier>().Single().KeyIdentifier));

            //Console.WriteLine();

            //Console.WriteLine("2:" + cert2m.Subject);
            //Console.WriteLine("2 SubjKey: " + BitHelper.BytesToHex(cert2m.Extensions.OfType<SubjectKeyIdentifier>().Single().KeyIdentifier));
            //Console.WriteLine("2 AuthKey: " + BitHelper.BytesToHex(cert2m.Extensions.OfType<AuthorityKeyIdentifier>().Single().KeyIdentifier));

            //return;

            //var store = new System.Security.Cryptography.X509Certificates.X509Store(StoreName.Root, StoreLocation.LocalMachine);
            //store.Open(OpenFlags.ReadOnly);
            //var coll = store.Certificates.Find(
            //    System.Security.Cryptography.X509Certificates.X509FindType.FindBySubjectKeyIdentifier,
            //    "b13ec36903f8bf4701d498261a0802ef63642bc3",
            //    false);
            /*var coll = store.Certificates.Find(
                System.Security.Cryptography.X509Certificates.X509FindType.FindByThumbprint,
                "5FB7EE0633E259DBAD0C4C9AE6D38F1A61C7DC25",
                false);*/

            //bool testLocalHost = false;
            //if (testLocalHost)
            //{
            //    var client = new TcpClient("localhost", 32028);
            //    var tls = new TlsStream(client.GetStream());
            //    tls.AuthenticateAsClient("localhost");

            //    byte[] buffer = new byte[12];
            //    tls.Read(buffer, 0, 12);

            //    Console.WriteLine(Encoding.UTF8.GetString(buffer));
            //}
            //else
            //{
            //    string host =
            //        //"www.howsmyssl.com"
            //        //"ru.wargaming.net"
            //        "ru.4game.com"
            //        ;

            //    var client = new TcpClient(host, 443);
            //    var tls = new TlsStream(client.GetStream());
            //    tls.AuthenticateAsClient(host);

            //    string request =
            //        "GET / HTTP/1.0" + Environment.NewLine +
            //        "Host: " + host + Environment.NewLine +
            //        Environment.NewLine;
            //    tls.Write(Encoding.ASCII.GetBytes(request));
            //    byte[] buffer = new byte[500];
            //    tls.Read(buffer, 0, 500);
            //    Console.WriteLine(Encoding.UTF8.GetString(buffer));
            //}

            // b13ec36903f8bf4701d498261a0802ef63642bc3

            /*var server = new TcpListener(IPAddress.Any, 32028);
            server.Start();
            var client = server.AcceptTcpClient();
            var tls = new TlsStream(client.GetStream());
            tls.AuthenticateAsServer("localhost", new X509Certificate2("test.p12", "hh87$-Jqo"));

            tls.Write(Encoding.ASCII.GetBytes("Hello"), 0, 5);*/

            //var ec = ECPoint.FromBytes(new uint[] { 0x03, 0x188DA80E, 0xB03090F6, 0x7CBF20EB, 0x43A18800, 0xF4FF0AFD, 0x82FF1012 }, EllipticCurve.secp192r1);

            /*SslStream stream = new SslStream(tcp.GetStream(), false, (a1, a2, a3, a4) => true);
            stream.AuthenticateAsClient("localhost", null, System.Security.Authentication.SslProtocols.Tls12, false);
            Console.WriteLine("**************************************************");
            Console.WriteLine(stream.CipherAlgorithm);
            Console.WriteLine(stream.CipherStrength);
            Console.WriteLine(stream.HashAlgorithm);
            Console.WriteLine(stream.HashStrength);
            Console.WriteLine(stream.KeyExchangeAlgorithm);
            Console.WriteLine(stream.KeyExchangeStrength);
            Console.WriteLine("**************************************************");
            Console.WriteLine(new StreamReader(stream).ReadToEnd());
            stream.Close();
            Console.ReadLine();*/

            var tcp = new TcpClient("localhost", 32028);
            var handler = new TlsClientProtocol(tcp.GetStream(), new Org.BouncyCastle.Security.SecureRandom());
            handler.Connect(new MyTlsClient());
            Console.WriteLine(new StreamReader(handler.Stream).ReadToEnd());
            Console.ReadLine();
        }

        class MyTlsClient : DefaultTlsClient
        {
            public override TlsAuthentication GetAuthentication()
            {
                return new MyTlsAuthentication();
            }
        }

        // Need class to handle certificate auth
        class MyTlsAuthentication : TlsAuthentication
        {
            public TlsCredentials GetClientCredentials(CertificateRequest certificateRequest)
            {
                // return client certificate
                return null;
            }

            public void NotifyServerCertificate(Certificate serverCertificate)
            {
                // validate server certificate
            }
        }
    }
}