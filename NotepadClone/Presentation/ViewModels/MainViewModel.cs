using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NotepadClone.Application.Interfaces;
using NotepadClone.Domain;
using System.Collections.ObjectModel;

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

        // Create initial empty tab
        CreateNewDocument();
    }

    [RelayCommand]
    private void CreateNewDocument()
    {
        var doc = new EditorDocument
        {
            Title = $"File {_fileCounter++}"
        };
        Documents.Add(doc);
        SelectedDocument = doc;
    }

    [RelayCommand]
    private void Exit()
    {
        System.Windows.Application.Current.Shutdown();
    }
}
