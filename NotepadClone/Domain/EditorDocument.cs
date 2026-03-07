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

    public string DisplayTitle => IsDirty ? $"*{Title}" : Title;

    partial void OnIsDirtyChanged(bool value)
    {
        OnPropertyChanged(nameof(DisplayTitle));
    }

    partial void OnTitleChanged(string value)
    {
        OnPropertyChanged(nameof(DisplayTitle));
    }
}
