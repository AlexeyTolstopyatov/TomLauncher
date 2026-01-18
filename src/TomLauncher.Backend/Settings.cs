namespace TomLauncher.Backend;

public class Settings
{
    public string GameDirectory { get; set; }
    public int CurrentLanguage { get; set; } 

    public Settings()
    {
        var settings = AppDomain.CurrentDomain.BaseDirectory + "Settings";
        if (!File.Exists(settings))
        {
            File.WriteAllLines(settings, ["0", "?"]); 
        }

        var content = File.ReadAllLines(settings);
        CurrentLanguage = int.Parse(content[0]);
        GameDirectory = content[1];
    }
    
}