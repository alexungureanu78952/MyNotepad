using CommunityToolkit.Mvvm.ComponentModel;
using NotepadClone.Application.Interfaces;
using NotepadClone.Domain;
using System.Collections.ObjectModel;

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
    private readonly IUiStateService _uiStateService;

    private string? _copiedFolderPath;
    private readonly HashSet<string> _expandedFolderPaths = new(StringComparer.OrdinalIgnoreCase);
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

    partial void OnIsFolderExplorerVisibleChanged(bool value)
    {
        SaveUiState();
    }

    partial void OnIsStandardViewChanged(bool value)
    {
        SaveUiState();
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
        IClipboardService clipboardService,
        IUiStateService uiStateService)
    {
        _fileService = fileService;
        _textSearchService = textSearchService;
        _dialogService = dialogService;
        _folderService = folderService;
        _clipboardService = clipboardService;
        _uiStateService = uiStateService;

        RestoreUiState();
        InitializeFolderTree();
        CreateNewDocument();
    }
}
