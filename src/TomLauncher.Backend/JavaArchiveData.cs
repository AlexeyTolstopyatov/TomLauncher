using System.ComponentModel;
using System.Runtime.Serialization;
using TomLauncher.Backend.Reader;

namespace TomLauncher.Backend;

/// <summary>
/// Leaf of Project tree. Project/Package data contains Loader data what has
/// own data and ends the tree, and Children (modifications list) which could be applied
/// to the package if LoaderData and JavaArchiveData are the same. 
/// </summary>
[DataObject]
public class JavaArchiveData
{
    /// <summary>
    /// Modification loaders type. Range from Undeclared/Unknown (by default)
    /// till known loader name.
    /// </summary>
    [DataMember]
    public LoaderType Loader
    {
        get;
        set;
    }

    /// <summary>
    /// Filesystem information of target java archive
    /// Fills automatically and will be used for other fields instances
    /// </summary>
    [DataMember]
    public FileInfo? File { get; set; }

    /// <summary>
    /// Title takes from the Manifest and depends on Loader type
    /// </summary>
    [DataMember]
    public string Title
    {
        get;
        set;
    } = string.Empty;

    /// <summary>
    /// Name takes from the Manifest and depends on Loader type
    /// </summary>
    [DataMember]
    public string Name
    {
        get;
        set;
    } = string.Empty;

    /// <summary>
    /// Manifest generals
    /// </summary>
    /// <returns></returns>
    [IgnoreDataMember]
    public Dictionary<LoaderType, ManifestGenerals> Manifests
    {
        get;
        set;
    } = [];
}