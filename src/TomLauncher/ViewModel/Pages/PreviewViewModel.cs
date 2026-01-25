using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TomLauncher.Backend;
using TomLauncher.Backend.Builder;
using TomLauncher.Model;
using Wpf.Ui.Appearance;

namespace TomLauncher.ViewModel.Pages;

public class PreviewViewModel
{
    public PreviewModel Model { get; set; }
    public ImageSource BackgroundSource { get; set; }
    
    public PreviewViewModel()
    {
        Model = new PreviewModel
        {
            Mods = new ObservableCollection<JavaArchiveData>(),
            Shaders = new ObservableCollection<ShadersData>(),
            Textures = new ObservableCollection<TextureData>()
        };
        foreach (var item in Directory.EnumerateFiles(App.GameLocation +  "\\mods"))
            Model.Mods?.Add(EntityBuilder.FillJavaArchive(item));
        
        foreach (var texture in Directory.EnumerateFiles(App.GameLocation + "\\resourcepacks"))
            Model.Textures?.Add(EntityBuilder.FillResource(texture));
        
        foreach (var shaders in Directory.EnumerateFiles(App.GameLocation + "\\shaderpacks"))
            Model.Shaders?.Add(EntityBuilder.FillShaders(shaders));
        
        if (SystemThemeManager.GetCachedSystemTheme()
            is SystemTheme.Dark
            or SystemTheme.Glow
            or SystemTheme.CapturedMotion
            or SystemTheme.HCBlack)
            BackgroundSource = new BitmapImage(new Uri("/Assets/dark.png", UriKind.Relative));
        else
            BackgroundSource = new BitmapImage(new Uri("/Assets/light.png", UriKind.Relative));
    }
}