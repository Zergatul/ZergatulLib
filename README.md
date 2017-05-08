# ZergatulLib

## FtpClient class
Provides high-level access to FTP server functions. TODO

## FtpConnection class
Provides low-level access to FTP server functions. In most cases using of FtpClient is preferable. It has more understandable interface, if you are not familiar with FTP protocol specifications.

#### Features
* IPv6 support

### Connect to FTP server
Simple example:
```C#
var ftp = new FtpConnection();
// connect using host name and port
ftp.Connect("your.hostname.com", 21);
```

If you need to connect by IP address, use overload of Connect method:
```C#
ftp.Connect(IPAddress.Parse("127.0.0.1"), 21);
```

You can also connect to IPv6 address:
```C#
ftp.Connect(IPAddress.Parse("::1"), 21);
```

If you need to use IPv6 and host name, you can specify to prefer IPv6 address. By default IPv4 address is preferred:
```C#
ftp.PreferIPv4 = false;
// if there is associated to this domain IPv6 address, it will be used.
// If there is no IPv6 addresses, IPv4 address will be used.
ftp.Connect("your.hostname.com", 21);
```


### Close connection to FTP server
Quit method will send QUIT command to FTP server and close underlying socket.
```C#
ftp.Quit();
```


### Login to FTP server
Simple example:
```C#
var ftp = new FtpConnection();
ftp.Connect("your.hostname.com", 21);
ftp.Login("anonymous", "anonymous@gmail.com");
```

You can also specify account information:
```C#
ftp.Login("admin", "qwerty");
ftp.Account("account_data");
```


### Reinitialize login status
Sometimes you need to log out, and keeping connection opened, login by using another credentials:
```C#
ftp.Login("admin1", "qwerty1");
// do some actions as admin1
ftp.Reinitialize();
ftp.Login("admin2", "qwerty2");
// do action as admin2
ftp.Quit();
```

### Passive mode
Use EnterPassiveMode or EnterPassiveModeEx method. EnterPassiveMode supports only IPv4, EnterPassiveModeEx supports both IPv4 and IPv6. You should explicitly enter passive or active mode before make any data transfer:
```C#
ftp.EnterPassiveMode();
ftp.RetrieveFile("1.txt", stream1);
ftp.EnterPassiveMode();
ftp.RetrieveFile("2.txt", stream2);
```


### Active mode
Use EnterActiveMode or EnterActiveModeEx method. EnterActiveMode supports only IPv4, EnterActiveModeEx supports both IPv4 and IPv6. You should explicitly enter passive or active mode before make any data transfer:
```C#
ftp.EnterActiveMode(IPAddress.Parse("127.0.0.1"), 60000);
ftp.RetrieveFile("1.txt", stream);
```
```C#
ftp.EnterActiveModeEx(IPAddress.Parse("::1"), 60000);
ftp.RetrieveFile("1.txt", stream);
```

### Transfer mode
Default mode is Stream. Other modes are not implemented.
```C#
ftp.SetTransferMode(FtpTransferMode.Stream);
```


### Representation type
Example:
```C#
ftp.SetRepresentationType(FtpRepresentation.Type.ASCII, FtpRepresentation.Param.NonPrint);
```


### Download/upload files
Before each operation you should explicitly enter active or passive mode by using corresponding methods.
Simple example:
```C#
ftp.EnterPassiveModeEx();
var fileStream = File.OpenWrite("1.txt");
ftp.RetrieveFile("1.txt, fileStream);
fileStream.Close();
```
```C#
ftp.EnterPassiveModeEx();
var fileStream = File.OpenRead("2.txt");
ftp.StoreFile("2.txt, fileStream);
fileStream.Close();
```
You can resume downloading or uploading from specified position:
```C#
// download starts from 100th byte
ftp.RetrieveFile("1.txt, 100, stream);
```
```C#
// upload starts from 100th byte
ftp.StoreFile("2.txt, 100, stream);
```
You can also append files:
```C#
ftp.EnterPassiveModeEx();
ftp.AppendFile("3.txt", stream);
```


### Download/upload files asynchronously
Example:
```C#
ftp.EnterPassiveModeEx();
var stream = new MemoryStream();
var cts = new CancellationTokenSource(5000); // download will abort after 5 seconds
var progress = new Progress<long>((p) => Console.WriteLine("Downloaded {0} bytes...", p)); // display progress
var task = ftp.RetrieveFileAsync("test.txt", stream, cts.Token, progress);
task.Wait();
```
You can also resume download/upload from specified position:
```C#
// upload starts from 100th byte
var task = ftp.StoreFileAsync("test.txt", 100, stream, cts.Token, progress);
```


### Rename file
```C#
ftp.RenameFile("old.txt", "new.txt");
```


### Delete file
```C#
ftp.DeleteFile("db.bak");
```


### Create directory
```C#
ftp.MakeDirectory("pub");
```


### Delete directory
```C#
ftp.RemoveDirectory("pub");
```


### Change working directory
```C#
ftp.ChangeWorkingDirectory("pub");
```


### Change working directory to parent directory
```C#
ftp.ChangeToParentDirectory();
```


### Enter secure mode
```C#
ftp.AuthTls();
```
You can specify client certificates by using X509CertificateCollection property:
```C#
ftp.X509CertificateCollection.Add(new X509Certificate("cert.p12"));
ftp.AuthTls();
```
You can override certificate validation:
```C#
ftp.CertificateValidationCallback = /* your method */;
ftp.AuthTls();
```
For testing purpose you can accpect all certificates:
```C#
ftp.CertificateValidationCallback = delegate { return true; };
ftp.AuthTls();
```


### Securing data connection
```C#
ftp.AuthTls();
ftp.ProtectionBufferSize(0);
ftp.DataChannelProtectionLevel(FtpDataChannelProtectionLevel.Private);
// any file transfter will use ssl now
```


### Return back to unsecure connection
This can be used for firewalls, if your ports are closed by default, and firewall scans traffic from FTP connection:
```C#
ftp.AuthTls();
ftp.Login("admin", "qwerty"); // send credentials under secure connection
ftp.ClearCommandChannel(); // retun back to unsecure connection
ftp.EnterPassiveMode(); // firewall can parse response from server and allow connection by specified port
```


### Logging
```C#
ftp.Log = Console.Out;
```


### Proxy
```C#
ftp.Proxy = new Socks4("127.0.0.1", 1001);
```
