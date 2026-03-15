using CommunityToolkit.Mvvm.Input;

namespace NotepadClone.Presentation.ViewModels;

public partial class MainViewModel
{
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
                return;
            }
        }

        System.Windows.Application.Current.Shutdown();
    }
}
