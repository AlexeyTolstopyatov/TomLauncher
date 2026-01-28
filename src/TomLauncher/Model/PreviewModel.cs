using System.Collections.ObjectModel;
using TomLauncher.Backend;

namespace TomLauncher.Model;

public class PreviewModel
{
    /// <summary>
    /// Modifications metadata collection
    /// </summary>
    public ObservableCollection<JavaArchiveData>? Mods
    {
        get; 
        init;
    }
    /// <summary>
    /// Texture-packs metadata collection
    /// </summary>
    public ObservableCollection<TextureData>? Textures
    {
        get; init;
    }
    /// <summary>
    /// Shader-packs metadata collection
    /// </summary>
    public ObservableCollection<ShadersData>? Shaders
    {
        get; init;
    }
    /// <summary>
    /// Free space on the disk where game is located
    /// </summary>
    public double DiskFreeSpaceGigaBytes
    {
        get;
    } = App.Disk.Size;
    /// <summary>
    /// Game filled disk space
    /// </summary>
    public double GameSpaceGigaBytes
    {
        get;
    } = App.Disk.GameSize;
    /// <summary>
    /// The percentage of how many gigabytes the game
    /// catalog occupies on the disk
    /// </summary>
    public double GamePercentage
    {
        get;
    } = App.Disk.GameSize / App.Disk.Size * 100;
    /// <summary>
    /// Version of registered Java components 
    /// </summary>
    public string JavaVersion
    {
        get;
    } = App.JavaVersion!;
}