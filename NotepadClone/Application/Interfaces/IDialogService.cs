namespace NotepadClone.Application.Interfaces;

/// <summary>
/// Service for showing dialogs (save, open, message boxes).
/// </summary>
public interface IDialogService
{
    /// <summary>
    /// Shows an Open File dialog. Returns selected file path or null if canceled.
    /// </summary>
    string? ShowOpenFileDialog(string filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*", string defaultExt = ".txt");

    /// <summary>
    /// Shows a Save File dialog. Returns selected file path or null if canceled.
    /// </summary>
    string? ShowSaveFileDialog(string filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*", string defaultExt = ".txt");

    /// <summary>
    /// Shows a message box with Yes/No/Cancel buttons. Returns the user's choice.
    /// </summary>
    bool? ShowYesNoCancelDialog(string message, string title);

    /// <summary>
    /// Shows an information message box.
    /// </summary>
    void ShowMessage(string message, string title);
}
