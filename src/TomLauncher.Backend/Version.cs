using System.Text.RegularExpressions;

namespace TomLauncher.Backend;

public partial class Version
{
    public uint Major { get; set; }
    public uint Minor { get; set; }
    public uint Revision { get; set; }
    public uint Build { get; set; }

    public static Version FromString(string? v)
    {
        if (v is null)
            return new Version();
            
        var source = JustNumbers().Split(v);
        var version = new string[4];
        
        for (byte i = 0; i < Math.Min(source.Length, 4); ++i)
        {
            version[i] = source[i];
        }
            
        return new Version
        {
            Major = Convert.ToUInt32(version[0]),
            Minor = Convert.ToUInt32(version[1]),
            Revision = Convert.ToUInt32(version[2]),
            Build = Convert.ToUInt32(version[3])
        };
    }

    public override string ToString()
    {
        if (Build == 0 && Revision == 0 && Minor == 0 && Major == 0)
            return "*";
        
        return $"{Major}.{Minor}.{Revision}.{Build}";
    }

    [GeneratedRegex(@"\D+")]
    private static partial Regex JustNumbers();
}