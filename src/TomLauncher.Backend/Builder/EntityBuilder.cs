using System.Text.Json;
using SharpCompress.Archives;
using TomLauncher.Backend.Reader;

namespace TomLauncher.Backend.Builder;

public static class EntityBuilder
{
    /// <summary>
    /// Extracts modification by given path in memory
    /// Selects the registered files and saves data model
    /// using contents of them.
    /// </summary>
    /// <param name="path">
    /// Archive location
    /// </param>
    public static JavaArchiveData FillJavaArchive(string path)
    {
        // Make a Filesys object instance firstly:
        var fileInfo = new FileInfo(path);
        var info = new JavaArchiveData
        {
            File = fileInfo,
            Loader = LoaderType.Unknown,
            Manifests = new Dictionary<LoaderType, ManifestGenerals>()
        };
        // ViewModel layer not denies using of custom named archives
        // But here I want to deny it because many exception chains
        // will influence at current performance.
        if (fileInfo.Extension != ".jar")
            return info;
        // Iterate all recognized filesys entries in opened archive
        // And filter them before the jumping-IL-code starts.
        //
        // (8 filtered entries VS 16038 at all)
        using var archive = ArchiveFactory.Open(fileInfo);
        var entries = archive.Entries
            .Where(e => !e.IsDirectory)
            .Where(e => !e.Key!.Contains('/') || e.Key.StartsWith("META-INF"));
        // Jumping IL code. Time to define the kind of loader embeddings
        // into modification archive. All recognized files are expands the meaning of JavaArchiveData.
        // And this moment very important for high-level calls (View -> VM -> builder -> *backend*).
        foreach (var entry in entries)
        {
            switch (entry.Key)
            {
                // Forged mod defined. Continue stream -> read manifest
                case "META-INF/neoforge.mods.toml":
                    info.Loader = LoaderType.NeoForge;
                    info.Manifests[LoaderType.NeoForge] = new TomlManifestReader(LoaderType.NeoForge)
                        .Read(entry.OpenEntryStream());
                    info.Title = info.Manifests[LoaderType.NeoForge].Title!;
                    info.Name = info.Manifests[LoaderType.NeoForge].Name!;
                    break;
                case "META-INF/mods.toml":
                case "META-INF/forge/mods.toml":
                    info.Loader = LoaderType.Forge;
                    info.Manifests[LoaderType.Forge] = new TomlManifestReader(LoaderType.Forge)
                        .Read(entry.OpenEntryStream());
                    info.Title = info.Manifests[LoaderType.Forge].Title!;
                    info.Name = info.Manifests[LoaderType.Forge].Name!;
                    break;
                // Fabric loader API defined
                case "fabric.mod.json":
                    info.Loader = LoaderType.Fabric;
                    info.Manifests[LoaderType.Fabric] = new FabricManifestReader()
                        .Read(entry.OpenEntryStream());
                    info.Title = info.Manifests[LoaderType.Fabric].Title!;
                    info.Name = info.Manifests[LoaderType.Fabric].Name!;
                    break;
                // Quilt loader API defined
                case "quilt.mod.json":
                    info.Loader = LoaderType.Quilt;
                    info.Manifests[LoaderType.Quilt] = new QuiltManifestReader()
                        .Read(entry.OpenEntryStream());
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
    /// <summary>
    /// Extracts archive of texturepack in memory
    /// and seeks for the .mcmeta package file. Fills
    /// simple TextureData instance, releases archive from memory
    ///
    /// If "pack.mcmeta" is missing - function returns the metadata too
    /// but most of filled properties fill be <c>unknown</c>.
    /// </summary>
    /// <param name="path">
    /// Location of textures archive
    /// </param>
    /// <returns>
    /// First Item -> Content of extracted pack.mcmeta file from textures
    /// Second Item -> Filesystem context of current texturepack
    /// </returns>
    public static TextureData FillResource(string path)
    {
        // Fill the filesys object firstly:
        var fileInfo = new FileInfo(path);
        var textures = new TextureData
        {
            File = fileInfo,
            PackDescription = "<unreachable>",
            PackFormat = 0
        };
        if (fileInfo.Extension != ".zip")
            return textures;
        // Open and filter all recognized entries in archive
        // (expecting 1 file, not 16038)
        using var archive = ArchiveFactory.Open(fileInfo);
        var mcmeta = archive.Entries.Where(e => e.Key == "pack.mcmeta");
        var archiveEntries = mcmeta as IArchiveEntry[] ?? mcmeta.ToArray();
        // If pack.mcmeta is missing, TextureData will be empty but still correct!
        // only information what you will see in View -> FileInfo what filled already.
        if (!archiveEntries.Any())
            return textures;
        // Time to read memory stream and return context at the high-level calls
        var content = new StreamReader(archiveEntries.First().OpenEntryStream()).ReadToEnd();
        try
        {
            var json = JsonDocument.Parse(content);
            var pack = json.RootElement.GetProperty("pack");
            var packFormat = pack.GetProperty("pack_format").GetInt32();
            var packDescription = pack.GetProperty("description").GetString() ?? "?";
            // Fill TexturePack model
            textures.PackDescription = packDescription;
            textures.PackFormat = packFormat;
            return textures;
        }
        catch (Exception e)
        {
            ExceptionWriter.Write(e);
            throw;
        }
    }
    /// <summary>
    /// Don't Extracts archive of shaders in memory
    /// Fills just FileInfo() and seeks for configuration file.
    /// if file is missing -> function returns filled model but
    /// marked this shaders instance as non-configured.
    /// </summary>
    /// <param name="path">
    /// Location of shaders of current mod-pack
    /// </param>
    public static ShadersData FillShaders(string path)
    {
        // Tasks: define configuration file of current shaderpack.
        // C:\path\to\shaderpack.zip --> "C:\path\to\" + "shaderpack" + ".txt"
        var info = new FileInfo(path);
        var conf = Path.GetFileName(path) + ".txt";
        var data = new ShadersData
        {
            File = info
        };
        // I'm seeking for the <selected_shaders>.txt file because
        // Utilities like Iris and Optifine creates special settings file
        // of shaders, which parameters was changed by the user.
        // 
        // If this file is missing -> DataModel stays correct!
        // But ShaderData instance marked as non-configured and View 
        // layer not shows you name of configuration file.
        if (!File.Exists(info.FullName + ".txt")) 
            return data;
        // Remember the base: mark shaders as already configured
        data.ConfigFileName = conf;
        data.IsConfigured = true;
        return data;
    }
}