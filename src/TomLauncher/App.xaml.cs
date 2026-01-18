using System.Globalization;
using System.IO;
using System.Windows;

namespace TomLauncher;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public string GameLocation { get; set; } = "?";

    /// <summary>
    /// Will be called when application tries to init WPF services
    /// Application CMD arguments will be caught here if they're necessary.
    /// </summary>
    /// <param name="e">
    /// Event arguments of current running Application instance
    /// </param>
    protected override void OnStartup(StartupEventArgs e)
    {
        // When WPF services not run yet, I gonna setup ResourceDictionaries
        // and other URLs of App.xaml like code-behind.
        var settings = AppDomain.CurrentDomain.BaseDirectory + "Settings";
        // The FirstWindow shows once when Settings file is missing
        // Startup language parameter defines at once depending on System CultureInfo
        // If Culture index same with Russian -> Default language will be russian and
        // Defaults of Settings TOML will be index of russian.
        if (!File.Exists(settings))
        {
            // Startup logic changes -> new FirstWindow
            // If Culture information tells ru-RU -> change the language index
            Current.Resources.Source = CultureInfo.CurrentCulture.EnglishName == "ru-RU" 
                ? new Uri("Culture/Russian.xaml") 
                : new Uri("Culture/English.xaml");
            // Redirect WPF services from MainWindow to the FirstWindow
            Current.StartupUri = new Uri("View/Windows/FirstWindow.xaml");
        }
        // Initialize WPF services and run prepared application instance
        base.OnStartup(e);
    }
}