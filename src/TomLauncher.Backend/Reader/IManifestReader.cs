namespace TomLauncher.Backend.Reader;
/// <summary>
/// Shorten report about extracted manifest file of
/// current game modification. Uses by Editor and ModPreview VMs
/// to recognize mod-compatibility and following dependencies
/// </summary>
public class ManifestGenerals
{
    /// <summary>
    /// This String is name of global namespace
    /// Usually this string contains just letters and no spaces
    /// or numbers. This is a "Project Name". 
    /// </summary>
    public string? Name
    {
        get;
        set;
    }
    /// <summary>
    /// This String shows in mods-menu in started game.
    /// </summary>
    public string? Title
    {
        get; 
        set;
    }
    /// <summary>
    /// Mod version. May be nullable
    /// </summary>
    public Version? Version
    {
        get; 
        set;
    }
    /// <summary>
    /// This string is the Name of modloader API.
    /// For the Neo/Forge modloaders, embedded manifest
    /// files contains "javafml" or "lowcodefml" strings
    /// (where fml is shorten "Forge Mod Loader").
    /// 
    /// For Fabric manifest this field may be "fabric-api" or "fabric-api-base"
    /// depending on already built modification.
    /// </summary>
    public string? Api { get; set; }
    /// <summary>
    /// Modloader API version
    /// </summary>
    public Version? ApiVersion
    {
        get;
        set;
    }
    /// <summary>
    /// Loader Version
    /// </summary>
    public Version? LoaderVersion
    {
        get;
        set;
    }
    /// <summary>
    /// One of five possible types
    /// </summary>
    public LoaderType LoaderType
    {
        get; 
        set;
    }
    /// <summary>
    /// Game version. Usually stores in mod dependencies
    /// </summary>
    public Version? MinecraftVersion 
    { 
        get; 
        set;
    }
    /// <summary>
    /// Dependencies of current mod. In most cases contain just loader client-side
    /// version and version of the game. But some mods use foreign libraries which
    /// also built in Java archives with the same manifests. 
    /// </summary>
    public Dictionary<string, List<ForeignArchiveData>>? Dependencies
    {
        get; 
        init;
    }
}
/// <summary>
/// Constraint for following next readers.
/// </summary>
public interface IManifestReader
{
    /// <summary>
    /// Reads stream of extracted mod manifest file.
    /// </summary>
    /// <param name="stream">
    /// Stream of chosen file
    /// </param>
    /// <returns>
    /// Shorten manifest structure of given mod
    /// </returns>
    ManifestGenerals Read(Stream stream);
}