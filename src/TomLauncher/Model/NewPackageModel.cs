using TomLauncher.Backend;
using TomLauncher.ViewModel;
using Version = TomLauncher.Backend.Version;

namespace TomLauncher.Model;

public class NewPackageModel : NotifyPropertyChanged
{
    private string? _name;
    private string? _path;
    private string? _minecraftString;
    private string _loaderType = nameof(TomLauncher.Backend.LoaderType.Undeclared);
    private string? _loaderString;
    
    public string? Name
    {
        get => _name;
        set => SetField(ref _name, value);
    }

    public string? Path
    {
        get => _path; 
        set => SetField(ref _path, value);
    }

    public Version Minecraft => Version.FromString(MinecraftString);
    
    public string? MinecraftString
    {
        get => _minecraftString;
        set => SetField(ref _minecraftString, value);
    }

    public string? LoaderString
    {
        get => _loaderString;
        set => SetField(ref _loaderString, value);
    }
    public Array LoaderTypes { get; } = Enum.GetValues(typeof(LoaderType));
    public string LoaderType 
    { 
        get => _loaderType; 
        set => SetField(ref _loaderType, value); 
    }
    public LoaderData Loader => new(Enum.Parse<LoaderType>(LoaderType), LoaderString);

    public PackageData ToPackageData => new(Name!)
    {
        Mods = [],
        Minecraft = Minecraft,
        Loader = Loader,
        Size = 0
    };
}