using TomLauncher.Backend;
using TomLauncher.Backend.Reader;

namespace TomLauncher.ViewModel.Windows;

public class MetadataViewModel(Dictionary<LoaderType, ManifestGenerals> manifests)
{
    public MetadataViewModel() : this([])
    {
        
    }

    public Dictionary<LoaderType, ManifestGenerals> Manifests { get; set; } = manifests;
}