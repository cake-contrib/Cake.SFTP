using Cake.Core.Tooling;

namespace Cake.SFTP;

/// <summary>
/// All needed settings to connect to the SFTP Server
/// </summary>
public class SFTPSettings: ToolSettings
{
    /// <summary>
    /// The SFTP URL
    /// </summary>
    public string Host { get; set; } = string.Empty;

    /// <summary>
    /// The port of the SFTP Server, is 22 by default
    /// </summary>
    public int Port { get; set; } = 22;
    /// <summary>
    /// The User Name on the server
    /// </summary>
    public string UserName { get; set; } = string.Empty;
    /// <summary>
    /// The Password on the server
    /// </summary>
    public string Password { get; set; } = string.Empty;
}