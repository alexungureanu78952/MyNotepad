namespace NotepadClone.Application.Interfaces;

/// <summary>
/// Service for file I/O operations.
/// </summary>
public interface IFileService
{
    /// <summary>
    /// Reads text from a file.
    /// </summary>
    string ReadFile(string filePath);

    /// <summary>
    /// Writes text to a file.
    /// </summary>
    void WriteFile(string filePath, string content);

    /// <summary>
    /// Checks if a file exists.
    /// </summary>
    bool FileExists(string filePath);
}
