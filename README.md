# ZergatulLib

## FtpClient class
Provides high-level access to FTP server functions. TODO

## FtpConnection class
Provides low-level access to FTP server functions. In most cases using of FtpClient is preferable. It has more understandable interface, if you are not familiar with FTP protocol specifications.

#### Features
* IPv6 support

### Connect to FTP server
To connect to an FTP server, use Connect method:
```C#
var ftp = new FtpConnection();
// connect using host name and port
ftp.Connect("your.hostname.com", 21);
```

If you need to connect by IP address, use overload of Connect method:
```
ftp.Connect(IPAddress.Parse("127.0.0.1"), 21);
```

You can also connect to IPv6 address:
```
ftp.Connect(IPAddress.Parse("::1"), 21);
```

If you need to use IPv6 and host name, you can specify to prefer IPv6 address. By default IPv4 address is preferred:
```
ftp.PreferIPv4 = false;
// if there is associated to this domain IPv6 address, it will be used.
// If there is no IPv6 addresses, IPv4 address will be used.
ftp.Connect("your.hostname.com", 21);
```


### Login to FTP server
To login to an FTP server, use Login method:
```
var ftp = new FtpConnection();
ftp.Connect("your.hostname.com", 21);
ftp.Login("anonymous", "anonymous@gmail.com");
```

You can also specify account information:
```
ftp.Login("admin", "qwerty");
ftp.Account("account_data");
```