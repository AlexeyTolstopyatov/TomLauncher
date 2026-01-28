using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TomLauncher.Backend;
using TomLauncher.Backend.Builder;
using TomLauncher.Model;
using Wpf.Ui.Appearance;
using Wpf.Ui.Input;

namespace TomLauncher.ViewModel.Pages;

public class PreviewViewModel
{
    public PreviewModel Model { get; set; }
    public ImageSource BackgroundSource { get; set; }
    public ICommand OpenCommand { get; }
    public PreviewViewModel()
    {
        // When Preview view model initializes -- system variables
        // and other information about installed components
        // already prepared by the application startup.
        Model = new PreviewModel
        {
            Mods = new ObservableCollection<JavaArchiveData>(),
            Shaders = new ObservableCollection<ShadersData>(),
            Textures = new ObservableCollection<TextureData>()
        };
        // Register placed entries in the \mods directory
        foreach (var item in Directory.EnumerateFiles(App.GameLocation +  "\\mods"))
            Model.Mods?.Add(EntityBuilder.FillJavaArchive(item));
        // Register placed entries in the \resourcepacks directory
        foreach (var texture in Directory.EnumerateFiles(App.GameLocation + "\\resourcepacks"))
            Model.Textures?.Add(EntityBuilder.FillResource(texture));
        // Register placed entries in the \shaderpacks directory 
        foreach (var shaders in Directory.EnumerateFiles(App.GameLocation + "\\shaderpacks"))
            Model.Shaders?.Add(EntityBuilder.FillShaders(shaders));
        // The WPF-UI represents many types of System backdrop types and
        // window themes for Windows 11.
        if (SystemThemeManager.GetCachedSystemTheme()
            is SystemTheme.Dark
            or SystemTheme.Glow
            or SystemTheme.CapturedMotion
            or SystemTheme.HCBlack)
            BackgroundSource = new BitmapImage(new Uri("/Assets/dark.png", UriKind.Relative));
        else
            BackgroundSource = new BitmapImage(new Uri("/Assets/light.png", UriKind.Relative));
        
        OpenCommand = new RelayCommand<string>(Open);
    }

    private void Open(string? path)
    {
        // The path is a name of Minecraft subdirectory 
        // %GAME%\mods or ...\resourcepacks or \shaderpacks
        Process.Start("explorer", App.GameLocation + path);
    }
}