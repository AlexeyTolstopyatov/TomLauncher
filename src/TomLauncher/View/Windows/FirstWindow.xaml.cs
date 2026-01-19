using System.IO;
using System.Windows;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace TomLauncher.View.Windows;

public partial class FirstWindow : FluentWindow
{
    public FirstWindow()
    {
        InitializeComponent();
        ApplicationThemeManager.ApplySystemTheme();
    }

    private void DoneButtonOnClick(object sender, RoutedEventArgs e)
    {
        // Settings file v1.0 is going to be written
        var settings = AppDomain.CurrentDomain.BaseDirectory + "Settings";
        string[] lines = [
            LanguageBox.SelectedIndex.ToString(),
            GameLocation.Text
        ];
        File.WriteAllLines(settings, lines);
        // Next start of application will redirect you at the MainWindow
        // instead of this "little oobe". 
        Close();
    }
}