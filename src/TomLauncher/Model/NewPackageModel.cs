using TomLauncher.Backend;
using TomLauncher.ViewModel;
using Version = TomLauncher.Backend.Version;

namespace TomLauncher.Model;

public class NewPackageModel : NotifyPropertyChanged
{
    private string? _name;
    private string? _path;
    private string? _minecraftString;
    private string _loaderType = nameof(TomLauncher.Backend.LoaderType.Unknown);
    private string? _loaderString;
    private bool _markAsRoot;
    /// <summary>
    /// The "Modpack Name". or "Folder name" of current ModPack
    /// </summary>
    public string? Name
    {
        get => _name;
        set => SetField(ref _name, value);
    }
    /// <summary>
    /// Location of the current ModPack
    /// </summary>
    public string? Path
    {
        get => _path; 
        set => SetField(ref _path, value);
    }
    /// <summary>
    /// Link to the game version
    /// </summary>
    private Version Minecraft => Version.FromString(MinecraftString);
    /// <summary>
    /// String with game version.
    /// </summary>
    public string? MinecraftString
    {
        get => _minecraftString;
        set => SetField(ref _minecraftString, value);
    }
    /// <summary>
    /// String with loader version
    /// </summary>
    public string? LoaderString
    {
        get => _loaderString;
        set => SetField(ref _loaderString, value);
    }
    /// <summary>
    /// Serialized possible types of modloader
    /// </summary>
    public Array LoaderTypes
    {
        get;
    } = Enum.GetValues(typeof(LoaderType));
    /// <summary>
    /// Serialized current modloader name
    /// </summary>
    public string LoaderType 
    { 
        get => _loaderType; 
        set => SetField(ref _loaderType, value); 
    }
    /// <summary>
    /// Link to the prepared LoaderData instance
    /// </summary>
    private LoaderData Loader => 
        new(Enum.Parse<LoaderType>(LoaderType), LoaderString);
    /// <summary>
    /// If flag set -> Disable Name Textbox and send data to builder
    /// </summary>
    public bool MarkAsRoot
    {
        get => _markAsRoot;
        set => SetField(ref _markAsRoot, value);
    }
    /// <summary>
    /// Safe cast of current NewPackageModel to prepared PackageData model
    /// </summary>
    public PackageData ToPackageData => new(Name!)
    {
        Mods = [],
        Minecraft = Minecraft,
        Loader = Loader,
        Size = 0
    };
}