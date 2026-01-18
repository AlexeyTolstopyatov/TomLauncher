using System.Text.Json;
using Tomlyn;
using Tomlyn.Model;

namespace TomLauncher.Backend.Builder;

/// <summary>
/// The ModPack builder creates project structure what clones ".minecraft" directory.
/// The ModPackBuilder must have all rights only for target directory and no more.
/// </summary>
public class PackageBuilder
{
    private string _path;
    /// <summary>
    /// Supported kinds of importing objects
    /// into current mod-pack workspace 
    /// </summary>
    public enum ArtifactKind
    {
        Mod,
        Resource,
        Shaders
    }
    public string Mods { get; }
    /// <summary>
    /// Location of textures/resources of current mod-pack
    /// </summary>
    public string ResourcePacks
    {
        get;
    }
    /// <summary>
    /// Location of shaders of current mod-pack
    /// </summary>
    public string ShaderPacks
    {
        get; 
    }
    /// <summary>
    /// Location of current mod-pack
    /// </summary>
    public string Root
    {
        get;
    }
    /// <summary>
    /// The Writer method:
    /// 
    /// Creates catalog and automatically makes workspace.
    /// Throws exceptions if something went wrong!
    /// </summary>
    /// <param name="path">Path</param>
    /// <param name="name">Name</param>
    /// <param name="isRoot">Save project by target path (true) or target path\name (false)</param>
    public PackageBuilder(string path, string name, bool isRoot)
    {
        var info = isRoot 
            ? new DirectoryInfo(path) 
            : new DirectoryInfo($"{path}\\{name}");

        info.CreateSubdirectory("mods");
        info.CreateSubdirectory("resourcepacks");
        info.CreateSubdirectory("shaderpacks");

        Mods = info.FullName + "\\mods";
        ResourcePacks = info.FullName + "\\resourcepacks";
        ShaderPacks = info.FullName + "\\shaderpacks";

        _path = info.FullName;
        Root = _path;
    }
    /// <summary>
    /// The Reader method
    ///
    /// Uses already filled TOML model and prepares output
    /// directories locations for editing current mod-pack.
    /// </summary>
    /// <param name="path"></param>
    /// <exception cref="InvalidDataException"></exception>
    public PackageBuilder(string path)
    {
        _path = path;
        Mods = _path + "\\mods";
        ResourcePacks = _path + "\\resourcepacks";
        ShaderPacks = _path + "\\shaderpacks";

        Root = _path;
        
        if (!File.Exists($"{_path}\\header.toml"))
            throw new InvalidDataException("This is not mod-pack directory");
    }
    /// <summary>
    /// Copies the filesystem entry into project folder
    /// </summary>
    /// <param name="path">Full Path to the object</param>
    /// <param name="kind">Kind of current object</param>
    public void Include(string path, ArtifactKind kind)
    {
        var info = new FileInfo(path);
        switch (kind)
        {
            case ArtifactKind.Mod:
                File.Copy(path, $"{Mods}\\{info.Name}");
                break;
            case ArtifactKind.Resource:
                File.Copy(path, $"{ResourcePacks}\\{info.Name}");
                break;
            case ArtifactKind.Shaders:
                File.Copy(path, $"{ShaderPacks}\\{info.Name}");
                break;
        }
        
    }
    /// <summary>
    /// Builds the Modification DataModel for prepared modpack
    /// </summary>
    /// <param name="path">Path to the current object</param>
    /// <param name="target">
    /// Filtering loader type
    /// (maybe moved to constructor methods at the next time)
    /// </param>
    public JavaArchiveData GetMod(string path, LoaderType target)
    {
        try
        {
            var m = ModificationBuilder.FillJavaArchive(path);
            if (m.Manifests.ContainsKey(target))
            {
                m.Title = m.Manifests[target].Title!;
                m.Name = m.Manifests[target].Name!;
                m.Loader = target;
            }
            else
            {
                Exclude(path);
            }
            return m;
        }
        catch (Exception e)
        {
            ExceptionWriter.Write(e);
            throw;
        }
    }
    /// <summary>
    /// Builds the Texture data model for current mod-pack 
    /// </summary>
    /// <param name="path">
    /// Location of current textures archive
    /// </param>
    public TextureData GetResource(string path)
    {
        var (content, info) = ModificationBuilder.FillResource(path);
        try
        {
            var json = JsonDocument.Parse(content);
            var pack = json.RootElement.GetProperty("pack");

            var packFormat = pack.GetProperty("pack_format").GetInt32();
            var packDescription = pack.GetProperty("description").GetString();
            
            return new TextureData
            {
                PackFormat = packFormat,
                PackDescription = packDescription!,
                File = info
            };
        }
        catch (Exception e)
        {
            ExceptionWriter.Write(e);
            throw;
        }
    }
    /// <summary>
    /// Builds the Shaders data model for current mod-pack 
    /// </summary>
    /// <param name="path">
    /// Location of current shaders archive
    /// </param>
    public ShadersData GetShaders(string path)
    {
        try
        {
            return ModificationBuilder.FillShaders(path);
        }
        catch (Exception e)
        {
            ExceptionWriter.Write(e);
            throw;
        }
    }
    /// <summary>
    /// Delete FS Object from project
    /// </summary>
    /// <param name="name">FullName of filesys object</param>
    public void Exclude(string name)
    {
        File.Delete(name);
    }
    /// <summary>
    /// Write file header of modpack
    /// </summary>
    public void Write(PackageData p)
    {
        // warn: This is the fastest way to make simple table & forget it
        var tab = $"name=\"{p.Name}\"\r\nloaderId=\"{(int)p.Loader!.Type}\"\r\nloaderVersion=\"{p.Loader}\"\r\nminecraftVersion=\"{p.Minecraft}\"";
        File.WriteAllText($"{_path}\\header.toml", tab);
    }
    /// <summary>
    /// Every (Toml)auncher mod-pack must contain header
    /// of the project. This is a TOML table "header.toml"
    /// which contains current mod-pack metadata
    ///
    /// This method reads and returns ready-to-use
    /// datamodel or current mod-pack depending on TOML header.
    /// </summary>
    public PackageData Read()
    {
        var tab = File.ReadAllText($"{_path}\\header.toml");
        var model = Toml.ToModel(tab);
        var name = GetTomlValue(model, "name");
        var loaderType = (LoaderType)int.Parse(GetTomlValue(model, "loaderId")!);
        var loaderVersion = GetTomlValue(model, "loaderVersion");
        var gameVersion = GetTomlValue(model, "minecraftVersion");
        var package = new PackageData(name!)
        {
            Loader = new LoaderData(loaderType, loaderVersion),
            Minecraft = Version.FromString(gameVersion)
        };

        return package;
    }
    #region Private methodology
    private string? GetTomlValue(TomlObject obj, string key)
    {
        if (obj is TomlTable table && table.TryGetValue(key, out var value))
        {
            return value.ToString();
        }
        return null;
    }
    #endregion
}