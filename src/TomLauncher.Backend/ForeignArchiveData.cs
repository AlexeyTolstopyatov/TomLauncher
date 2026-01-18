namespace TomLauncher.Backend;

public sealed class ForeignArchiveData(string m, Version v)
{
    /// <summary>
    /// Name of foreign library
    /// </summary>
    public string Mod 
    { 
        get;
    } = m;
    /// <summary>
    /// Version of foreign library
    /// </summary>
    public Version Version
    {
        get; 
    } = v;
}