using CommunityToolkit.Mvvm.Input;

namespace NotepadClone.Presentation.ViewModels;

public partial class MainViewModel
{
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
}
