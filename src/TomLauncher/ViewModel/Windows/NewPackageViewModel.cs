using System.Windows.Input;
using Microsoft.Win32;
using TomLauncher.Model;
using Wpf.Ui.Input;

namespace TomLauncher.ViewModel.Windows;

/// <summary>
/// NewPackageWindow middleware. Contains command and data
/// bindings for the NewPackage view.
///
/// Main tasks of NewPackageViewModel: represent Browse, Create commands
/// and bindings to expected ModPack model at the EditorPage VM.
/// </summary>
public class NewPackageViewModel
{
    public NewPackageModel Model { get; }
    
    public NewPackageViewModel()
    {
        Model = new NewPackageModel();
        BrowseCommand = new RelayCommand<object>(Browse);
    }
    public ICommand BrowseCommand { get; }
    /// <summary>
    /// Calls OpenFolderDialog to provide path to Directory where game directories
    /// will be created.
    /// Next stage of FolderName will be the _builder? instance in the EditorPage VM.
    /// When flag "Mark as root" is checked -> No nested project folder will be created!
    ///
    /// _modPackName = goblincore
    /// _folderName  = C:\ModPacks\
    /// _markAsRoot  = true  -> [_builder.Path] C:\ModPacks\
    /// _markAsRoot  = false -> [_builder.Path] C:\ModPacks\goblincore
    /// </summary>
    /// <param name="_">
    /// Unused
    /// </param>
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