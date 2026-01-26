using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using TomLauncher.Backend;
using TomLauncher.Backend.Builder;
using TomLauncher.Model;
using TomLauncher.View.Windows;
using TomLauncher.ViewModel.Windows;
using Wpf.Ui.Input;

namespace TomLauncher.ViewModel.Pages;

/// <summary>
/// EditorPage view model contains just bindings with EditorPage Model
/// </summary>
public class EditorViewModel
{
    private PackageBuilder? _builder;
    public EditorViewModel()
    {
        Model = new EditorPageModel
        {
            PackageOpened = Visibility.Collapsed
        };
        CreatePackageCommand = new RelayCommand<object>(Create);
        OpenExistingCommand = new RelayCommand<object>(OpenExisting);
        OpenPackageCommand = new RelayCommand<object>(Open);
        ExportCommand = new RelayCommand<object>(Export);
        ExplorerCommand = new RelayCommand<object>(Explorer);
        IncludeCommand = new RelayCommand<string>(Include);
        ExcludeCommand = new RelayCommand<object>(Exclude);
    }

    public EditorPageModel Model { get; }
    public ICommand CreatePackageCommand { get; }
    public ICommand OpenPackageCommand { get; }
    public ICommand OpenExistingCommand { get; }
    public ICommand ExcludeCommand { get; }
    public ICommand IncludeCommand { get; }
    public ICommand ExportCommand { get; }
    public ICommand ExplorerCommand { get; }

    private void Export(object? _)
    {
        var minecraft = new DirectoryInfo(App.GameLocation);
        if (!minecraft.Exists)
        {
            Console.WriteLine("Missing .minecraft catalog");
            return;
        }

        foreach (var mod in Directory.EnumerateFiles(_builder!.Mods))
            File.Copy(mod, App.GameLocation + $"\\mods\\{Path.GetFileName(mod)}", true);
        
        foreach (var texture in Directory.EnumerateFiles(_builder!.ResourcePacks))
            File.Copy(texture, App.GameLocation + $"\\resourcepacks\\{Path.GetFileName(texture)}", true);
        
        foreach (var shaders in Directory.EnumerateFiles(_builder!.ShaderPacks))
            File.Copy(shaders, App.GameLocation + $"\\shaderpacks\\{Path.GetFileName(shaders)}", true);
        
        Model.IsExportEnabled = false;
    }
    
    private void Explorer(object? _)
    {
        Process.Start("explorer", _builder is not null 
            ? _builder!.Root 
            : App.GameLocation);
    }
    
    private void Create(object? _)
    {
        var dialog = new NewPackageWindow();
        dialog.ShowDialog();

        var result = dialog.DialogResult!.Value;
        
        if (!result || dialog.DataContext is not NewPackageViewModel vm) 
            return;
        
        Model.PackageOpened = Visibility.Visible;
        Model.Package = vm.Model.ToPackageData;

        _builder = new PackageBuilder(vm.Model.Path!, vm.Model.Name!, false);
        _builder.Write(Model.Package);
    }

    private void OpenExisting(object? _)
    {
        Model.IsExportEnabled = false;
        Model.Package = new("Minecraft")
        {
            Mods = new(),
            Textures = new(),
            Shaders = new(),
            Loader = new LoaderData(LoaderType.Unknown, "0.0.0.0")
        };
        foreach (var item in Directory.EnumerateFiles(App.GameLocation +  "\\mods"))
            Model.Package.Mods.Add(EntityBuilder.FillJavaArchive(item));
        
        foreach (var texture in Directory.EnumerateFiles(App.GameLocation + "\\resourcepacks"))
            Model.Package.Textures.Add(EntityBuilder.FillResource(texture));
        
        foreach (var shaders in Directory.EnumerateFiles(App.GameLocation + "\\shaderpacks"))
            Model.Package.Shaders.Add(EntityBuilder.FillShaders(shaders));
        
        Model.PackageOpened = Visibility.Visible;
    }
    
