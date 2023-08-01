using Cake.Core;
using Cake.Core.Annotations;
using System.Linq;
using Cake.Core.Diagnostics;
using Renci.SshNet;

namespace Cake.SFTP;

/// <summary>
/// Contains functionality for working with an SFTP server.
/// </summary>
[CakeAliasCategory("SFTP")]
public static class CakeSFTP
{
    /// <summary>
    /// Gets all files from the SFTP server
    /// </summary>
    /// <example>
    /// <code>
    /// var remotefiles = SFTPListAllFiles(settings, "uploads/current");
    /// </code>
    /// </example>
    /// <param name="cakecontext">The context.</param>
    /// <param name="settings">The settings file for the SFTP server.</param>
    /// <param name="remoteDirectory">The folder on the SFTP server where you want to get the contents from.</param>
    /// <returns>A list of the files on the server.</returns>
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
            cakecontext.Log.Write(Verbosity.Normal, LogLevel.Error, "Failed in listing files under [{0}]",
                remoteDirectory);
            return Array.Empty<String>();
        }
        finally
        {
            client.Disconnect();
        }
    }

    /// <summary>
    /// Uploads a file to the SFTP server
    /// </summary>
    /// <example>
    /// <code>
    /// SFTPUploadFile(settings, "./somefile.txt", "/uploads/somefile.txt");
    /// </code>
    /// </example>
    /// <param name="cakecontext">The context.</param>
    /// <param name="settings">The settings file for the SFTP server.</param>
    /// <param name="localFilePath">The path to the local file you want to upload.</param>
    /// <param name="remoteFilePath">The folder on the SFTP server where you want to upload the file, including the remote filename.</param>
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
                "Finished uploading the file [{0}] to [{1}]", localFilePath, remoteFilePath);
        }
        catch
        {
            cakecontext.Log.Write(Verbosity.Normal, LogLevel.Error,
                "Failed uploading the file [{0}] to [{1}]", localFilePath, remoteFilePath);
        }
        finally
        {
            client.Disconnect();
        }
    }

    /// <summary>
    /// Downloads a file from the SFTP server
    /// </summary>
    /// <example>
    /// <code>
    /// SFTPDownloadFile(settings, "./somefile.txt", "/uploads/somefile.txt");
    /// </code>
    /// </example>
    /// <param name="cakecontext">The context.</param>
    /// <param name="settings">The settings file for the SFTP server.</param>
    /// <param name="localFilePath">The path to the local file you want to download.</param>
    /// <param name="remoteFilePath">The folder on the SFTP server where you want to download the file, including the remote filename.</param>
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
                "Finished downloading the file [{0}] from [{1}]", localFilePath,
                remoteFilePath);
        }
        catch
        {
            cakecontext.Log.Write(Verbosity.Normal, LogLevel.Error,
                "Failed downloading the file [{0}] from [{1}]", localFilePath, remoteFilePath);
        }
        finally
        {
            client.Disconnect();
        }
    }

    /// <summary>
    /// Uploads a file to the SFTP server
    /// </summary>
    /// <example>
    /// <code>
    /// SFTPUploadFile(settings, "./somefile.txt", "/uploads/somefile.txt");
    /// </code>
    /// </example>
    /// <param name="cakecontext">The context.</param>
    /// <param name="settings">The settings file for the SFTP server.</param>
    /// <param name="remoteFilePath">The path to the file on the server you want to delete.</param>
    [CakeMethodAlias]
    public static void SFTPDeleteFile(this ICakeContext cakecontext, SFTPSettings settings, string remoteFilePath)
    {
        using var client = new SftpClient(settings.Host, settings.Port == 0 ? 22 : settings.Port, settings.UserName,
            settings.Password);
        try
        {
            client.Connect();
            client.DeleteFile(remoteFilePath);
            cakecontext.Log.Write(Verbosity.Normal, LogLevel.Information, "File [{0}] is deleted.",
                remoteFilePath);
        }
        catch
        {
            cakecontext.Log.Write(Verbosity.Normal, LogLevel.Error, "Failed to delete the file [{0}]",
                remoteFilePath);
        }
        finally
        {
            client.Disconnect();
        }
    }

    /// <summary>
    /// Uploads a file to the SFTP server
    /// </summary>
    /// <example>
    /// <code>
    /// SFTPUploadFile(settings, "./somefile.txt", "/uploads/somefile.txt");
    /// </code>
    /// </example>
    /// <param name="cakecontext">The context.</param>
    /// <param name="settings">The settings file for the SFTP server.</param>
    /// <param name="remoteFilePaths">A list of paths to the files on the server you want to delete.</param>
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
                    cakecontext.Log.Write(Verbosity.Normal, LogLevel.Information, "File [{0}] is deleted.",
                        f);
                }
                catch

                {
                    cakecontext.Log.Write(Verbosity.Normal, LogLevel.Error,
                        "Failed to delete the file [{0}]", f);
                }
            }
        }
        finally
        {
            client.Disconnect();
        }
    }
}
    