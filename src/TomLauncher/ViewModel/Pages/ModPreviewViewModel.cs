using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using TomLauncher.Backend.Builder;
using TomLauncher.Model;
using Wpf.Ui.Input;

namespace TomLauncher.ViewModel.Pages;
/// <summary>
/// ModPreview ViewModel provides Command and Model bindings to the View.
/// Main tasks: Call OpenFileDialog to provide/write filesys. path of the mod.
/// Selected mod will be read by Backend services and Model layer updates then.
/// </summary>
public class ModPreviewViewModel
{
    public ICommand OpenModCommand { get; }
    public ModPreviewModel Model { get; set; }
    public ModPreviewViewModel()
    {
        Model = new ModPreviewModel();
        Model.ModVisibility = Visibility.Hidden;
        OpenModCommand = new RelayCommand<object>(Open);
    }
    /// <summary>
    /// Calls the OpenFileDialog to provide single filesys Java Archive path
    /// Backend services updates the Model instance here. 
    /// </summary>
    /// <param name="_"></param>
    private void Open(object? _)
    {
        var dialog = new OpenFileDialog
        {
            Multiselect = false,
            Title = "Jar checkout",
            Filter = "Java Minecraft Mods (*.jar)|*.jar"
        };
        dialog.ShowDialog();
        // Update discarded. Model will not show at all. But:
        // If Model initializes by the non-minecraft mod archive -> model
        // shows correctly too. File information will be written.
        // Mod cards and dependencies for each mod card not.
        // Field "EmbeddedLoaders" will be set to 0. It says that current Jar is not Minecraft mod.
        if (string.IsNullOrEmpty(dialog.FileName)) 
            return;
        // Reads JavaArchive, extracts the Manifest files in Application Memory.
        // Makes shorten report -> writes to Dictionary<TLoader, TManifest>
        Model.Mod = EntityBuilder.FillJavaArchive(dialog.FileName);
        // Expands hidden XAML elements (Modification Card)
        // When Model is ready, there's no fallbacks, and the model will be shown correctly
        // The Star (*) keyword in the Model strings called "похуй-keyword" what means "Any or Nothing"
        // The Question says "Undefined" 
        Model.ModVisibility = Visibility.Visible;
    }
}