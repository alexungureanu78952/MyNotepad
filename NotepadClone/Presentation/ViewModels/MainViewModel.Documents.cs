using CommunityToolkit.Mvvm.Input;
using NotepadClone.Domain;
using System.IO;

namespace NotepadClone.Presentation.ViewModels;

public partial class MainViewModel
{
    [RelayCommand]
    private void CreateNewDocument()
    {
        var doc = new EditorDocument
        {
            Title = $"File {_fileCounter++}"
        };

        AttachDocumentTracking(doc);
        Documents.Add(doc);
        SelectedDocument = doc;
    }

    [RelayCommand]
    private void OpenFile()
    {
        var filePath = _dialogService.ShowOpenFileDialog();
        if (filePath == null)
        {
            return;
        }

        OpenFileInTab(filePath);
    }

    [RelayCommand(CanExecute = nameof(CanExecuteFileCommand))]
    private void SaveFile()
    {
        if (SelectedDocument == null)
        {
            return;
        }

        if (string.IsNullOrEmpty(SelectedDocument.FilePath))
        {
            SaveFileAs();
            return;
        }

        TrySaveDocument(SelectedDocument, SelectedDocument.FilePath);
    }

    [RelayCommand(CanExecute = nameof(CanExecuteFileCommand))]
    private void SaveFileAs()
    {
        if (SelectedDocument == null)
        {
            return;
        }

        var filePath = _dialogService.ShowSaveFileDialog();
        if (filePath == null)
        {
            return;
        }

        TrySaveDocument(SelectedDocument, filePath);
    }

    [RelayCommand]
    private void CloseDocument(EditorDocument document)
    {
        if (document == null)
        {
            return;
        }

        if (!PromptSaveIfNeeded(document))
        {
            return;
        }

        Documents.Remove(document);

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
                return;
            }
        }

        Documents.Clear();
        CreateNewDocument();
    }

    private void OpenFileInTab(string filePath)
    {
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

            AttachDocumentTracking(doc);
            Documents.Add(doc);
            SelectedDocument = doc;
        }
        catch (Exception ex)
        {
            _dialogService.ShowMessage($"Error opening file: {ex.Message}", "Error");
        }
    }

    private void AttachDocumentTracking(EditorDocument document)
    {
        document.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(EditorDocument.Text))
            {
                document.IsDirty = true;
            }
        };
    }

    private bool TrySaveDocument(EditorDocument document, string path)
    {
        try
        {
            _fileService.WriteFile(path, document.Text);
            document.FilePath = path;
            document.Title = Path.GetFileName(path);
            document.IsDirty = false;
            return true;
        }
        catch (Exception ex)
        {
            _dialogService.ShowMessage($"Error saving file: {ex.Message}", "Error");
            return false;
        }
    }

    private bool PromptSaveIfNeeded(EditorDocument document)
    {
        if (!document.IsDirty)
        {
            return true;
        }

        var result = _dialogService.ShowYesNoCancelDialog(
            $"Do you want to save changes to '{document.Title}'?",
            "Unsaved Changes");

        if (result == null)
        {
            return false;
        }

        if (result == true)
        {
            if (!string.IsNullOrEmpty(document.FilePath))
            {
                return TrySaveDocument(document, document.FilePath);
            }

            var path = _dialogService.ShowSaveFileDialog();
            return path != null && TrySaveDocument(document, path);
        }

        return true;
    }

    private bool CanExecuteFileCommand()
    {
        return SelectedDocument != null;
    }
}
