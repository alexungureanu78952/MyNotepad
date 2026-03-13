namespace NotepadClone.Domain;

public class UiState
{
    public bool IsFolderExplorerVisible { get; set; } = true;
    public List<string> ExpandedFolderPaths { get; set; } = new();
}
