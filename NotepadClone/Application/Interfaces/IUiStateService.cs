using NotepadClone.Domain;

namespace NotepadClone.Application.Interfaces;

public interface IUiStateService
{
    UiState Load();
    void Save(UiState state);
}
