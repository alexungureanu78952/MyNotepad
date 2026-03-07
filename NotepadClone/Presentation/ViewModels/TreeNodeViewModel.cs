using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;

namespace NotepadClone.Presentation.ViewModels;

/// <summary>
/// Represents a node in the folder explorer tree (folder or file).
/// </summary>
public partial class TreeNodeViewModel : ObservableObject
{
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

    public TreeNodeViewModel(string path, bool isDirectory)
    {
        _fullPath = path;
        _isDirectory = isDirectory;
        _name = isDirectory ? new DirectoryInfo(path).Name : Path.GetFileName(path);

        // Add dummy child for directories to show expand arrow
        if (_isDirectory)
        {
            Children.Add(new TreeNodeViewModel("", false) { Name = "Loading..." });
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
            var directories = Directory.GetDirectories(_fullPath);
            foreach (var dir in directories.OrderBy(d => d))
            {
                try
                {
                    Children.Add(new TreeNodeViewModel(dir, true));
                }
                catch
                {
                    // Skip inaccessible directories
                }
            }

            // Load files
            var files = Directory.GetFiles(_fullPath);
            foreach (var file in files.OrderBy(f => f))
            {
                try
                {
                    Children.Add(new TreeNodeViewModel(file, false));
                }
                catch
                {
                    // Skip inaccessible files
                }
            }

            // If no children, indicate empty folder
            if (Children.Count == 0)
            {
                Children.Add(new TreeNodeViewModel("", false) { Name = "(Empty)" });
            }
        }
        catch (UnauthorizedAccessException)
        {
            Children.Add(new TreeNodeViewModel("", false) { Name = "(Access Denied)" });
        }
        catch (Exception ex)
        {
            Children.Add(new TreeNodeViewModel("", false) { Name = $"(Error: {ex.Message})" });
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
                Children.Add(new TreeNodeViewModel("", false) { Name = "Loading..." });
            }
        }
    }
}
