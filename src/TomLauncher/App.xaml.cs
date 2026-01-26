using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using SearchOption = Microsoft.VisualBasic.FileIO.SearchOption;

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
    /// Installed JRE or JDK version string.
    /// </summary>
    public static string? JavaVersion { get; set; } = "?";
    /// <summary>
    /// This Tuple contains filesystem info and fills once at the
    /// application startup. That's why it contains here 
    /// </summary>
    public static (double Size, double GameSize) Disk { get; private set; }
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
        // Defaults of Settings TOML will be russian dictionary ordinal.
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
        JavaVersion = GetJavaInstallationPath();
        Disk = GetDiskCapacity();
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
    private static string? GetJavaInstallationPath()
    {
        // %JAVA_HOME% creates when Java development kit installed
        var environmentPath = Environment.GetEnvironmentVariable("JAVA_HOME");
        if (!string.IsNullOrEmpty(environmentPath))
        {
            var versionInfo = FileVersionInfo.GetVersionInfo(environmentPath + @"\bin\javac.exe");
            return versionInfo.FileVersion;
        }
        // Available registry keys into JavaSoft submodule
        const string jre = @"SOFTWARE\JavaSoft\Java Runtime Environment\";
        const string jdk = @"SOFTWARE\JavaSoft\JDK\";
        
        using var jreKey = Registry.LocalMachine.OpenSubKey(jre);
        var currentVersion = jreKey?.GetValue("CurrentVersion")?.ToString() ?? string.Empty;
        
        if (!string.IsNullOrEmpty(currentVersion)) 
            return currentVersion + " (JRE)";
        
        using var jdkKey = Registry.LocalMachine.OpenSubKey(jdk);
        currentVersion = jdkKey?.GetValue("CurrentVersion")?.ToString();
        // If current version is null -> send fallback value
        // instead of empty " (JDK)" string. 
        if (currentVersion is null)
            currentVersion = "?";
        else
            currentVersion += " (JDK)";
        // Already prepared string with Java RE or DK components version
        // Will be shown in PreviewPage for quick report to user.
        return currentVersion;
    }

    private static (double disk, double game) GetDiskCapacity()
    {
        // Root field in FileSystemInfo typed members
        // returns the Drive path where FileSystemInfo entry located.
        // Then gigabytes fraction will be computed and returned.
        var diskSize = Task.Run(double () =>
        {
            var targetDrive = new DirectoryInfo(GameLocation).Root;
            var drive = new DriveInfo(targetDrive.FullName);
            
            return Math.Round((double)drive.TotalSize / (1024 * 1024 * 1024), 2);
        });
        // Run another task to compute game directory.
        // Size shows in gigabytes then I want use f64 typed field
        // and run this task at once.
        var gameSize = Task.Run(double () =>
        {
            var gameLocation = new DirectoryInfo(GameLocation);
            var size = gameLocation
                .EnumerateFiles("*.*", System.IO.SearchOption.AllDirectories)
                .Sum(f => f.Length);
            
            var result = Math.Round((double)size / (1024 * 1024 * 1024), 2);
            return result;
        });
        
        return (diskSize.Result, gameSize.Result);
    }
}