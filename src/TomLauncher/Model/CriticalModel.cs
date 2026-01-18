using System.IO;

namespace TomLauncher.Model;

public class CriticalModel
{
    public FileInfo? CrashInfo { get; set; }
    public string? CrashLog { get; set; }
}