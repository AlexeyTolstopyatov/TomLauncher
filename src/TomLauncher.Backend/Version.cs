using System.Text.RegularExpressions;

namespace TomLauncher.Backend;
/// <summary>
/// Base class for version contained classes, bases on Microsoft
/// versioning guide. (if I remember this exists)
/// </summary>
public partial class Version
{
    /// <summary>
    /// Global changes in the product. 
    /// </summary>
    public uint Major
    {
        get; 
        set;
    }
    /// <summary>
    /// The minor version number is usually increased when
    /// new features are added to the software or existing
    /// features are significantly improved. These changes are usually
    /// compatible with previous versions, meaning that users can upgrade
    /// to the new version without making major changes to their code.
    /// </summary>
    public uint Minor
    {
        get; 
        set;
    }
    /// <summary>
    /// The revision field is optional in Microsoft versioning
    /// It means a product version what closes critical issue(s)
    /// or security issue(s) what was occured in previous version.
    /// </summary>
    public uint Revision
    {
        get; 
        set;
    }
    /// <summary>
    /// Different build numbers are needed if the recompilation
    /// is performed for a different processor, platform,
    /// optimization options, or a different compiler.
    /// </summary>
    public uint Build
    {
        get;
        set;
    }
    /// <summary>
    /// Make a Version instance using a string with declared version
    /// </summary>
    /// <param name="v">
    /// Version string
    /// </param>
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
    /// <summary>
    /// Returns string of current version. If string contains zeroes (exactly 0.0.0.0)
    /// the resulting string will be "*".
    ///
    /// Remember: the star "*" is a "похуй"-keyword which tells the others
    /// this is an "Any version supported" or "nothing here"
    /// </summary>
    public override string ToString()
    {
        if (Build == 0 && Revision == 0 && Minor == 0 && Major == 0)
            return "*";
        
        return $"{Major}.{Minor}.{Revision}.{Build}";
    }
    /// <summary>
    /// Regular expression delegate for versioning string deserialize
    /// </summary>
    [GeneratedRegex(@"\D+")]
    private static partial Regex JustNumbers();
}