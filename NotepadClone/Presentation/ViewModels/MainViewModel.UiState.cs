using NotepadClone.Domain;

namespace NotepadClone.Presentation.ViewModels;

public partial class MainViewModel
{
    private void RestoreUiState()
    {
        var uiState = _uiStateService.Load();

        IsFolderExplorerVisible = uiState.IsFolderExplorerVisible;
        IsStandardView = !uiState.IsFolderExplorerVisible;

        _expandedFolderPaths.Clear();
        foreach (var path in uiState.ExpandedFolderPaths.Where(path => !string.IsNullOrWhiteSpace(path)))
        {
            _expandedFolderPaths.Add(path);
        }
    }

    private void SaveUiState()
    {
        _uiStateService.Save(new UiState
        {
            IsFolderExplorerVisible = IsFolderExplorerVisible,
            ExpandedFolderPaths = _expandedFolderPaths.OrderBy(path => path).ToList()
        });
    }
}
