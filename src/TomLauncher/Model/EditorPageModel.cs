using System.Windows;
using TomLauncher.Backend;
using TomLauncher.ViewModel;

namespace TomLauncher.Model;

public class EditorPageModel : NotifyPropertyChanged
{
    private Visibility _packageOpened = Visibility.Collapsed;
    private PackageData? _packageData;
    private bool _isExportEnabled;
    /// <summary>
    /// Given by NewPackage model or deserialized
    /// through the ModPack Opening prepared instance.
    /// </summary>
    public PackageData? Package
    {
        get => _packageData; 
        set => SetField(ref _packageData, value);
    }
    /// <summary>
    /// Sets Visible if PackageData model initialized
    /// </summary>
    public Visibility PackageOpened
    {
        get => _packageOpened;
        set => SetField(ref _packageOpened, value);
    }
    /// <summary>
    /// Sets enabled at once, when package is initialized.
    /// If "Export" button is pressed -> this flag sets "false"
    /// to avoid the overwrite problem.
    /// </summary>
    public bool IsExportEnabled
    {
        get => _isExportEnabled; 
        set => SetField(ref _isExportEnabled, value);
    }
}