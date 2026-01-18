namespace TomLauncher.Backend;

public class LoaderData : Version
{
    public LoaderType Type { get; set; }

    public LoaderData(LoaderType type, string? version)
    {
        Type = type;
        var v = FromString(version);

        Major = v.Major;
        Minor = v.Minor;
        Revision = v.Revision;
        Build = v.Build;
    }
}