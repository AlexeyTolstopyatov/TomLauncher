namespace TomLauncher.Backend;

public class Settings
{
    /// <summary>
    /// Settings constructor enum defines main
    /// constructor behavior:
    /// Will instance filled by the FileStream or not?
    /// </summary>
    public enum SettingsConstructor
    {
        /// <summary>
        /// Open FileStream and fill model from file lines
        /// </summary>
        UseFile,
        /// <summary>
        /// Don't open FileStream, fill DEBUG values
        /// </summary>
        UseModel,
    }
    /// <summary>
    /// Defined Minecraft location
    /// </summary>
    public string GameLocation
    {
        get;
        set;
    }
    /// <summary>
    /// Index of language item in whole list 
    /// </summary>
    public int CurrentLanguage
    {
        get;
        set;
    } 
    /// <summary>
    /// Initializes FileStream and reads data from the model.
    /// If file not exists -> creates and writes debug values
    /// into Settings file.
    ///
    /// Elsewhere, reads whole content and fills the
    /// Settings model instance
    /// </summary>
    public Settings(SettingsConstructor mode = SettingsConstructor.UseFile)
    {
        // Sometimes, reader method is redundant, and needed
        // to memorize data only. If ConstructorSettings::UseModel flag
        // is set -> constructor defines debug values of new model instance
        // and other steps seems to be reachable. 
        if (mode == SettingsConstructor.UseModel)
        {
            CurrentLanguage = 0;
            GameLocation = "?";
            return;
        }
        // If Settings file not exists -> debug values
        // will be "English" local language and "?" path
        // because the location strings less than 3 symbols
        // not exists. 
        var settings = AppDomain.CurrentDomain.BaseDirectory + "Settings";
        if (!File.Exists(settings))
        {
            File.WriteAllLines(settings, ["0", "?"]);
            GameLocation = "?";
            CurrentLanguage = 0;
            return;
        }
        // Elsewhere reader stream seems to be initialized
        var content = File.ReadAllLines(settings);
        CurrentLanguage = int.Parse(content[0]);
        GameLocation = content[1];
    }
    /// <summary>
    /// Initializes FileStream and writes modified Settings model instance.
    /// </summary>
    public static void Write(Settings settings)
    {
        var fs = AppDomain.CurrentDomain.BaseDirectory + "Settings";
        
        File.WriteAllLines(fs, [
            settings.CurrentLanguage.ToString(),
            settings.GameLocation
        ]); 
    }
}