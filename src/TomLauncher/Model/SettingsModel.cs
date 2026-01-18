namespace TomLauncher.Model;

/// <summary>
/// Settings File schema version 1.0
/// </summary>
public class SettingsModel
{
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
    public string GameDirectory
    {
        get; 
        set;
    } = ".";
    /// <summary>
    /// Automatically updates when Settings VM will be init
    /// </summary>
    public int LanguageIndex
    {
        get; 
        set;
    } = 0;
}