using CommunityToolkit.Mvvm.Input;
using NotepadClone.Domain;

namespace NotepadClone.Presentation.ViewModels;

public partial class MainViewModel
{
    [RelayCommand(CanExecute = nameof(CanExecuteFileCommand))]
    private void Find()
    {
        var searchText = _dialogService.ShowInputDialog("Find what:", "Find");
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return;
        }

        var targets = GetSearchTargets().ToList();
        if (targets.Count == 0)
        {
            return;
        }

        var totalOccurrences = 0;
        var matchedDocs = new List<string>();
        EditorDocument? firstMatchDocument = null;
        var firstMatchIndex = -1;

        foreach (var document in targets)
        {
            var firstIndexInDocument = document.Text.IndexOf(searchText, StringComparison.OrdinalIgnoreCase);
            if (firstIndexInDocument >= 0 && firstMatchDocument == null)
            {
                firstMatchDocument = document;
                firstMatchIndex = firstIndexInDocument;
            }

            var occurrences = _textSearchService.CountOccurrences(document.Text, searchText);
            if (occurrences > 0)
            {
                totalOccurrences += occurrences;
                matchedDocs.Add($"{document.Title}: {occurrences}");
            }
        }

        if (firstMatchDocument != null)
        {
            SelectedDocument = firstMatchDocument;
            firstMatchDocument.RequestFindSelection(firstMatchIndex, searchText.Length);
        }

        if (IsSearchScopeSelectedTab)
        {
            var message = totalOccurrences > 0
                ? $"Found {totalOccurrences} occurrence(s) in '{targets[0].Title}'."
                : $"No matches found in '{targets[0].Title}'.";
            _dialogService.ShowMessage(message, "Find Result");
            return;
        }

        var allTabsMessage = totalOccurrences > 0
            ? $"Found {totalOccurrences} occurrence(s) in {matchedDocs.Count} tab(s).\n\n{string.Join("\n", matchedDocs)}"
            : "No matches found in open tabs.";
        _dialogService.ShowMessage(allTabsMessage, "Find Result");
    }

    [RelayCommand(CanExecute = nameof(CanExecuteFileCommand))]
    private void Replace()
    {
        var searchText = _dialogService.ShowInputDialog("Find what:", "Replace");
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return;
        }

        var replacementText = _dialogService.ShowInputDialog("Replace with:", "Replace", string.Empty) ?? string.Empty;

        var targets = GetSearchTargets().ToList();
        if (targets.Count == 0)
        {
            return;
        }

        var replacedInTabs = 0;
        foreach (var document in targets)
        {
            if (_textSearchService.ReplaceFirst(document.Text, searchText, replacementText, out var updatedText))
            {
                document.Text = updatedText;
                document.IsDirty = true;
                replacedInTabs++;

                if (IsSearchScopeSelectedTab)
                {
                    break;
                }
            }
        }

        var message = replacedInTabs > 0
            ? IsSearchScopeSelectedTab
                ? "One occurrence replaced in selected tab."
                : $"One occurrence replaced in {replacedInTabs} tab(s)."
            : "No match found to replace.";
        _dialogService.ShowMessage(message, "Replace Result");
    }

    [RelayCommand(CanExecute = nameof(CanExecuteFileCommand))]
    private void ReplaceAll()
    {
        var searchText = _dialogService.ShowInputDialog("Find what:", "Replace All");
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return;
        }

        var replacementText = _dialogService.ShowInputDialog("Replace with:", "Replace All", string.Empty) ?? string.Empty;

        var targets = GetSearchTargets().ToList();
        if (targets.Count == 0)
        {
            return;
        }

        var replacedTabs = 0;
        var totalReplacements = 0;

        foreach (var document in targets)
        {
            var replacedCount = _textSearchService.ReplaceAll(document.Text, searchText, replacementText, out var updatedText);
            if (replacedCount > 0)
            {
                document.Text = updatedText;
                document.IsDirty = true;
                replacedTabs++;
                totalReplacements += replacedCount;
            }
        }

        var message = totalReplacements > 0
            ? IsSearchScopeSelectedTab
                ? $"Replaced {totalReplacements} occurrence(s) in selected tab."
                : $"Replaced {totalReplacements} occurrence(s) in {replacedTabs} tab(s)."
            : "No matches found for Replace All.";
        _dialogService.ShowMessage(message, "Replace All Result");
    }

    private IEnumerable<EditorDocument> GetSearchTargets()
    {
        if (IsSearchScopeSelectedTab)
        {
            if (SelectedDocument != null)
            {
                yield return SelectedDocument;
            }

            yield break;
        }

        foreach (var document in Documents)
        {
            yield return document;
        }
    }
}
