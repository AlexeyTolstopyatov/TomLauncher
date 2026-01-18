namespace TomLauncher.Backend;

public class ShadersData
{
    /// <summary>
    /// Base filesys object name (shorten)
    /// </summary>
    public FileInfo File
    {
        get; 
        set;
    }

    /// <summary>
    /// Sets as True if shaderpack was imported with same name configuration file
    /// </summary>
    public bool IsConfigured
    {
        get;
        set;
    }
    /// <summary>
    /// If IsConfigured flag is True -> This must be a name of filesys object
    /// which represents .txt setup file 
    /// </summary>
    public string ConfigFileName 
    { 
        get; 
        set;
    } = string.Empty;
}