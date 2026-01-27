using System.Collections.ObjectModel;
using TomLauncher.Backend;
using TomLauncher.Backend.Reader;

namespace TomLauncher.Model;

public sealed class ManifestsCollection : ObservableCollection<Dictionary<LoaderType, ManifestGenerals>>;