using System.Text.Json;
using Tomlyn;
using Tomlyn.Model;

namespace TomLauncher.Backend.Builder;

/// <summary>
/// The ModPack builder creates project structure what clones ".minecraft" directory.
/// The ModPackBuilder must have all rights only for target directory and no more.
/// </summary>
public class ModPackBuilder
{
    private string _path;
    public enum ArtifactKind
    {
        Mod,
        Resource,
        Shaders
    }
    public string Mods { get; }
    public string ResourcePacks { get; }
    public string ShaderPacks { get; }
    public string Root { get; }

    /// <summary>
    /// Creates catalog and automatically makes workspace.
    /// Throws exceptions if something went wrong!
    /// </summary>
    /// <param name="path">Path</param>
    /// <param name="name">Name</param>
    /// <param name="isRoot">Save project by target path (true) or target path\name (false)</param>
    public ModPackBuilder(string path, string name, bool isRoot)
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

    public ModPackBuilder(string path)
    {
        _path = path;
        Mods = _path + "\\mods";
        ResourcePacks = _path + "\\resourcepacks";
        ShaderPacks = _path + "\\shaderpacks";

        Root = _path;
        
        if (!File.Exists($"{_path}\\header.toml"))
            throw new InvalidDataException("This is not modpack directory");
    }

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

    public JavaArchiveData GetMod(string path, LoaderType target)
    {
        try
        {
            var m = ModModelBuilder.FillJavaArchive(path);
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

    public TextureData GetResource(string path)
    {
        var (content, info) = ModModelBuilder.FillResource(path);
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

    public ShadersData GetShaders(string path)
    {
        try
        {
            return ModModelBuilder.FillShaders(path);
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

    public PackageData Read()
    {
        var tab = File.ReadAllText($"{_path}\\header.toml");
        var model = Toml.ToModel(tab);
        var name = GetStringValue(model, "name");
        var loaderType = (LoaderType)int.Parse(GetStringValue(model, "loaderId")!);
        var loaderVersion = GetStringValue(model, "loaderVersion");
        var gameVersion = GetStringValue(model, "minecraftVersion");
        var package = new PackageData(name!)
        {
            Loader = new LoaderData(loaderType, loaderVersion),
            Minecraft = Version.FromString(gameVersion)
        };

        return package;
    }
    
    private string? GetStringValue(TomlObject obj, string key)
    {
        if (obj is TomlTable table && table.TryGetValue(key, out var value))
        {
            return value.ToString();
        }
        return null;
    }
    
    private string? GetStringProperty(JsonElement element, string propertyName)
    {
        if (element.TryGetProperty(propertyName, out var prop) && 
            prop.ValueKind == JsonValueKind.String)
        {
            return prop.GetString();
        }
        return null;
    }
}