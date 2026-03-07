namespace NotepadClone.Application.Interfaces;

public interface ITextSearchService
{
    int CountOccurrences(string source, string searchText, StringComparison comparison = StringComparison.OrdinalIgnoreCase);
    bool ReplaceFirst(string source, string searchText, string replacementText, out string result, StringComparison comparison = StringComparison.OrdinalIgnoreCase);
    int ReplaceAll(string source, string searchText, string replacementText, out string result, StringComparison comparison = StringComparison.OrdinalIgnoreCase);
}
