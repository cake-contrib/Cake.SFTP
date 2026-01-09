using Cake.Core;
using Cake.Core.Annotations;
using System.Linq;
using System.Text;
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
    /// var remotefiles = SFTPListAllFiles(settings, &quot;uploads/current&quot;);
    /// </code>
    /// </example>
    /// <param name="cakecontext">The context.</param>
    /// <param name="settings">The settings for the SFTP server.</param>
    /// <param name="remoteDirectory">The folder on the SFTP server where you want to get the contents from.</param>
    /// <returns>A list of the files on the server.</returns>
    [CakeMethodAlias]
    public static IEnumerable<String> SFTPListAllFiles(this ICakeContext cakecontext, SFTPSettings settings,
        String remoteDirectory = ".")
    {
        using var client = new SftpClient(_CreateConnectionInfo(settings));
        try
        {
            client.Connect();
            return from f in client.ListDirectory(remoteDirectory) where f.Name != "." select f.Name;
        }
        catch
        {
            cakecontext.Log.Write(Verbosity.Normal, LogLevel.Error, "Failed in listing files under [{0}]",
                remoteDirectory);
            throw;
        }
        finally
        {
            client.Disconnect();
        }
    }

    /// <summary>
    /// Creates a folder on the SFTP server
    /// </summary>
    /// <param name="cakecontext">The context.</param>
    /// <param name="settings">The settings for the SFTP server.</param>
    /// <param name="remoteDirectoryPath">The folder on the SFTP server that you want to create.</param>
    [CakeMethodAlias]
    public static void SFTPCreateRemoteDirectory(this ICakeContext cakecontext, SFTPSettings settings, string remoteDirectoryPath)
    {
        using var client = new SftpClient(_CreateConnectionInfo(settings));
        try
        {
            client.Connect();
            client.CreateDirectory(remoteDirectoryPath);
            cakecontext.Log.Write(Verbosity.Normal, LogLevel.Information,
                "Finished creating the folder [{0}] on server", remoteDirectoryPath);
        }
        catch
        {
            cakecontext.Log.Write(Verbosity.Normal, LogLevel.Error,
                "Failed creating the folder [{0}] on server", remoteDirectoryPath);
            throw;
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
    ///var settings = new SFTPSettings
    ///{
    ///    UserName = &quot;someUserName&quot;,
    ///    Password = &quot;somePassword&quot;,
    ///    Host = &quot;192.168.1.100&quot;,
    ///    Port = 22
    ///};
    ///SFTPUploadFile(settings, &quot;./somefile.txt&quot;, &quot;/uploads/somefile.txt&quot;);
    /// </code>
    /// </example>
    /// <param name="cakecontext">The context.</param>
    /// <param name="settings">The settings for the SFTP server.</param>
    /// <param name="localFilePath">The path to the local file you want to upload.</param>
    /// <param name="remoteFilePath">The folder on the SFTP server where you want to upload the file, including the remote filename.</param>
    [CakeMethodAlias]
    public static void SFTPUploadFile(this ICakeContext cakecontext, SFTPSettings settings, string localFilePath,
        string remoteFilePath)
    {
        using var client = new SftpClient(_CreateConnectionInfo(settings));
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
            throw;
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
    /// SFTPDownloadFile(settings, &quot;/uploads/somefile.txt&quot;, &quot;./somefile.txt&quot;);
    /// </code>
    /// </example>
    /// <param name="cakecontext">The context.</param>
    /// <param name="settings">The settings for the SFTP server.</param>
    /// <param name="localFilePath">The path to the local file you want to download.</param>
    /// <param name="remoteFilePath">The folder on the SFTP server where you want to download the file, including the remote filename.</param>
    [CakeMethodAlias]
    public static void SFTPDownloadFile(this ICakeContext cakecontext, SFTPSettings settings, string remoteFilePath,
        string localFilePath)
    {
        using var client = new SftpClient(_CreateConnectionInfo(settings));
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
            throw;
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
    /// SFTPDownloadFiles(settings, aListOfFilePaths);
    /// </code>
    /// </example>
    /// <param name="cakecontext">The context.</param>
    /// <param name="settings">The settings for the SFTP server.</param>
    /// <param name="paths">a list of paths to download</param>
    [CakeMethodAlias]
    public static void SFTPDownloadFiles(this ICakeContext cakecontext, SFTPSettings settings, IEnumerable<SFTPFilePair> paths)
    {
        using var client = new SftpClient(_CreateConnectionInfo(settings));
        try
        {
            client.Connect();

            foreach (var f in paths)
            {
                try
                {
                    using var s = File.Create(f.LocalFilePath);
                    client.DownloadFile(f.RemoteFilePath, s);
                    cakecontext.Log.Write(Verbosity.Normal, LogLevel.Information, "File [{0}] is downloaded.", f.RemoteFilePath);
                }
                catch

                {
                    cakecontext.Log.Write(Verbosity.Normal, LogLevel.Error,
                        "Failed to download the file [{0}]", f.RemoteFilePath);
                    throw;
                }
            }
        }
        finally
        {
            client.Disconnect();
        }
    }

    /// <summary>
    /// Deletes a file on the SFTP server
    /// </summary>
    /// <example>
    /// <code>
    /// SFTPDeleteFile(settings, &quot;/uploads/somefile.txt&quot;);
    /// </code>
    /// </example>
    /// <param name="cakecontext">The context.</param>
    /// <param name="settings">The settings for the SFTP server.</param>
    /// <param name="remoteFilePath">The path to the file on the server you want to delete.</param>
    [CakeMethodAlias]
    public static void SFTPDeleteFile(this ICakeContext cakecontext, SFTPSettings settings, string remoteFilePath)
    {
        using var client = new SftpClient(_CreateConnectionInfo(settings));
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
            throw;
        }
        finally
        {
            client.Disconnect();
        }
    }

    /// <summary>
    /// Deletes files on the SFTP server
    /// </summary>
    /// <example>
    /// <code>
    /// var aListOfRemoteFilePaths = new List&lt;String&gt;{&quot;/uploads/somefileA.txt&quot;, &quot;/uploads/somefileB.txt&quot;};
    /// SFTPDeleteFiles(settings, aListOfRemoteFilePaths);
    /// </code>
    /// </example>
    /// <param name="cakecontext">The context.</param>
    /// <param name="settings">The settings for the SFTP server.</param>
    /// <param name="remoteFilePaths">A list of paths to the files on the server you want to delete.</param>
    [CakeMethodAlias]
    public static void SFTPDeleteFiles(this ICakeContext cakecontext, SFTPSettings settings,
        IEnumerable<String> remoteFilePaths)
    {
        using var client = new SftpClient(_CreateConnectionInfo(settings));
        try
        {
            client.Connect();

            foreach (var f in remoteFilePaths)
            {
                try
                {
                    client.DeleteFile(f);
                    cakecontext.Log.Write(Verbosity.Normal, LogLevel.Information, "File [{0}] is deleted.", f);
                }
                catch

                {
                    cakecontext.Log.Write(Verbosity.Normal, LogLevel.Error,
                        "Failed to delete the file [{0}]", f);
                    throw;
                }
            }
        }
        finally
        {
            client.Disconnect();
        }
    }

    private static ConnectionInfo _CreateConnectionInfo(SFTPSettings settings)
    {
        if (!String.IsNullOrEmpty(settings.KeyFile))
        {
            var ci = new ConnectionInfo(settings.Host, settings.Port, settings.UserName,
                new PrivateKeyAuthenticationMethod(settings.UserName, new PrivateKeyFile(settings.KeyFile)));
            return ci;
        }
        else
        {
            if (!String.IsNullOrEmpty(settings.Key))
            {
                using var keyStream = new MemoryStream(Encoding.UTF8.GetBytes(settings.Key));
                var ci = new ConnectionInfo(settings.Host, settings.Port, settings.UserName,
                    new PrivateKeyAuthenticationMethod(settings.UserName, new PrivateKeyFile(keyStream)));
                return ci;
            }
            else
            {
                var ci = new ConnectionInfo(settings.Host, settings.Port, settings.UserName,
                    new PasswordAuthenticationMethod(settings.UserName, settings.Password));
                return ci;
            }
        }
    }
}
    