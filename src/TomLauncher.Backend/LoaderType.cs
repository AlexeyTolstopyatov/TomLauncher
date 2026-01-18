namespace TomLauncher.Backend;

/// <summary>
/// Possible modloader types for this application
/// </summary>
public enum LoaderType
{
    /// <summary>
    /// Unknown
    /// </summary>
    Unknown = 0,
    /// <summary>
    /// https://fabricmc.net/
    /// </summary>
    Fabric = 1,
    /// <summary>
    /// https://www.curseforge.com/
    /// </summary>
    Forge  = 2,
    /// <summary>
    /// https://neoforged.net/
    /// </summary>
    NeoForge = 3,
    /// <summary>
    /// https://quiltmc.org/
    /// </summary>
    Quilt = 4
}