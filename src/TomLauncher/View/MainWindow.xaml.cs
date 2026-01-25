using System.IO;
using System.Windows;
using TomLauncher.Model;
using TomLauncher.View.Pages;
using TomLauncher.View.Windows;
using TomLauncher.ViewModel.Windows;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace TomLauncher.View;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : FluentWindow
{
    public MainWindow()
    {
        InitializeComponent();
        //Appearance.SystemThemeWatcher.Watch(this);;
        SystemThemeWatcher.Watch(this);
        ReadCrashLog();
    }

    private void ReadCrashLog()
    {
        var path = AppDomain.CurrentDomain.BaseDirectory + "Crash.log";
        
        if (!File.Exists(path)) return;

        var content = File.ReadAllText(path);
        var wm = new CriticalViewModel
        {
            Model = new CriticalModel
            {
                CrashInfo = new FileInfo(path),
                CrashLog = content
            }
        };
        var window = new CriticalWindow
        {
            DataContext = wm
        };
        window.Show();
    }
    
    private void MainNavigationViewOnLoaded(object sender, RoutedEventArgs e)
    {
        MainNavigationView.Navigate(typeof(ModPreviewPage));
    }
}