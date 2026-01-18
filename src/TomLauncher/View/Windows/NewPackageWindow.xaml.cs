using System.Windows;
using Wpf.Ui.Controls;

namespace TomLauncher.View.Windows;

public partial class NewPackageWindow : FluentWindow
{
    public NewPackageWindow()
    {
        InitializeComponent();
    }

    private void CreateButtonOnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }

    private void CancelButtonOnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}