# Cake.SFTP
A Cake add-on to upload/download/list/delete files on a SFTP Server

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
