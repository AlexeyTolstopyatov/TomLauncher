using System.Windows.Input;
using Microsoft.Win32;
using TomLauncher.Model;
using Wpf.Ui.Input;

namespace TomLauncher.ViewModel.Windows;

public class NewPackageViewModel
{
    public NewPackageModel Model { get; private set; }

    public NewPackageViewModel()
    {
        Model = new NewPackageModel();
        BrowseCommand = new RelayCommand<object>(Browse);
    }
    public ICommand BrowseCommand { get; }
    private void Browse(object? _)
    {
        OpenFolderDialog dialog = new()
        {
            Multiselect = false
        };
        dialog.ShowDialog();

        if (string.IsNullOrEmpty(dialog.FolderName))
            return;
        
        Model.Path = dialog.FolderName;
    }
}