using CommunityToolkit.Mvvm.ComponentModel;
using NotepadClone.Application.Interfaces;
using System.Collections.ObjectModel;
using System.IO;

namespace NotepadClone.Presentation.ViewModels;

/// <summary>
/// Represents a node in the folder explorer tree (folder or file).
/// </summary>
public partial class TreeNodeViewModel : ObservableObject
{
    private readonly IFolderService _folderService;
    private readonly string _fullPath;
    private readonly bool _isDirectory;
    private bool _isExpanded;
    private bool _childrenLoaded;

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private ObservableCollection<TreeNodeViewModel> _children = new();

    [ObservableProperty]
    private bool _isSelected;

    public string FullPath => _fullPath;
    public bool IsDirectory => _isDirectory;

    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            if (SetProperty(ref _isExpanded, value) && value && !_childrenLoaded)
            {
                LoadChildren();
            }
        }
    }

    public TreeNodeViewModel(string path, bool isDirectory, IFolderService folderService, bool addPlaceholder = true)
    {
        _folderService = folderService;
        _fullPath = path;
        _isDirectory = isDirectory;
        var directoryName = isDirectory && !string.IsNullOrWhiteSpace(path)
            ? new DirectoryInfo(path).Name
            : string.Empty;
        _name = isDirectory
            ? (string.IsNullOrWhiteSpace(directoryName) ? path : directoryName)
            : Path.GetFileName(path);

        // Add dummy child for directories to show expand arrow
        if (_isDirectory && addPlaceholder)
        {
            Children.Add(new TreeNodeViewModel(string.Empty, false, _folderService, false) { Name = "Loading..." });
        }
    }

    private void LoadChildren()
    {
        if (_childrenLoaded || !_isDirectory) return;

        Children.Clear();
        _childrenLoaded = true;

        try
        {
            // Load subdirectories
            var directories = _folderService.GetDirectories(_fullPath);
            foreach (var dir in directories.OrderBy(d => d))
            {
                try
                {
                    Children.Add(new TreeNodeViewModel(dir, true, _folderService));
                }
                catch
                {
                    // Skip inaccessible directories
                }
            }

            // Load files
            var files = _folderService.GetFiles(_fullPath);
            foreach (var file in files.OrderBy(f => f))
            {
                try
                {
                    Children.Add(new TreeNodeViewModel(file, false, _folderService, false));
                }
                catch
                {
                    // Skip inaccessible files
                }
            }

            // If no children, indicate empty folder
            if (Children.Count == 0)
            {
                Children.Add(new TreeNodeViewModel(string.Empty, false, _folderService, false) { Name = "(Empty)" });
            }
        }
        catch (UnauthorizedAccessException)
        {
            Children.Add(new TreeNodeViewModel(string.Empty, false, _folderService, false) { Name = "(Access Denied)" });
        }
        catch (Exception ex)
        {
            Children.Add(new TreeNodeViewModel(string.Empty, false, _folderService, false) { Name = $"(Error: {ex.Message})" });
        }
    }

    public void Refresh()
    {
        if (_isDirectory && _childrenLoaded)
        {
            _childrenLoaded = false;
            if (_isExpanded)
            {
                LoadChildren();
            }
            else
            {
                Children.Clear();
                Children.Add(new TreeNodeViewModel(string.Empty, false, _folderService, false) { Name = "Loading..." });
            }
        }
    }
}
