using System.Windows;
using TomLauncher.Backend;
using TomLauncher.ViewModel;

namespace TomLauncher.Model;

public class ModPreviewModel : NotifyPropertyChanged
{
    private JavaArchiveData? _mod;
    private Visibility _modVisibility;
    public JavaArchiveData? Mod
    {
        get => _mod;
        set => SetField(ref _mod, value);
    }

    public Visibility ModVisibility
    {
        get => _modVisibility;
        set => SetField(ref _modVisibility, value);
    }
}