namespace NotepadClone.Application.Interfaces;

/// <summary>
/// Service for clipboard operations.
/// </summary>
public interface IClipboardService
{
    /// <summary>
    /// Sets text to the clipboard.
    /// </summary>
    void SetText(string text);

    /// <summary>
    /// Gets text from the clipboard.
    /// </summary>
    string? GetText();
}
