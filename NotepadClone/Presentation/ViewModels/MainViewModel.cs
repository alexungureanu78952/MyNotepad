using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NotepadClone.Application.Interfaces;
using NotepadClone.Domain;
using System.Collections.ObjectModel;
using System.IO;

namespace NotepadClone.Presentation.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private const string AboutStudentName = "Ungureanu Alexandru-Florin";
    private const string AboutGroupName = "10LF244";
    private const string AboutInstitutionalEmail = "florin.ungureanu@student.unitbv.ro";

    private readonly IFileService _fileService;
    private readonly ITextSearchService _textSearchService;
    private readonly IDialogService _dialogService;
    private readonly IFolderService _folderService;
    private readonly IClipboardService _clipboardService;
    private string? _copiedFolderPath;

    private int _fileCounter = 1;

    [ObservableProperty]
    private ObservableCollection<EditorDocument> _documents = new();

    [ObservableProperty]
    private EditorDocument? _selectedDocument;

    [ObservableProperty]
    private bool _isFolderExplorerVisible = true;

    [ObservableProperty]
    private bool _isStandardView;

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

    partial void OnSelectedTreeNodeChanged(TreeNodeViewModel? value)
    {
        NewFileInFolderCommand.NotifyCanExecuteChanged();
        CopyPathCommand.NotifyCanExecuteChanged();
        CopyFolderCommand.NotifyCanExecuteChanged();
        PasteFolderCommand.NotifyCanExecuteChanged();
    }

    public MainViewModel(
        IFileService fileService,
        ITextSearchService textSearchService,
        IDialogService dialogService,
        IFolderService folderService,
        IClipboardService clipboardService)
    {
        _fileService = fileService;
        _textSearchService = textSearchService;
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
                    FolderTreeRoots.Add(new TreeNodeViewModel(drive, true, _folderService));
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
        var searchText = _dialogService.ShowInputDialog("Find what:", "Find");
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return;
        }

        var targets = GetSearchTargets().ToList();
        if (targets.Count == 0)
        {
            return;
        }

        var totalOccurrences = 0;
        var matchedDocs = new List<string>();

        foreach (var document in targets)
        {
            var occurrences = _textSearchService.CountOccurrences(document.Text, searchText);
            if (occurrences > 0)
            {
                totalOccurrences += occurrences;
                matchedDocs.Add($"{document.Title}: {occurrences}");
            }
        }

        if (IsSearchScopeSelectedTab)
        {
            var message = totalOccurrences > 0
                ? $"Found {totalOccurrences} occurrence(s) in '{targets[0].Title}'."
                : $"No matches found in '{targets[0].Title}'.";
            _dialogService.ShowMessage(message, "Find Result");
            return;
        }

        var allTabsMessage = totalOccurrences > 0
            ? $"Found {totalOccurrences} occurrence(s) in {matchedDocs.Count} tab(s).\n\n{string.Join("\n", matchedDocs)}"
            : "No matches found in open tabs.";
        _dialogService.ShowMessage(allTabsMessage, "Find Result");
    }

    [RelayCommand(CanExecute = nameof(CanExecuteFileCommand))]
    private void Replace()
    {
        var searchText = _dialogService.ShowInputDialog("Find what:", "Replace");
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return;
        }

        var replacementText = _dialogService.ShowInputDialog("Replace with:", "Replace", string.Empty) ?? string.Empty;

        var targets = GetSearchTargets().ToList();
        if (targets.Count == 0)
        {
            return;
        }

        var replacedInTabs = 0;
        foreach (var document in targets)
        {
            if (_textSearchService.ReplaceFirst(document.Text, searchText, replacementText, out var updatedText))
            {
                document.Text = updatedText;
                document.IsDirty = true;
                replacedInTabs++;

                if (IsSearchScopeSelectedTab)
                {
                    break;
                }
            }
        }

        var message = replacedInTabs > 0
            ? IsSearchScopeSelectedTab
                ? "One occurrence replaced in selected tab."
                : $"One occurrence replaced in {replacedInTabs} tab(s)."
            : "No match found to replace.";
        _dialogService.ShowMessage(message, "Replace Result");
    }

    [RelayCommand(CanExecute = nameof(CanExecuteFileCommand))]
    private void ReplaceAll()
    {
        var searchText = _dialogService.ShowInputDialog("Find what:", "Replace All");
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return;
        }

        var replacementText = _dialogService.ShowInputDialog("Replace with:", "Replace All", string.Empty) ?? string.Empty;

        var targets = GetSearchTargets().ToList();
        if (targets.Count == 0)
        {
            return;
        }

        var replacedTabs = 0;
        var totalReplacements = 0;

        foreach (var document in targets)
        {
            var replacedCount = _textSearchService.ReplaceAll(document.Text, searchText, replacementText, out var updatedText);
            if (replacedCount > 0)
            {
                document.Text = updatedText;
                document.IsDirty = true;
                replacedTabs++;
                totalReplacements += replacedCount;
            }
        }

        var message = totalReplacements > 0
            ? IsSearchScopeSelectedTab
                ? $"Replaced {totalReplacements} occurrence(s) in selected tab."
                : $"Replaced {totalReplacements} occurrence(s) in {replacedTabs} tab(s)."
            : "No matches found for Replace All.";
        _dialogService.ShowMessage(message, "Replace All Result");
    }

    [RelayCommand]
    private void ShowStandardView()
    {
        IsStandardView = true;
        IsFolderExplorerVisible = false;
    }

    [RelayCommand]
    private void ShowFolderExplorerView()
    {
        IsFolderExplorerVisible = true;
        IsStandardView = false;
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

    [RelayCommand(CanExecute = nameof(CanRunDirectoryCommand))]
    private void NewFileInFolder(TreeNodeViewModel? node)
    {
        if (node == null)
        {
            return;
        }

        var newFilePath = BuildNextNewFilePath(node.FullPath);

        try
        {
            _folderService.CreateFile(newFilePath);
            node.Refresh();
            node.IsExpanded = true;
        }
        catch (Exception ex)
        {
            _dialogService.ShowMessage($"Error creating file: {ex.Message}", "Error");
        }
    }

    [RelayCommand(CanExecute = nameof(CanRunDirectoryCommand))]
    private void CopyPath(TreeNodeViewModel? node)
    {
        if (node == null)
        {
            return;
        }

        _clipboardService.SetText(node.FullPath);
    }

    [RelayCommand(CanExecute = nameof(CanRunDirectoryCommand))]
    private void CopyFolder(TreeNodeViewModel? node)
    {
        if (node == null)
        {
            return;
        }

        _copiedFolderPath = node.FullPath;
        PasteFolderCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand(CanExecute = nameof(CanPasteFolder))]
    private void PasteFolder(TreeNodeViewModel? targetNode)
    {
        if (targetNode == null || string.IsNullOrWhiteSpace(_copiedFolderPath))
        {
            return;
        }

        var sourcePath = _copiedFolderPath;
        var targetPath = targetNode.FullPath;

        if (string.Equals(sourcePath, targetPath, StringComparison.OrdinalIgnoreCase))
        {
            _dialogService.ShowMessage("Cannot paste a folder into itself.", "Invalid Operation");
            return;
        }

        if (targetPath.StartsWith(sourcePath + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
        {
            _dialogService.ShowMessage("Cannot paste a folder into one of its subfolders.", "Invalid Operation");
            return;
        }

        var sourceName = Path.GetFileName(sourcePath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        if (string.IsNullOrWhiteSpace(sourceName))
        {
            _dialogService.ShowMessage("This folder cannot be copied.", "Invalid Operation");
            return;
        }

        var destinationPath = BuildUniqueFolderPath(targetPath, sourceName);

        try
        {
            _folderService.CopyFolder(sourcePath, destinationPath);
            targetNode.Refresh();
            targetNode.IsExpanded = true;
        }
        catch (Exception ex)
        {
            _dialogService.ShowMessage($"Error pasting folder: {ex.Message}", "Error");
        }
    }

    [RelayCommand]
    private void ShowAbout()
    {
        _dialogService.ShowAboutDialog(AboutStudentName, AboutGroupName, AboutInstitutionalEmail);
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

    private IEnumerable<EditorDocument> GetSearchTargets()
    {
        if (IsSearchScopeSelectedTab)
        {
            if (SelectedDocument != null)
            {
                yield return SelectedDocument;
            }

            yield break;
        }

        foreach (var document in Documents)
        {
            yield return document;
        }
    }

    private bool CanRunDirectoryCommand(TreeNodeViewModel? node)
    {
        return node is { IsDirectory: true } && !string.IsNullOrWhiteSpace(node.FullPath);
    }

    private bool CanPasteFolder(TreeNodeViewModel? node)
    {
        return CanRunDirectoryCommand(node)
            && !string.IsNullOrWhiteSpace(_copiedFolderPath)
            && _folderService.DirectoryExists(_copiedFolderPath);
    }

    private string BuildNextNewFilePath(string folderPath)
    {
        var counter = 1;
        while (true)
        {
            var candidate = Path.Combine(folderPath, $"NewFile{counter}.txt");
            if (!_fileService.FileExists(candidate))
            {
                return candidate;
            }

            counter++;
        }
    }

    private string BuildUniqueFolderPath(string targetPath, string sourceName)
    {
        var candidate = Path.Combine(targetPath, sourceName);
        if (!_folderService.DirectoryExists(candidate))
        {
            return candidate;
        }

        var index = 1;
        while (true)
        {
            var suffixed = Path.Combine(targetPath, $"{sourceName} - Copy {index}");
            if (!_folderService.DirectoryExists(suffixed))
            {
                return suffixed;
            }

            index++;
        }
    }
}
