﻿using Org.BouncyCastle.Crypto.Tls;
using Org.BouncyCastle.Pkcs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zergatul;
using Zergatul.Cryptography;
using Zergatul.Cryptography.Symmetric;
using Zergatul.Cryptography.Certificate;
using Zergatul.Ftp;
using Zergatul.Math;
using Zergatul.Network;
using Zergatul.Network.ASN1;
using Zergatul.Network.Proxy;
using Zergatul.Network.Tls;
using Zergatul.Network.Tls.Extensions;

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

        // TODO: GenerateOIDs, use Lazy<T> ???

        static void Main(string[] args)
        {
            var serpent = new Org.BouncyCastle.Crypto.Engines.SerpentEngine();
            serpent.Init(true, new Org.BouncyCastle.Crypto.Parameters.KeyParameter(BitHelper.HexToBytes("8000000000000000000000000000000000000000000000000000000000000000")));
            byte[] res = new byte[16];
            serpent.ProcessBlock(BitHelper.HexToBytes("00000000000000000000000000000000"), 0, res, 0);
            Console.WriteLine(BitHelper.BytesToHex(res));
            return;

            /*var tests = new Zergatul.Tls.Tests.FragmentationTests();
            tests.Send100MB();*/

            // dsa-1024: kK0-;;*1[eH
            // dsa-3072: ;qIU8*1m,q3
            //var cert = new Zergatul.Cryptography.Certificate.X509Certificate("dsa-1024.pfx", "kK0-;;*1[eH");
            //var cert = new Zergatul.Cryptography.Certificate.X509Certificate("dsa-3072.pfx", ";qIU8*1m,q3");
            //var cert = new Zergatul.Cryptography.Certificate.X509Certificate(@"dh-2048.pfx", @"kJ==+`!j8qzm&");
            /*var cert = new Zergatul.Cryptography.Certificate.X509Certificate(@"ecdsa-secp521r1.pfx", @"\7FoIK*f1{\q");
            cert.PublicKey.ResolveAlgorithm();
            cert.PrivateKey.ResolveAlgorithm();*/

            //var element = ASN1Element.ReadFrom(BitHelper.HexToBytes("3059301306072A8648CE3D020106082A8648CE3D03010703420004405134B8399A666F12ADA32F19F646F85FB7D1FC3D9AA0688BF8984A35B86AD49FBB96CFAD78B994013D06D7B98C09B0104323DD0A2473CDCA79342277027D3F"));

            //TestMyServerAndBCClient();
            //TestMyClientAndBCServer();
            //TestMyServerAndNETClient();
            //TestMyClientAndNETServer();
            //ConnectToExternal();
            //TestBlockCipher();
            //DownloadOIDs.Go("1.2.840.10045.3.1", "1.txt");
            TestSpeed();
            return;

            // l(*qqqJ;q30[]e

            /*Org.BouncyCastle.Pkcs.Pkcs12Store store = new Org.BouncyCastle.Pkcs.Pkcs12Store();
            store.Load(new FileStream("../../../ConsoleTest2/test.p12", FileMode.Open), "hh87$-Jqo".ToCharArray());
            return;*/

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
            //byte[] buffer;
            //if (testLocalHost)
            //{
            //    var client = new TcpClient("localhost", 32028);
            //    var tls = new TlsStream(client.GetStream());
            //    tls.AuthenticateAsClient("localhost");

            //    buffer = new byte[12];
            //    tls.Read(buffer, 0, 12);

            //    Console.WriteLine(Encoding.UTF8.GetString(buffer));
            //}
            //else
            //{
                
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
        }

        [DllImport("../../../x64/Release/ZergatulLibNative.dll")]
        public static extern unsafe void AESEncRound(byte* state, byte* key);

        private static unsafe void TestSpeed()
        {
            int cycles = 1000000;

            var rnd = new DefaultSecureRandom();

            var key = new byte[32];
            rnd.GetBytes(key);

            var plain = new byte[16];
            rnd.GetBytes(plain);

            /**/

            fixed (byte* state = plain)
            {
                fixed (byte* rkey = key)
                {
                    AESEncRound(state, rkey);
                }
            }
            return;

            /**/

            var encrypt = new AES256().CreateEncryptor(key);
            var decrypt = new AES256().CreateDecryptor(key);

            var sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < cycles; i++)
            {
                byte[] x = decrypt(encrypt(plain));
            }
            sw.Stop();

            Console.WriteLine($"My: {sw.Elapsed}");

            var rinj = new System.Security.Cryptography.RijndaelManaged();
            rinj.Key = key;
            rinj.Padding = System.Security.Cryptography.PaddingMode.None;
            var encryptor = rinj.CreateEncryptor();
            var decryptor = rinj.CreateDecryptor();

            sw.Restart();
            for (int i = 0; i < cycles; i++)
            {
                byte[] x = decryptor.TransformFinalBlock(encryptor.TransformFinalBlock(plain, 0, 16), 0, 16);
            }
            sw.Stop();

            Console.WriteLine($".NET: {sw.Elapsed}");

            var engEnc = new Org.BouncyCastle.Crypto.Engines.AesEngine();
            engEnc.Init(true, new Org.BouncyCastle.Crypto.Parameters.KeyParameter(key));
            var engDec = new Org.BouncyCastle.Crypto.Engines.AesEngine();
            engDec.Init(false, new Org.BouncyCastle.Crypto.Parameters.KeyParameter(key));

            sw.Restart();
            for (int i = 0; i < cycles; i++)
            {
                byte[] c = new byte[16];
                engEnc.ProcessBlock(plain, 0, c, 0);
                byte[] p = new byte[16];
                engDec.ProcessBlock(c, 0, p, 0);
            }
            sw.Stop();

            Console.WriteLine($"BC: {sw.Elapsed}");
        }

        private static void TestBlockCipher()
        {
            var cipher = new Org.BouncyCastle.Crypto.Modes.GcmBlockCipher(new Org.BouncyCastle.Crypto.Engines.AesEngine());
            var @params = new Org.BouncyCastle.Crypto.Parameters.AeadParameters(
                new Org.BouncyCastle.Crypto.Parameters.KeyParameter(BitHelper.HexToBytes("00000000000000000000000000000000")),
                16 * 8,
                BitHelper.HexToBytes("000000000000000000000000"));
            cipher.Init(true, @params);

            var ciphertext = new byte[32];
            int len = cipher.ProcessBytes(BitHelper.HexToBytes("00000000000000000000000000000000"), 0, 16, ciphertext, 0);
            cipher.DoFinal(ciphertext, len);
        }

        private static void ConnectToExternal()
        {
            /*var sc = new SupportedEllipticCurves(new Zergatul.Network.Tls.NamedCurve[]
            {
                Zergatul.Network.Tls.NamedCurve.secp384r1,
                Zergatul.Network.Tls.NamedCurve.secp256r1,
            });
            var x = sc.Data;*/


            string host =
                    //"www.howsmyssl.com"
                    //"ru.wargaming.net"
                    //"ru.4game.com"
                    //"www.tera-online.ru"
                    //"www.facebook.com"
                    //"security.stackexchange.com"
                    //"www.avito.ru"
                    //"ria.ru"
                    //"kp.ru"
                    //"lenta.ru" // chacha20-poly1305
                    //"gazeta.ru"
                    //"vesti.ru"
                    //"smi2.ru"
                    //"rt.com"
                    //"iz.ru"
                    "rg.ru"
                    //"fishki.net"
                    //"yahoo.com"
                    //"google.com"
                    //"jve.linuxwall.info"
                    ;

            var client = new TcpClient(host, 443);
            var tls = new TlsStream(client.GetStream());
            tls.Settings = new TlsStreamSettings
            {
                SupportExtendedMasterSecret = true,
                //CipherSuites = TlsStream.SupportedCipherSuites
                //    .Where(cs => cs.ToString().Contains("CAMELLIA"))
                //    .Where(cs => cs.ToString().Contains("GCM"))
                //    .ToArray(),
                CipherSuites = new Zergatul.Network.Tls.CipherSuite[]
                {
                    //Zergatul.Network.Tls.CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA,
                    //Zergatul.Network.Tls.CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA256,
                    //Zergatul.Network.Tls.CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA,
                    //Zergatul.Network.Tls.CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA384,
                    //Zergatul.Network.Tls.CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_GCM_SHA256,
                    //Zergatul.Network.Tls.CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_GCM_SHA384,
                    //Zergatul.Network.Tls.CipherSuite.TLS_DHE_RSA_WITH_AES_128_GCM_SHA256,
                    //Zergatul.Network.Tls.CipherSuite.TLS_DHE_RSA_WITH_AES_128_CCM,
                    //Zergatul.Network.Tls.CipherSuite.TLS_DHE_RSA_WITH_AES_128_CCM_8,
                    //Zergatul.Network.Tls.CipherSuite.TLS_DHE_RSA_WITH_AES_256_CCM,
                    //Zergatul.Network.Tls.CipherSuite.TLS_DHE_RSA_WITH_AES_256_CCM_8,

                    //Zergatul.Network.Tls.CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA,
                    //Zergatul.Network.Tls.CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA256,
                    //Zergatul.Network.Tls.CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_GCM_SHA256,
                    //Zergatul.Network.Tls.CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CCM,
                    //Zergatul.Network.Tls.CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CCM_8,

                    //Zergatul.Network.Tls.CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CBC_SHA,
                    //Zergatul.Network.Tls.CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CBC_SHA384,
                    Zergatul.Network.Tls.CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_GCM_SHA384,
                    //Zergatul.Network.Tls.CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CCM,
                    //Zergatul.Network.Tls.CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CCM_8,

                    //Zergatul.Network.Tls.CipherSuite.TLS_DHE_RSA_WITH_CHACHA20_POLY1305_SHA256,
                    //Zergatul.Network.Tls.CipherSuite.TLS_ECDHE_RSA_WITH_CHACHA20_POLY1305_SHA256,
                    //Zergatul.Network.Tls.CipherSuite.TLS_ECDHE_ECDSA_WITH_CHACHA20_POLY1305_SHA256
                },
                SupportedCurves = Enumerable.Range(1, 25).Select(i => (Zergatul.Network.Tls.NamedGroup)i).ToArray()
            };
            tls.AuthenticateAsClient(host);

            string request =
                "GET / HTTP/1.0" + Environment.NewLine +
                "Host: " + host + Environment.NewLine +
                Environment.NewLine;
            tls.Write(Encoding.ASCII.GetBytes(request));
            byte[] buffer = new byte[500];
            tls.Read(buffer, 0, 500);
            Console.WriteLine(Encoding.UTF8.GetString(buffer));

            Console.ReadLine();
        }

        private static void TestMyServerAndNETClient()
        {
            var serverThread = new Thread(() =>
            {
                var listener = new TcpListener(IPAddress.Any, 32028);
                listener.Start();

                var client = listener.AcceptTcpClient();
                var tlsStream = new TlsStream(client.GetStream());
                tlsStream.Settings = new TlsStreamSettings
                {
                    CipherSuites = new Zergatul.Network.Tls.CipherSuite[]
                    {
                        Zergatul.Network.Tls.CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA,
                        Zergatul.Network.Tls.CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA256,
                        Zergatul.Network.Tls.CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA,
                        Zergatul.Network.Tls.CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA384,
                    },
                    SupportedCurves = new Zergatul.Network.Tls.NamedGroup[]
                    {
                        Zergatul.Network.Tls.NamedGroup.secp256r1
                    }
                };

                tlsStream.AuthenticateAsServer("localhost", new Zergatul.Cryptography.Certificate.X509Certificate("test.p12", "hh87$-Jqo"));
                tlsStream.Write(Encoding.ASCII.GetBytes("Hello World!"));
            });

            var clientThread = new Thread(() =>
            {
                var tcp = new TcpClient("localhost", 32028);
                var ssl = new SslStream(tcp.GetStream(), true, (p1, p2, p3, p4) => true);
                ssl.AuthenticateAsClient("localhost");


                byte[] buffer = new byte[12];
                ssl.Read(buffer, 0, 12);
                Console.WriteLine(Encoding.ASCII.GetString(buffer));

                ssl.Close();
            });

            serverThread.Start();
            clientThread.Start();

            serverThread.Join();
            clientThread.Join();

            Console.ReadLine();
        }

        private static void TestMyClientAndNETServer()
        {
            var serverThread = new Thread(() =>
            {
                var listener = new TcpListener(IPAddress.Any, 32028);
                listener.Start();

                var client = listener.AcceptTcpClient();

                var ssl = new SslStream(client.GetStream(), true, (p1, p2, p3, p4) => true);
                var cert = new System.Security.Cryptography.X509Certificates.X509Certificate2("test.p12", "hh87$-Jqo");
                ssl.AuthenticateAsServer(cert, false, System.Security.Authentication.SslProtocols.Tls12, false);

                ssl.Write(Encoding.ASCII.GetBytes("Hello World!"));

                ssl.Close();
            });

            var clientThread = new Thread(() =>
            {
                var tcp = new TcpClient("localhost", 32028);
                var tls = new TlsStream(tcp.GetStream());
                tls.AuthenticateAsClient("localhost");

                byte[] buffer = new byte[12];
                tls.Read(buffer, 0, 12);
                Console.WriteLine(Encoding.ASCII.GetString(buffer));
            });

            serverThread.Start();
            clientThread.Start();

            serverThread.Join();
            clientThread.Join();

            Console.ReadLine();
        }

        private static void TestMyServerAndBCClient()
        {
            bool ECcert = false;
            var serverThread = new Thread(() =>
            {
                var listener = new TcpListener(IPAddress.Any, 32028);
                listener.Start();

                var client = listener.AcceptTcpClient();
                var tlsStream = new TlsStream(client.GetStream());
                tlsStream.Settings = new TlsStreamSettings
                {
                    CipherSuites = new Zergatul.Network.Tls.CipherSuite[]
                    {
                        /*Zergatul.Network.Tls.CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA,
                        Zergatul.Network.Tls.CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA256,
                        Zergatul.Network.Tls.CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA,*/
                        //Zergatul.Network.Tls.CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA384,
                        //Zergatul.Network.Tls.CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_GCM_SHA256
                        Zergatul.Network.Tls.CipherSuite.TLS_DHE_RSA_WITH_CHACHA20_POLY1305_SHA256
                    },
                    SupportedCurves = new Zergatul.Network.Tls.NamedGroup[]
                    {
                        Zergatul.Network.Tls.NamedGroup.secp256r1
                    }
                };

                var cert = ECcert ?
                    new Zergatul.Cryptography.Certificate.X509Certificate("ecdsa-cert.pfx", @"\7FoIK*f1{\q") :
                    new Zergatul.Cryptography.Certificate.X509Certificate("rsa-cert.pfx", "hh87$-Jqo");
                tlsStream.AuthenticateAsServer("localhost", cert);
                tlsStream.Write(Encoding.ASCII.GetBytes("Hello World!"));
            });

            var clientThread = new Thread(() =>
            {
                var tcp = new TcpClient("localhost", 32028);
                var handler = new TlsClientProtocol(tcp.GetStream(), new Org.BouncyCastle.Security.SecureRandom());
                handler.Connect(new MyTlsClient(Zergatul.Network.Tls.CipherSuite.TLS_DHE_RSA_WITH_CHACHA20_POLY1305_SHA256));

                byte[] buffer = new byte[12];
                handler.Stream.Read(buffer, 0, 12);
                Console.WriteLine(Encoding.ASCII.GetString(buffer));

                handler.Close();
            });

            serverThread.Start();
            clientThread.Start();

            serverThread.Join();
            clientThread.Join();

            Console.ReadLine();
        }

        private static void TestMyClientAndBCServer()
        {
            bool ECcert = false;

            var serverThread = new Thread(() =>
            {
                var listener = new TcpListener(IPAddress.Any, 32028);
                listener.Start();

                Pkcs12Store store = new Pkcs12Store();
                store.Load(new FileStream(ECcert ? "ecdsa-cert.pfx" : "rsa-cert.pfx", FileMode.Open), (ECcert ? @"\7FoIK*f1{\q" : "hh87$-Jqo").ToCharArray());

                var client = listener.AcceptTcpClient();
                var handler = new TlsServerProtocol(client.GetStream(), new Org.BouncyCastle.Security.SecureRandom());
                handler.Accept(new MyTlsServer(store));

                handler.Stream.Write(Encoding.ASCII.GetBytes("Hello World!"), 0, 12);
            });

            var clientThread = new Thread(() =>
            {
                var tcp = new TcpClient("localhost", 32028);

                var tlsStream = new TlsStream(tcp.GetStream());
                tlsStream.Settings = new TlsStreamSettings
                {
                    CipherSuites = new Zergatul.Network.Tls.CipherSuite[]
                    {
                        Zergatul.Network.Tls.CipherSuite.TLS_DHE_RSA_WITH_AES_128_CBC_SHA
                    }
                };
                tlsStream.AuthenticateAsClient("localhost");

                byte[] buffer = new byte[12];
                tlsStream.Read(buffer, 0, 12);

                Console.WriteLine(Encoding.ASCII.GetString(buffer));
            });

            serverThread.Start();
            clientThread.Start();

            serverThread.Join();
            clientThread.Join();

            Console.ReadLine();
        }

        class MyTlsClient : DefaultTlsClient
        {
            private Zergatul.Network.Tls.CipherSuite[] _suites;

            public MyTlsClient(params Zergatul.Network.Tls.CipherSuite[] suites)
            {
                this._suites = suites;
            }

            public override TlsAuthentication GetAuthentication()
            {
                return new MyTlsAuthentication();
            }

            public override int[] GetCipherSuites()
            {
                if (_suites.Length == 0)
                    return base.GetCipherSuites();
                else
                    return _suites.Select(cs => (int)cs).ToArray();
            }
        }

        class MyTlsServer : DefaultTlsServer
        {
            private Pkcs12Store _store;

            public MyTlsServer(Pkcs12Store store)
            {
                this._store = store;
            }

            protected override Org.BouncyCastle.Crypto.Tls.ProtocolVersion MinimumVersion => Org.BouncyCastle.Crypto.Tls.ProtocolVersion.TLSv12;
            protected override Org.BouncyCastle.Crypto.Tls.ProtocolVersion MaximumVersion => Org.BouncyCastle.Crypto.Tls.ProtocolVersion.TLSv12;

            public override Org.BouncyCastle.Crypto.Tls.ProtocolVersion GetServerVersion()
            {
                return base.GetServerVersion();
                //return Org.BouncyCastle.Crypto.Tls.ProtocolVersion.TLSv12;
            }

            protected override TlsSignerCredentials GetRsaSignerCredentials()
            {
                var cert = new Certificate(new Org.BouncyCastle.Asn1.X509.X509CertificateStructure[]
                {
                    _store.GetCertificate("be6f96853dd4066f51238725362420b7b193713c").Certificate.CertificateStructure
                });
                return new DefaultTlsSignerCredentials(
                    this.mContext,
                    cert,
                    _store.GetKey("BE6F96853DD4066F51238725362420B7B193713C").Key,
                    // SHA512, RSA
                    new SignatureAndHashAlgorithm(6, 1));
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