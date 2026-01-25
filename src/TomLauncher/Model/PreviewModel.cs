using System.Collections.ObjectModel;
using TomLauncher.Backend;

namespace TomLauncher.Model;

public class PreviewModel
{
    public ObservableCollection<JavaArchiveData>? Mods { get; set; }
    public ObservableCollection<TextureData>? Textures { get; set; }
    public ObservableCollection<ShadersData>? Shaders { get; set; }
    public double DiskCapacityGigaBytes { get; set; }
    public double GameCapacityGigaBytes { get; set; }
    public string JavaVersion { get; } = App.JavaVersion!;
}