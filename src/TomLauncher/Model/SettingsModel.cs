using TomLauncher.ViewModel;

namespace TomLauncher.Model;

/// <summary>
/// Settings File schema version 1.0
/// </summary>
public class SettingsModel : NotifyPropertyChanged
{
    private string _gameLocation = ".";
    private int _currentLanguage;
    /// <summary>
    /// ComboBox list of possible languages.
    /// Sorry for no reflection (;- ;)
    /// </summary>
    public string[] Languages { get; set; } = 
    [
        "English", 
        "Russian"
    ];
    /// <summary>
    /// Automatically updates when Settings VM will be init
    /// </summary>
    public string GameLocation
    {
        get => _gameLocation; 
        set => SetField(ref _gameLocation, value);
    }
    /// <summary>
    /// Automatically updates when Settings VM will be init
    /// </summary>
    public int CurrentLanguage
    {
        get => _currentLanguage;
        set
        {
            App.SetLanguage(Languages[value]);
            SetField(ref _currentLanguage, value);
        }
    }
}