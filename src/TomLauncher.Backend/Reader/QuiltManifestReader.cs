using System.Text;
using System.Text.Json;

namespace TomLauncher.Backend.Reader;

public class QuiltManifestReader(LoaderType type = LoaderType.Quilt) : IManifestReader
{
    public ManifestGenerals GetManifestGenerals(Stream stream)
    {
        var generals = new ManifestGenerals
        {
            LoaderType = type,
            Api = type.ToString(),
            Dependencies = new Dictionary<string, List<Dependancy>>()
        };
        
        try
        {
            using var reader = new StreamReader(stream, Encoding.UTF8);
            var jsonContent = reader.ReadToEnd();
            
            using var jsonDoc = JsonDocument.Parse(jsonContent);
            var root = jsonDoc.RootElement.GetProperty("quilt_loader");
            
            
            var versionStr = GetStringProperty(root, "version");
            if (versionStr != null)
            {
                generals.Version = Version.FromString(versionStr);
            }

            var metadata = root.GetProperty("metadata");
            
            generals.Name = GetStringProperty(metadata, "id") ?? 
                            "<unknown>";
            
            generals.Title = GetStringProperty(metadata, "name") ??
                             generals.Name;

            
            ExtractDependencies(root, generals.Dependencies);
            
            var mc = generals.Dependencies["depends"]
                .First(d => d.Mod == "minecraft");
            generals.MinecraftVersion = mc.Version;

            var ld = generals.Dependencies["depends"]
                .First(d => d.Mod == "quilt_resource_loader");
            generals.LoaderVersion = ld.Version;

            var api = generals.Dependencies["depends"]
                .First(d => d.Mod is "fabric-api" or "fabric-api-base");
            generals.Api = "fabric-api";
            generals.ApiVersion = api.Version;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Json manifest read error: {ex.Message}");
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
        return match.Success ? match.Groups[1].Value : "0";
    }
    private void ExtractDependencies(JsonElement root, Dictionary<string, List<Dependancy>> dependencies)
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
            
            var depList = new List<Dependancy>();

            if (deps.ValueKind == JsonValueKind.Array)
            {
                var list = deps
                    .EnumerateArray()
                    .ToList();

                depList
                    .AddRange(list
                    .Select(t => new Dependancy(
                        t.GetProperty("id").GetString()!,
                        Version.FromString(ExtractVersion(t.GetProperty("versions").GetString()!)))));
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