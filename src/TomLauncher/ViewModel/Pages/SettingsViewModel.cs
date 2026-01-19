using System.Windows.Input;
using Microsoft.Win32;
using TomLauncher.Backend;
using TomLauncher.Model;
using Wpf.Ui.Input;

namespace TomLauncher.ViewModel.Pages;

public class SettingsViewModel
{
    public SettingsModel Model { get; set; }

    public SettingsViewModel()
    {
        var settings = new Settings();
        Model = new SettingsModel
        {
            GameLocation = settings.GameLocation,
            CurrentLanguage = settings.CurrentLanguage
        };
        WriteSettingsCommand = new RelayCommand<object?>(WriteSettings);
        BrowseCommand = new RelayCommand<object?>(Browse);
    }

    public ICommand WriteSettingsCommand { get; }
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
    private void WriteSettings(object? _)
    {
        // At the moment of missing of LanguageChanged event handler
        // bad but existing way to update View is an application restart
        App.GameLocation = Model.GameLocation;
        Settings.Write(new Settings(Settings.SettingsConstructor.UseModel)
        {
            CurrentLanguage = Model.CurrentLanguage,
            GameLocation = Model.GameLocation
        });
        // Then find the current application resources
        // And rewrite them right now
        App.SetLanguage(Model.Languages[Model.CurrentLanguage]);
    }
}