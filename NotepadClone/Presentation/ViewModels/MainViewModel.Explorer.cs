using CommunityToolkit.Mvvm.Input;
using System.IO;

namespace NotepadClone.Presentation.ViewModels;

public partial class MainViewModel
{
    private void InitializeFolderTree()
    {
        try
        {
            FolderTreeRoots.Clear();
            var drives = _folderService.GetLogicalDrives();
            foreach (var drive in drives)
            {
                try
                {
                    FolderTreeRoots.Add(new TreeNodeViewModel(drive, true, _folderService, _expandedFolderPaths, SaveUiState));
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
    private void TreeNodeDoubleClick(TreeNodeViewModel? node)
    {
        if (node == null || node.IsDirectory || string.IsNullOrEmpty(node.FullPath))
        {
            return;
        }

        OpenFileInTab(node.FullPath);
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
            SaveUiState();
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
            SaveUiState();
        }
        catch (Exception ex)
        {
            _dialogService.ShowMessage($"Error pasting folder: {ex.Message}", "Error");
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