    private void Open(object? _)
    {
        var dialog = new OpenFolderDialog
        {
            Multiselect = false
        };
        dialog.ShowDialog();
        
        Console.WriteLine(dialog.FolderName);
        if (string.IsNullOrEmpty(dialog.FolderName))
        {
            Console.WriteLine("Empty directory");
            return;
        }
        
        if (!File.Exists($"{dialog.FolderName}\\header.toml"))
        {
            Console.WriteLine("Missing header.toml");
            return;
        }
        
        // Fill the card from header.toml
        _builder = new PackageBuilder(dialog.FolderName);
        Model.Package = _builder.Read();
        // Scan nested mods
        foreach (var mod in Directory.EnumerateFiles(_builder.Mods))
        {
            var model = _builder.GetMod(mod, Model.Package.Loader!.Type);
                
            if (model.Manifests.ContainsKey(Model.Package.Loader!.Type))
                Model.Package.Mods.Add(model);
            else
                Console.WriteLine($"Skipped: {Model.Package.Loader.Type} not embedded into {model.Name}");
            
        }
        // Scan nested textures
        foreach (var resource in Directory.EnumerateFiles(_builder.ResourcePacks))
        {
            var model = _builder.GetResource(resource);
            Model.Package.Textures.Add(model);
        }
        // Scan nested shaders
        foreach (var shaders in Directory.EnumerateFiles(_builder.ShaderPacks).Where(t => t.Contains(".zip")))
        {
            var model = _builder.GetShaders(shaders);
            Model.Package.Shaders.Add(model);
        }
        
        Model.PackageOpened = Visibility.Visible;
        Model.IsExportEnabled = true;
    }
    
    private void Include(string? parameter)
    {
        var kind = Enum.Parse<PackageBuilder.ArtifactKind>(parameter!);
        var dialog = new OpenFileDialog
        {
            Multiselect = true,
            Filter = "Minecraft Mods (*.jar)|*.jar|Minecraft Resources (*.zip)|*.zip|Minecraft Shaders (*.zip)|*.zip",
            FilterIndex = (int)kind + 1
        };
        dialog.ShowDialog();

        foreach (var item in dialog.FileNames)
        {
            // unoptimized
            switch (kind)
            {
                case PackageBuilder.ArtifactKind.Mod:
                    if (!Model.Package!.Mods.Select(t => t.File?.Name).Contains(Path.GetFileName(item)))
                    {
                        Model.Package?.Mods.Add(_builder?.GetMod(item, Model.Package.Loader!.Type)!);
                        _builder?.Include(item, kind);
                    }
                    break;
                case PackageBuilder.ArtifactKind.Resource:
                    if (!Model.Package!.Textures.Select(t => t.File.Name).Contains(Path.GetFileName(item)))
                    {
                        Model.Package?.Textures.Add(_builder?.GetResource(item)!);
                        _builder?.Include(item, kind);
                    }
                    break;
                case PackageBuilder.ArtifactKind.Shaders:
                    if (!Model.Package!.Mods.Select(t => t.File?.Name).Contains(Path.GetFileName(item)))
                    {
                        Model.Package?.Shaders.Add(_builder?.GetShaders(item)!);
                        _builder?.Include(item, kind);
                    }
                    break;
            }
        }
    }

    private void Exclude(object? model)
    {
        switch (model)
        {
            case JavaArchiveData j:
                _builder?.Exclude($"{_builder.Mods}\\{j.File?.Name}");
                Model.Package?.Mods.Remove(j);
                break;
            case TextureData t:
                _builder?.Exclude($"{_builder.ResourcePacks}\\{t.File.Name}");
                Model.Package?.Textures.Remove(t);
                break;
            case ShadersData s:
                _builder?.Exclude($"{_builder.ShaderPacks}\\{s.File.Name}");
                
                if (!string.IsNullOrEmpty(s.ConfigFileName))
                    _builder?.Exclude($"{_builder.ShaderPacks}\\{s.ConfigFileName}");
                
                Model.Package?.Shaders.Remove(s);
                break;
        }
    }
}