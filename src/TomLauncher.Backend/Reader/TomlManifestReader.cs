using System.Text;
using Tomlyn;
using Tomlyn.Model;

namespace TomLauncher.Backend.Reader;

public class TomlManifestReader(LoaderType type) : IManifestReader
{
    public ManifestGenerals GetManifestGenerals(Stream stream)
    {
        var generals = new ManifestGenerals
        {
            LoaderType = type,
            Dependencies = new Dictionary<string, List<Dependancy>>(),
        };

        try
        {
            using var reader = new StreamReader(stream, Encoding.UTF8);
            var tomlContent = reader.ReadToEnd();
            
            var model = Toml.ToModel(tomlContent);
            
            if (model.TryGetValue("mods", out var m))
            {
                var mods = (TomlTableArray)m;
                if (mods.Count > 0)
                {
                    var firstMod = mods[0];
                    
                    generals.Name = GetStringValue(firstMod, "modId")!;
                    generals.Title = GetStringValue(firstMod, "displayName") ?? generals.Name;
                    
                    var versionStr = GetStringValue(firstMod, "version");
                    if (versionStr != null)
                    {
                        // Prepare Version
                        versionStr = CleanVersionString(versionStr);
                        generals.Version = Version.FromString(versionStr);
                    }
                }
            }
            
            // Loader Information
            generals.Api = GetStringValue(model, "modLoader") ?? "<javafml>";
            
            // Get the loader version
            var loaderVersionStr = GetStringValue(model, "loaderVersion");
            if (loaderVersionStr != null)
            {
                var match = System.Text.RegularExpressions.Regex.Match(loaderVersionStr, @"\d+");
                if (match.Success)
                {
                    generals.ApiVersion = Version.FromString(match.Value);
                }
            }
            
            ExtractDependencies(model, generals.Dependencies);

            var mc = generals.Dependencies[generals.Name!].First(d => d.Mod == "minecraft").Version;
            generals.MinecraftVersion = mc;

            var ld = generals.Dependencies[generals.Name!].First(l => l.Mod == type.ToString().ToLowerInvariant())
                .Version;
            generals.LoaderVersion = ld;
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Toml manifest read error: {ex.Message}");
        }
        
        return generals;
    }
    
    private string? GetStringValue(TomlObject obj, string key)
    {
        if (obj is TomlTable table && table.TryGetValue(key, out var value))
        {
            return value.ToString();
        }
        return null;
    }
    
    private string CleanVersionString(string version)
    {
        // Removing ${file.jarVersion}
        return System.Text.RegularExpressions.Regex.Replace(
            version, 
            @"\$\{[^}]+\}", 
            "0");
    }
    
    private string ExtractVersion(string range)
    {
        // Translate "[1.20.1,1.21)" -> "1.20.1"
        var match = System.Text.RegularExpressions.Regex.Match(range, @"(\d+\.\d+(?:\.\d+)?)");
        return match.Success ? match.Groups[1].Value : "0";
    }
    
    private void ExtractDependencies(TomlTable model, Dictionary<string, List<Dependancy>> dependencies)
    {
        if (!model.TryGetValue("dependencies", out var d)) return;
        
        var depsTable = (TomlTable)d;
        foreach (var depKey in depsTable.Keys)
        {
            if (depsTable[depKey] is not TomlTableArray depArray) 
                continue;
            
            var depList = new List<Dependancy>();
            foreach (var dep in depArray)
            {
                var modId = GetStringValue(dep, "modId");
                var versionRange = GetStringValue(dep, "versionRange");
                if (modId != null)
                {
                    depList.Add(new Dependancy(modId, Version.FromString(ExtractVersion(versionRange!))));
                }
            }
            dependencies[depKey] = depList;
        }
    }
}