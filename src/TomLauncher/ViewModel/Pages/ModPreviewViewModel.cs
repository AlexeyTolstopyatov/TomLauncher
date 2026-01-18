using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using TomLauncher.Backend.Builder;
using TomLauncher.Model;
using Wpf.Ui.Input;

namespace TomLauncher.ViewModel.Pages;

public class ModPreviewViewModel
{
    public ModPreviewModel Model { get; set; }

    public ICommand OpenModCommand { get; }

    public ModPreviewViewModel()
    {
        Model = new();
        Model.ModVisibility = Visibility.Hidden;
        OpenModCommand = new RelayCommand<object>(Open);
    }

    private void Open(object? _)
    {
        var dialog = new OpenFileDialog
        {
            Multiselect = false,
            Title = "Jar checkout",
            Filter = "Java Minecraft Mods (*.jar)|*.jar"
        };

        dialog.ShowDialog();

        if (string.IsNullOrEmpty(dialog.FileName)) 
            return;

        Model.Mod = ModModelBuilder.FillJavaArchive(dialog.FileName);
        Model.ModVisibility = Visibility.Visible;
    }
}