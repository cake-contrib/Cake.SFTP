using Cake.Core.Tooling;

namespace Cake.SFTP;

/// <summary>
/// All needed settings to connect to the SFTP Server
/// It is important to either use the KeyFile or the Key property, not both. Leave the unused property empty.
/// If you only want to use a username/password combo leave both KeyFile and Key properties empty.
/// </summary>
public class SFTPSettings: ToolSettings 
{
    /// <summary>
    /// The constructor
    /// </summary>
    /// <param name="host">The SFTP Server URL</param>
    /// <param name="port">The port of the SFTP Server, is normally 22</param>
    /// <param name="userName">The User Name on the server</param>
    /// <param name="password">The Password on the server</param>
    /// <param name="keyFile">The path to the private key file on you local computer</param>
    public SFTPSettings(string host, int port, string userName, string password, string keyFile)
    {
        Host = host;
        Port = port;
        UserName = userName;
        Password = password;
        KeyFile = keyFile;
        Key = String.Empty;
    }
    
    /// <summary>
    /// The constructor
    /// </summary>
    /// <param name="host">The SFTP Server URL</param>
    /// <param name="port">The port of the SFTP Server, is normally 22</param>
    /// <param name="userName">The User Name on the server</param>
    /// <param name="password">The Password on the server</param>
    public SFTPSettings(string host, int port, string userName, string password)
    {
        Host = host;
        Port = port;
        UserName = userName;
        Password = password;
        KeyFile = String.Empty;
        Key = String.Empty;
    }
    
    /// <summary>
    /// The constructor
    /// </summary>
    /// <param name="host">The SFTP Server URL</param>
    /// <param name="userName">The User Name on the server</param>
    /// <param name="password">The Password on the server</param>
    public SFTPSettings(string host, string userName, string password)
    {
        Host = host;
        Port = 22;
        UserName = userName;
        Password = password;
        KeyFile = String.Empty;
        Key = String.Empty;
    }
    
    /// <summary>
    /// The constructor
    /// </summary>
    public SFTPSettings()
    {
        KeyFile = String.Empty;
        Host = String.Empty;
        Port = 22;
        UserName = String.Empty;
        Password = String.Empty;
        Key = String.Empty;
    }

    /// <summary>
    /// The SFTP Server URL
    /// </summary>
    public string Host { get; set; }

    /// <summary>
    /// The port of the SFTP Server, is 22 by default
    /// </summary>
    public int Port { get; set; }
    /// <summary>
    /// The User Name on the server
    /// </summary>
    public string UserName { get; set; }
    /// <summary>
    /// The Password on the server
    /// </summary>
    public string Password { get; set; }
    /// <summary>
    /// This is the path to the private key file on you local computer
    /// </summary>
    public string KeyFile { get; set; }
    /// <summary>
    /// This is the string containing the private key
    /// </summary>
    public string Key { get; set; }
}