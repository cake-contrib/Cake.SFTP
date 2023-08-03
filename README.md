# Cake.SFTP

[![Version](https://img.shields.io/nuget/vpre/Cake.SFTP.svg)](https://www.nuget.org/packages/Cake.SFTP)
[![NuGet download count](https://img.shields.io/nuget/dt/Cake.SFTP.svg)](https://www.nuget.org/packages/Cake.SFTP)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A Cake add-on to upload/download/list/delete files on a SFTP Server

This add-on uses the excellent SSH.NET package (see https://github.com/sshnet/SSH.NET/) 

## Usage

### Include an Add-In directive

#addin "nuget:?package=Cake.SFTP&loaddependencies=true"

### Upload Task Example

```c#
Task("UploadToSFTP")
    .IsDependentOn("CreateInstaller")
    .Does(() =>
{    
    var settings = new SFTPSettings
    {
        UserName = "someUserName",
        Password = "somePassword",
        Host = "192.168.1.100",
        Port = 22
    };

    SFTPUploadFile(settings, "somefile.txt", "uploads/somefile.txt");
});
```
### See Cake Add-On page for more info

https://cakebuild.net/extensions/cake-sftp/
