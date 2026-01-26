using System.Collections.ObjectModel;
using TomLauncher.Backend;

namespace TomLauncher.Model;

public class PreviewModel
{
    public ObservableCollection<JavaArchiveData>? Mods
    {
        get; init;
    }

    public ObservableCollection<TextureData>? Textures
    {
        get; init;
    }

    public ObservableCollection<ShadersData>? Shaders
    {
        get; init;
    }

    public double DiskCapacityGigaBytes
    {
        get;
    } = App.Disk.Size;

    public double GameCapacityGigaBytes
    {
        get;
    } = App.Disk.GameSize;

    public double GamePercentage
    {
        get;
    } = App.Disk.GameSize / App.Disk.Size * 100;

    public string JavaVersion
    {
        get;
    } = App.JavaVersion!;
}