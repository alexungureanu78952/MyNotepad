using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NotepadClone.Application.Interfaces;
using NotepadClone.Domain;
using System.Collections.ObjectModel;
using System.IO;

namespace NotepadClone.Presentation.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IFileService _fileService;
    private readonly IDialogService _dialogService;
    private readonly IFolderService _folderService;
    private readonly IClipboardService _clipboardService;

    private int _fileCounter = 1;

    [ObservableProperty]
    private ObservableCollection<EditorDocument> _documents = new();

    [ObservableProperty]
    private EditorDocument? _selectedDocument;

    [ObservableProperty]
    private bool _isFolderExplorerVisible = true;

    [ObservableProperty]
    private bool _isStandardView = true;

    [ObservableProperty]
    private bool _isSearchScopeSelectedTab = true;

    [ObservableProperty]
    private ObservableCollection<TreeNodeViewModel> _folderTreeRoots = new();

    [ObservableProperty]
    private TreeNodeViewModel? _selectedTreeNode;

    public bool IsSearchScopeAllTabs
    {
        get => !IsSearchScopeSelectedTab;
        set
        {
            if (value)
            {
                IsSearchScopeSelectedTab = false;
            }
        }
    }

    partial void OnIsSearchScopeSelectedTabChanged(bool value)
    {
        OnPropertyChanged(nameof(IsSearchScopeAllTabs));
    }

    public MainViewModel(
        IFileService fileService,
        IDialogService dialogService,
        IFolderService folderService,
        IClipboardService clipboardService)
    {
        _fileService = fileService;
        _dialogService = dialogService;
        _folderService = folderService;
        _clipboardService = clipboardService;

        // Initialize folder tree with drives
        InitializeFolderTree();

        // Create initial empty tab
        CreateNewDocument();
    }

    private void InitializeFolderTree()
    {
        try
        {
            var drives = _folderService.GetLogicalDrives();
            foreach (var drive in drives)
            {
                try
                {
                    FolderTreeRoots.Add(new TreeNodeViewModel(drive, true));
                }
                catch
                {
                    // Skip inaccessible drives
                }
            }
        }
        catch (Exception ex)
        {
            _dialogService.ShowMessage($"Error loading drives: {ex.Message}", "Error");
        }
    }

    [RelayCommand]
    private void CreateNewDocument()
    {
        var doc = new EditorDocument
        {
            Title = $"File {_fileCounter++}"
        };

        // Subscribe to text changes to set IsDirty
        doc.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(EditorDocument.Text) && !doc.IsDirty && !string.IsNullOrEmpty(doc.FilePath))
            {
                doc.IsDirty = true;
            }
        };

        Documents.Add(doc);
        SelectedDocument = doc;
    }

    [RelayCommand]
    private void OpenFile()
    {
        var filePath = _dialogService.ShowOpenFileDialog();
        if (filePath == null) return;

        try
        {
            var content = _fileService.ReadFile(filePath);
            var doc = new EditorDocument
            {
                Title = Path.GetFileName(filePath),
                FilePath = filePath,
                Text = content,
                IsDirty = false
            };

            // Subscribe to text changes
            doc.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(EditorDocument.Text))
                {
                    doc.IsDirty = true;
                }
            };

            Documents.Add(doc);
            SelectedDocument = doc;
        }
        catch (Exception ex)
        {
            _dialogService.ShowMessage($"Error opening file: {ex.Message}", "Error");
        }
    }

    [RelayCommand(CanExecute = nameof(CanExecuteFileCommand))]
    private void SaveFile()
    {
        if (SelectedDocument == null) return;

        if (string.IsNullOrEmpty(SelectedDocument.FilePath))
        {
            SaveFileAs();
            return;
        }

        try
        {
            _fileService.WriteFile(SelectedDocument.FilePath, SelectedDocument.Text);
            SelectedDocument.IsDirty = false;
        }
        catch (Exception ex)
        {
            _dialogService.ShowMessage($"Error saving file: {ex.Message}", "Error");
        }
    }

    [RelayCommand(CanExecute = nameof(CanExecuteFileCommand))]
    private void SaveFileAs()
    {
        if (SelectedDocument == null) return;

        var filePath = _dialogService.ShowSaveFileDialog();
        if (filePath == null) return;

        try
        {
            _fileService.WriteFile(filePath, SelectedDocument.Text);
            SelectedDocument.FilePath = filePath;
            SelectedDocument.Title = Path.GetFileName(filePath);
            SelectedDocument.IsDirty = false;
        }
        catch (Exception ex)
        {
            _dialogService.ShowMessage($"Error saving file: {ex.Message}", "Error");
        }
    }

    [RelayCommand]
    private void CloseDocument(EditorDocument document)
    {
        if (document == null) return;

        if (!PromptSaveIfNeeded(document))
        {
            return; // User cancelled
        }

        Documents.Remove(document);

        // If no documents left, create a new one
        if (Documents.Count == 0)
        {
            CreateNewDocument();
        }
    }

    [RelayCommand]
    private void CloseAllFiles()
    {
        var documentsToClose = Documents.ToList();

        foreach (var doc in documentsToClose)
        {
            if (!PromptSaveIfNeeded(doc))
            {
                return; // User cancelled, stop closing
            }
        }

        Documents.Clear();
        CreateNewDocument();
    }

    [RelayCommand(CanExecute = nameof(CanExecuteFileCommand))]
    private void Find()
    {
        // TODO: Implement in Milestone 6
        _dialogService.ShowMessage("Find functionality will be implemented in Milestone 6", "Coming Soon");
    }

    [RelayCommand(CanExecute = nameof(CanExecuteFileCommand))]
    private void Replace()
    {
        // TODO: Implement in Milestone 6
        _dialogService.ShowMessage("Replace functionality will be implemented in Milestone 6", "Coming Soon");
    }

    [RelayCommand(CanExecute = nameof(CanExecuteFileCommand))]
    private void ReplaceAll()
    {
        // TODO: Implement in Milestone 6
        _dialogService.ShowMessage("Replace All functionality will be implemented in Milestone 6", "Coming Soon");
    }

    [RelayCommand]
    private void ShowStandardView()
    {
        IsStandardView = true;
        IsFolderExplorerVisible = false;
    }

    [RelayCommand]
    private void ToggleFolderExplorer()
    {
        IsFolderExplorerVisible = !IsFolderExplorerVisible;
        IsStandardView = !IsFolderExplorerVisible;
    }

    [RelayCommand]
    private void TreeNodeDoubleClick(TreeNodeViewModel? node)
    {
        if (node == null || node.IsDirectory || string.IsNullOrEmpty(node.FullPath))
        {
            return;
        }

        // Open file in new tab
        try
        {
            var content = _fileService.ReadFile(node.FullPath);
            var doc = new EditorDocument
            {
                Title = Path.GetFileName(node.FullPath),
                FilePath = node.FullPath,
                Text = content,
                IsDirty = false
            };

            // Subscribe to text changes
            doc.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(EditorDocument.Text))
                {
                    doc.IsDirty = true;
                }
            };

            Documents.Add(doc);
            SelectedDocument = doc;
        }
        catch (Exception ex)
        {
            _dialogService.ShowMessage($"Error opening file: {ex.Message}", "Error");
        }
    }

    [RelayCommand]
    private void ShowAbout()
    {
        _dialogService.ShowMessage(
            "Notepad Clone - WPF Homework Project\n\n" +
            "Student: [Your Name]\n" +
            "Grupa: [Your Group]\n" +
            "Email: [your.email@student.pub.ro]\n\n" +
            "Developed with .NET 8 + WPF + MVVM",
            "About Notepad Clone");
    }

    [RelayCommand]
    private void Exit()
    {
        var documentsToCheck = Documents.ToList();

        foreach (var doc in documentsToCheck)
        {
            if (!PromptSaveIfNeeded(doc))
            {
                return; // User cancelled exit
            }
        }

        System.Windows.Application.Current.Shutdown();
    }

    /// <summary>
    /// Prompts user to save document if it has unsaved changes.
    /// Returns true if it's safe to proceed (saved, discarded, or no changes).
    /// Returns false if user cancelled.
    /// </summary>
    private bool PromptSaveIfNeeded(EditorDocument document)
    {
        if (!document.IsDirty)
        {
            return true; // No unsaved changes
        }

        var result = _dialogService.ShowYesNoCancelDialog(
            $"Do you want to save changes to '{document.Title}'?",
            "Unsaved Changes");

        if (result == null) // Cancel
        {
            return false;
        }

        if (result == true) // Yes - save
        {
            if (string.IsNullOrEmpty(document.FilePath))
            {
                var filePath = _dialogService.ShowSaveFileDialog();
                if (filePath == null)
                {
                    return false; // User cancelled save dialog
                }

                try
                {
                    _fileService.WriteFile(filePath, document.Text);
                    document.FilePath = filePath;
                    document.Title = Path.GetFileName(filePath);
                    document.IsDirty = false;
                    return true;
                }
                catch (Exception ex)
                {
                    _dialogService.ShowMessage($"Error saving file: {ex.Message}", "Error");
                    return false;
                }
            }
            else
            {
                try
                {
                    _fileService.WriteFile(document.FilePath, document.Text);
                    document.IsDirty = false;
                    return true;
                }
                catch (Exception ex)
                {
                    _dialogService.ShowMessage($"Error saving file: {ex.Message}", "Error");
                    return false;
                }
            }
        }

        // No - discard changes
        return true;
    }

    private bool CanExecuteFileCommand()
    {
        return SelectedDocument != null;
    }
}
