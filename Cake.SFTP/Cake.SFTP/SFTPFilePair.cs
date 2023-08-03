namespace Cake.SFTP;

/// <summary>
/// this class contains a pair of file paths for both the local as the remote path for that file.
/// </summary>
public class SFTPFilePair
{
    /// <summary>
    /// The constructor
    /// </summary>
    /// <param name="localFilePath">The local path of the file</param>
    /// <param name="remoteFilePath">The SFTP server path of the file</param>
    public SFTPFilePair(string localFilePath, string remoteFilePath)
    {
        LocalFilePath = localFilePath;
        RemoteFilePath = remoteFilePath;
    }

    /// <summary>
    /// The constructor
    /// </summary>
    public SFTPFilePair()
    {
        LocalFilePath = String.Empty;
        RemoteFilePath = String.Empty;
    }

    /// <summary>
    /// The local path of the file
    /// </summary>
    public String LocalFilePath { get; set; }
    /// <summary>
    /// The SFTP server path of the file
    /// </summary>
    public String RemoteFilePath { get; set; }
}