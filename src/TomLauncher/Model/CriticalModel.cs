using System.IO;

namespace TomLauncher.Model;

public class CriticalModel
{
    /// <summary>
    /// Smart pointer to the crach log
    /// Not updates.
    /// </summary>
    public FileInfo? CrashInfo
    {
        get; 
        set;
    }
    /// <summary>
    /// Smart pointer to the file contents
    /// Not updates.
    /// </summary>
    public string? CrashLog
    {
        get; 
        set;
    }
}