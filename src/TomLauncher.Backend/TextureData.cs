namespace TomLauncher.Backend;

public class TextureData
{
    /// <summary>
    /// Depending on pack.mcmeta this is a package format.
    /// Uses in EditorPage view at TexturePack "ToolTip" binding
    /// </summary>
    public int PackFormat 
    { 
        get; 
        set;
    }
    /// <summary>
    /// Depending on pack.mcmeta this is a description
    /// of current texturepack which you see in ResourcePacks menu
    /// in the Game.
    /// May contain special symbols what game
    /// recognizes as color markers for strings.
    /// </summary>
    public string PackDescription
    {
        get; 
        set;
    } = string.Empty;
    /// <summary>
    /// File information property.
    /// </summary>
    public FileInfo File
    {
        get;
        set;
    }
}