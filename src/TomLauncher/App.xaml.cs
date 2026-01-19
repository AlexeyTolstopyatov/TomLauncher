using System.Globalization;
using System.IO;
using System.Windows;

namespace TomLauncher;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Location of Minecraft directory
    /// </summary>
    public static string GameLocation { get; set; } = "?";
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
            // Startup logic changes -> new FirstWindow(void)
            // If Culture information tells ru-RU -> change the language index
            var dictionary = new ResourceDictionary
            {
                Source = CultureInfo.CurrentCulture.Name == "ru-RU" 
                    ? new Uri("/Culture/Russian.xaml", UriKind.Relative) 
                    : new Uri("/Culture/English.xaml", UriKind.Relative)
            };
            // Redirect WPF services from MainWindow to the FirstWindow
            Current.Resources.MergedDictionaries.Add(dictionary);
            Current.StartupUri = new Uri("/View/Windows/FirstWindow.xaml", UriKind.Relative);
        }
        else
        {
            // Elsewhere the ResourceDictionary also depends on selected index
            // in the Settings file. 
            var lines = File.ReadAllLines(settings);
            // 1st Line contains index 0 -> English
            // 1st Line contains index 1 -> Russian
            var dictionary = new ResourceDictionary
            {
                Source = lines[0] == 0.ToString() 
                    ? new Uri("/Culture/English.xaml", UriKind.Relative)
                    : new Uri("/Culture/Russian.xaml", UriKind.Relative)
            };
            // Don't redirect to FirstWindow. Just continue
            Current.Resources.MergedDictionaries.Add(dictionary);
            GameLocation = lines[1];
        }
        // Initialize WPF services and run prepared application instance
        base.OnStartup(e);
    }
    /// <summary>
    /// Rewrites the ResourceDictionary of
    /// current application instance.
    /// </summary>
    /// <param name="name">/Culture/*.xaml</param>
    public static void SetLanguage(string name)
    {
        // Prepare the relative URL of replacing resource
        // Append it to application resources
        var dictionary = new ResourceDictionary
        {
            Source = new Uri($"/Culture/{name}.xaml", UriKind.Relative)
        };
        Current.Resources.MergedDictionaries.Add(dictionary);
    }
}