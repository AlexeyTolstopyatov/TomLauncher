namespace TomLauncher.Backend.Reader;

public class ManifestGenerals
{
    public string? Name { get; set; }
    public string? Title { get; set; }
    public Version? Version { get; set; }

    /// <summary>
    /// (Neo)Forge mods may contain "lowcodefml" or "javafml"
    /// This string are stores them
    /// </summary>
    public string? Api { get; set; }
    /// <summary>
    /// API version
    /// </summary>
    public Version? ApiVersion { get; set; }
    public Version? LoaderVersion { get; set; }
    public LoaderType LoaderType { get; set; }
    public Version? MinecraftVersion { get; set; }
    public Dictionary<string, List<Dependancy>>? Dependencies { get; init; }
}

public interface IManifestReader
{
    ManifestGenerals GetManifestGenerals(Stream stream);
}