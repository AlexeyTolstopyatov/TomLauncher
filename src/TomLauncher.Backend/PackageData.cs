using System.Collections.ObjectModel;

namespace TomLauncher.Backend;

public class PackageData(string name)
{
    /// <summary>
    /// Name of current package
    /// </summary>
    public string Name
    {
        get; 
        set;
    } = name;
    /// <summary>
    /// Size of current package
    /// </summary>
    public float Size
    {
        get; 
        set;
    }
    /// <summary>
    /// Global loader type declared for each mod in collection
    /// By this flag occurs incompatible mods and excludes from collection.
    /// By this flag, same mods appends to collection and their data shows.
    /// 
    /// (Mod may contain 2 embedded loaders but mod default data model will be built
    /// using LoaderType key)
    /// </summary>
    public LoaderData? Loader
    {
        get; 
        set;
    }
    /// <summary>
    /// By this version flag the check occurs incompatible mods
    /// and marks them as yellow cards in View.
    /// </summary>
    public Version? Minecraft
    {
        get; 
        set;
    }
    /// <summary>
    /// Main collection of mods what will be placed in $Name$/mods directory
    /// </summary>
    public ObservableCollection<JavaArchiveData> Mods
    {
        get; 
        set;
    } = [];
    /// <summary>
    /// Main collection of resources which appends to the game
    /// and will be placed in $Name$/resourcepacks directory
    /// </summary>
    public ObservableCollection<TextureData> Textures
    {
        get; 
        set;
    } = [];
    /// <summary>
    /// Main collection of shaders which appends to the game
    /// and will be placed in $Name$/shaderpacks directory
    /// </summary>
    public ObservableCollection<ShadersData> Shaders
    {
        get; 
        set;
    } = [];
}