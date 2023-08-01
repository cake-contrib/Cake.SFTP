// See https://aka.ms/new-console-template for more information

using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.SFTP;
using Moq;

Console.WriteLine("Hello, World!");

var s = new SFTPSettings();
s.UserName = "gsd";
s.Password = "123gsd321";
s.Host = "192.168.1.35";
s.Port = 2022;

var m = new Mock<ICakeContext>();
m.Setup(l => l.Log.Write(It.IsAny<Verbosity>(), It.IsAny<LogLevel>(), It.IsAny<String>(), It.IsAny<Object[]>()))
    .Callback((Verbosity a, LogLevel b, String c, Object[] d) => Console.WriteLine(c, d));


//CakeSFTP.SFTPDeleteFile(m.Object, s ,"Thunderbolt.xml");

CakeSFTP.SFTPUploadFile(m.Object, s, "/Users/jeroenvandezande/Documents/repos/GSD/Thundertbolt/thunderbolt-firmware/Thunderbolt.xml", "Uploads/VLAM/ThunderboltX.xml");

var fls = CakeSFTP.SFTPListAllFiles(m.Object, s);
foreach (var f in fls)
{
    Console.WriteLine(f);
}