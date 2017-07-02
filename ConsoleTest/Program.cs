﻿using System;
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
            /*var tcp = new TcpClient("localhost", 32028);

            TlsStream stream = new TlsStream(tcp.GetStream());*/

            var server = new TcpListener(IPAddress.Any, 32028);
            server.Start();
            var client = server.AcceptTcpClient();
            var tls = new TlsStream(client.GetStream());
            tls.AuthenticateAsServer("localhost", new X509Certificate2("test.p12", "hh87$-Jqo"));

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


            /*var handler = new TlsClientProtocol(tcp.GetStream(), new Org.BouncyCastle.Security.SecureRandom());
            handler.Connect(new MyTlsClient());
            Console.WriteLine(new StreamReader(handler.Stream).ReadToEnd());
            Console.ReadLine();*/
        }

        /*class MyTlsClient : DefaultTlsClient
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
        }*/
    }
}