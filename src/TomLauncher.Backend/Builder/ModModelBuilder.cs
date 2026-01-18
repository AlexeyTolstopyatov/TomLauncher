using SharpCompress.Archives;
using TomLauncher.Backend.Reader;

namespace TomLauncher.Backend.Builder;

public static class ModModelBuilder
{
    public static JavaArchiveData FillJavaArchive(string path)
    {
        var fileInfo = new FileInfo(path);
        var info = new JavaArchiveData
        {
            File = fileInfo,
            Loader = LoaderType.Undeclared,
            Manifests = new()
        };
        
        if (fileInfo.Extension != ".jar")
            return info;

        using var archive = ArchiveFactory.Open(fileInfo);

        var entries = archive.Entries
            .Where(e => !e.IsDirectory)
            .Where(e => !e.Key!.Contains('/') || e.Key.StartsWith("META-INF"));
        
        foreach (var entry in entries)
        {
            switch (entry.Key)
            {
                // Forged mod defined. Continue stream -> read manifest
                case "META-INF/neoforge.mods.toml":
                    info.Loader = LoaderType.NeoForge;
                    info.Manifests[LoaderType.NeoForge] = new TomlManifestReader(LoaderType.NeoForge)
                        .GetManifestGenerals(entry.OpenEntryStream());
                    info.Title = info.Manifests[LoaderType.NeoForge].Title!;
                    info.Name = info.Manifests[LoaderType.NeoForge].Name!;
                    break;
                case "META-INF/mods.toml":
                case "META-INF/forge/mods.toml":
                    info.Loader = LoaderType.Forge;
                    info.Manifests[LoaderType.Forge] = new TomlManifestReader(LoaderType.Forge)
                        .GetManifestGenerals(entry.OpenEntryStream());
                    info.Title = info.Manifests[LoaderType.Forge].Title!;
                    info.Name = info.Manifests[LoaderType.Forge].Name!;
                    break;
                // Fabric loader API defined
                case "fabric.mod.json":
                    info.Loader = LoaderType.Fabric;
                    info.Manifests[LoaderType.Fabric] = new FabricManifestReader()
                        .GetManifestGenerals(entry.OpenEntryStream());
                    info.Title = info.Manifests[LoaderType.Fabric].Title!;
                    info.Name = info.Manifests[LoaderType.Fabric].Name!;
                    break;
                // Quilt loader API defined
                case "quilt.mod.json":
                    info.Loader = LoaderType.Quilt;
                    info.Manifests[LoaderType.Quilt] = new QuiltManifestReader()
                        .GetManifestGenerals(entry.OpenEntryStream());
                    info.Title = info.Manifests[LoaderType.Quilt].Title!;
                    info.Name = info.Manifests[LoaderType.Quilt].Name!;
                    break;
                default:
                    // nothing here :D
                    continue;
            }
        }
        
        return info;
    }

    public static (string, FileInfo) FillResource(string path)
    {
        var fileInfo = new FileInfo(path);
        if (fileInfo.Extension != ".zip")
            return ("<unknown>", fileInfo);

        using var archive = ArchiveFactory.Open(fileInfo);
        var mcmeta = archive.Entries.Where(e => e.Key == "pack.mcmeta");
        var archiveEntries = mcmeta as IArchiveEntry[] ?? mcmeta.ToArray();
        
        if (!archiveEntries.Any())
            return ("<unknown>", fileInfo);

        var content = new StreamReader(archiveEntries.First().OpenEntryStream()).ReadToEnd();
        return (content, fileInfo);
    }

    public static ShadersData FillShaders(string path)
    {
        var info = new FileInfo(path);
        var conf = Path.GetFileNameWithoutExtension(path) + ".txt";
        var data = new ShadersData
        {
            File = info
        };

        if (File.Exists(conf))
        {
            data.ConfigFileName = conf;
            data.IsConfigured = true;
        }

        return data;
    }
}