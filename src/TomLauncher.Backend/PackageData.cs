using System.Collections.ObjectModel;

namespace TomLauncher.Backend;

public class PackageData(string name)
{
    
    public string Name { get; set; } = name;
    public float Size { get; set; }
    
    public LoaderData? Loader { get; set; }
    public Version? Minecraft { get; set; }
    public ObservableCollection<JavaArchiveData> Mods { get; set; } = [];
    public ObservableCollection<TextureData> Resources { get; set; } = [];
    public ObservableCollection<ShadersData> Shaders { get; set; } = [];
}