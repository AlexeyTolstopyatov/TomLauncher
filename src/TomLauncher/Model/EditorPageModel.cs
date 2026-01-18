using System.Windows;
using TomLauncher.Backend;
using TomLauncher.ViewModel;

namespace TomLauncher.Model;

public class EditorPageModel : NotifyPropertyChanged
{
    private Visibility _packageOpened = Visibility.Collapsed;
    private PackageData? _packageData;
    private bool _isExportEnabled;
    public PackageData? Package
    {
        get => _packageData; 
        set => SetField(ref _packageData, value);
    }

    public Visibility PackageOpened
    {
        get => _packageOpened; 
        set => SetField(ref _packageOpened, value);
    }

    public bool IsExportEnabled
    {
        get => _isExportEnabled; 
        set => SetField(ref _isExportEnabled, value);
    }
}