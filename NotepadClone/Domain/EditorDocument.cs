using CommunityToolkit.Mvvm.ComponentModel;

namespace NotepadClone.Domain;

/// <summary>
/// Represents a document in the editor (a tab with text content).
/// </summary>
public partial class EditorDocument : ObservableObject
{
    [ObservableProperty]
    private string _title = "Untitled";

    [ObservableProperty]
    private string? _filePath;

    [ObservableProperty]
    private string _text = string.Empty;

    [ObservableProperty]
    private bool _isDirty;

    [ObservableProperty]
    private bool _isReadOnly;

    [ObservableProperty]
    private int _findSelectionStart = -1;

    [ObservableProperty]
    private int _findSelectionLength;

    [ObservableProperty]
    private int _findSelectionRequestId;

    public string DisplayTitle => IsDirty ? $"*{Title}" : Title;

    partial void OnIsDirtyChanged(bool value)
    {
        OnPropertyChanged(nameof(DisplayTitle));
    }

    partial void OnTitleChanged(string value)
    {
        OnPropertyChanged(nameof(DisplayTitle));
    }

    public void RequestFindSelection(int start, int length)
    {
        if (start < 0 || length <= 0)
        {
            return;
        }

        FindSelectionStart = start;
        FindSelectionLength = length;
        FindSelectionRequestId++;
    }
}
