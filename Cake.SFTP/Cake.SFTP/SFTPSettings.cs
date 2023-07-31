using Cake.Core.Tooling;

namespace Cake.SFTP;

public class SFTPSettings: ToolSettings
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}