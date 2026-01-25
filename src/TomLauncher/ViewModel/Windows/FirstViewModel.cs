using System.Windows.Input;
using Microsoft.Win32;
using TomLauncher.Model;
using Wpf.Ui.Input;

namespace TomLauncher.ViewModel.Windows;

public class FirstViewModel
{
    public SettingsModel Model
    {
        get;
        set;
    }

    public FirstViewModel()
    {
        Model = new SettingsModel();
        BrowseCommand = new RelayCommand<object>(Browse);
    }

    public ICommand BrowseCommand { get; }
    
    private void Browse(object? _)
    {
        var dialog = new OpenFolderDialog
        {
            Multiselect = false
        };
        dialog.ShowDialog();
        
        if (string.IsNullOrEmpty(dialog.FolderName))
            return;

        Model.GameLocation = dialog.FolderName;
    }
}