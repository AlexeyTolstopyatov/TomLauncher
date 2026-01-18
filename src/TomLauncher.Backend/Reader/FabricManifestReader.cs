using System.Text;
using System.Text.Json;

namespace TomLauncher.Backend.Reader;

public class FabricManifestReader(LoaderType type = LoaderType.Fabric) : IManifestReader
{
    public ManifestGenerals Read(Stream stream)
    {
        var generals = new ManifestGenerals
        {
            LoaderType = type,
            Api = type.ToString(),
            Dependencies = new Dictionary<string, List<ForeignArchiveData>>()
        };
        
        try
        {
            using var reader = new StreamReader(stream, Encoding.UTF8);
            var jsonContent = reader.ReadToEnd();
            
            using var jsonDoc = JsonDocument.Parse(jsonContent);
            var root = jsonDoc.RootElement;
            
            generals.Name = GetStringProperty(root, "id") ?? 
                           "<unknown>";
            
            generals.Title = GetStringProperty(root, "name") ??
                            generals.Name;
            
            var versionStr = GetStringProperty(root, "version");
            if (versionStr != null)
            {
                try
                {
                    generals.Version = Version.FromString(versionStr);
                }
                catch (Exception)
                {
                    Console.WriteLine("bad version: " + versionStr);
                }
                if (versionStr != "not a fabric mod")
                    generals.Version = Version.FromString(versionStr);
            }
            
            ExtractDependencies(root, generals.Dependencies);
            
            var mc = generals.Dependencies["depends"]
                .First(d => d.Mod == "minecraft");
            generals.MinecraftVersion = mc.Version;

            var ld = generals.Dependencies["depends"]
                .First(d => d.Mod == "fabricloader");
            generals.LoaderVersion = ld.Version;

            var api = generals.Dependencies["depends"]
                .First(d => d.Mod is "fabric-api" or "fabric-api-base");
            generals.Api = "fabric-api";
            generals.ApiVersion = api.Version;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"fabric manifest read error: {ex.Message}");
        }
        
        return generals;
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
    
    private string ExtractVersion(string constraint)
    {
        // ">=1.20.1" -> "1.20.1"
        var match = System.Text.RegularExpressions.Regex.Match(
            constraint, 
            @"(\d+\.\d+(?:\.\d+)?)");
        return match.Success 
            ? match.Groups[1].Value 
            : "0";
    }
    
    private void ExtractDependencies(JsonElement root, Dictionary<string, List<ForeignArchiveData>> dependencies)
    {
        var depSections = new[]
        {
            "depends",  // *
            "dependencies", 
            "recommends", 
            "suggests", // *
            "breaks", 
            "conflicts"
        };
        
        foreach (var section in depSections)
        {
            if (!root.TryGetProperty(section, out var deps)) 
                continue;
            
            var depList = new List<ForeignArchiveData>();

            if (deps.ValueKind == JsonValueKind.Object)
            {
                var list = deps.EnumerateObject().ToList();
                depList
                    .AddRange(list
                        .Select(dep => new ForeignArchiveData(dep.Name, Version.FromString(ExtractVersion(dep.Value.GetString()!)))));
            }
            // case JsonValueKind.Array:
            //     depList.AddRange(deps
            //         .EnumerateArray()
            //         .Select(dep => dep.GetString() ?? "<unknown>"));
            //     break;

            dependencies[section] = depList;
        }
    }
}