using Cake.Core;
using Cake.Core.Annotations;
using System.Linq;
using Cake.Core.Diagnostics;
using Renci.SshNet;

namespace Cake.SFTP;

[CakeAliasCategory("SFTP")]
public static class CakeSFTP
{
    [CakeMethodAlias]
    public static IEnumerable<String> SFTPListAllFiles(this ICakeContext cakecontext, SFTPSettings settings,
        String remoteDirectory = ".")
    {
        using var client = new SftpClient(settings.Host, settings.Port == 0 ? 22 : settings.Port, settings.UserName,
            settings.Password);
        try
        {
            client.Connect();
            return from f in client.ListDirectory(remoteDirectory) where f.Name != "." select f.Name;
        }
        catch
        {
            cakecontext.Log.Write(Verbosity.Normal, LogLevel.Error, "Failed in listing files under [{remoteDirectory}]",
                remoteDirectory);
            return Array.Empty<String>();
        }
        finally
        {
            client.Disconnect();
        }
    }

    [CakeMethodAlias]
    public static void SFTPUploadFile(this ICakeContext cakecontext, SFTPSettings settings, string localFilePath,
        string remoteFilePath)
    {
        using var client = new SftpClient(settings.Host, settings.Port == 0 ? 22 : settings.Port, settings.UserName,
            settings.Password);
        try
        {
            client.Connect();
            using var s = File.OpenRead(localFilePath);
            client.UploadFile(s, remoteFilePath);
            cakecontext.Log.Write(Verbosity.Normal, LogLevel.Information,
                "Finished uploading the file [{localFilePath}] to [{remoteFilePath}]", localFilePath, remoteFilePath);
        }
        catch
        {
            cakecontext.Log.Write(Verbosity.Normal, LogLevel.Error,
                "Failed uploading the file [{localFilePath}] to [{remoteFilePath}]", localFilePath, remoteFilePath);
        }
        finally
        {
            client.Disconnect();
        }
    }

    [CakeMethodAlias]
    public static void SFTPDownloadFile(this ICakeContext cakecontext, SFTPSettings settings, string remoteFilePath,
        string localFilePath)
    {
        using var client = new SftpClient(settings.Host, settings.Port == 0 ? 22 : settings.Port, settings.UserName,
            settings.Password);
        try
        {
            client.Connect();
            using var s = File.Create(localFilePath);
            client.DownloadFile(remoteFilePath, s);
            cakecontext.Log.Write(Verbosity.Normal, LogLevel.Information,
                "Finished downloading the file [{localFilePath}] from [{remoteFilePath}]", localFilePath,
                remoteFilePath);
        }
        catch
        {
            cakecontext.Log.Write(Verbosity.Normal, LogLevel.Error,
                "Failed downloading the file [{localFilePath}] from [{remoteFilePath}]", localFilePath, remoteFilePath);
        }
        finally
        {
            client.Disconnect();
        }
    }

    [CakeMethodAlias]
    public static void SFTPDeleteFile(this ICakeContext cakecontext, SFTPSettings settings, string remoteFilePath)
    {
        using var client = new SftpClient(settings.Host, settings.Port == 0 ? 22 : settings.Port, settings.UserName,
            settings.Password);
        try
        {
            client.Connect();
            client.DeleteFile(remoteFilePath);
            cakecontext.Log.Write(Verbosity.Normal, LogLevel.Information, "File [{remoteFilePath}] is deleted.",
                remoteFilePath);
        }
        catch
        {
            cakecontext.Log.Write(Verbosity.Normal, LogLevel.Error, "Failed to delete the file [{remoteFilePath}]",
                remoteFilePath);
        }
        finally
        {
            client.Disconnect();
        }
    }

    [CakeMethodAlias]
    public static void SFTPDeleteFiles(this ICakeContext cakecontext, SFTPSettings settings,
        IList<String> remoteFilePaths)
    {
        using var client = new SftpClient(settings.Host, settings.Port == 0 ? 22 : settings.Port, settings.UserName,
            settings.Password);
        try
        {
            client.Connect();

            foreach (var f in remoteFilePaths)
            {
                try
                {
                    client.DeleteFile(f);
                    cakecontext.Log.Write(Verbosity.Normal, LogLevel.Information, "File [{remoteFilePath}] is deleted.",
                        f);
                }
                catch

                {
                    cakecontext.Log.Write(Verbosity.Normal, LogLevel.Error,
                        "Failed to delete the file [{remoteFilePath}]", f);
                }
            }
        }
        finally
        {
            client.Disconnect();
        }
    }
}
    