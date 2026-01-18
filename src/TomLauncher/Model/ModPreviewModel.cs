using System.Windows;
using TomLauncher.Backend;
using TomLauncher.ViewModel;

namespace TomLauncher.Model;

public class ModPreviewModel : NotifyPropertyChanged
{
    /// <summary>
    /// Mod model.
    /// </summary>
    private JavaArchiveData? _mod;
    /// <summary>
    /// If archive filename not empty and Mod model
    /// are filled correctly -> changes to Visible
    /// </summary>
    private Visibility _modVisibility;
    /// <summary>
    /// Smart pointer to the modification model.
    /// Dynamically updates in View and Model
    /// </summary>
    public JavaArchiveData? Mod
    {
        get => _mod;
        set => SetField(ref _mod, value);
    }
    /// <summary>
    /// Smart pointer to the visibility flag
    /// Dynamically updates.
    /// </summary>
    public Visibility ModVisibility
    {
        get => _modVisibility;
        set => SetField(ref _modVisibility, value);
    }
}