namespace TomLauncher.Model;

/// <summary>
/// Settings File schema version 1.0
/// </summary>
public class SettingsModel
{
    public string[] Languages { get; set; } = 
    [
        "English", 
        "Russian"
    ];

    public string GameDirectory { get; set; } = ".minecraft";
}