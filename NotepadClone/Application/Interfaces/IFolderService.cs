namespace NotepadClone.Application.Interfaces;

/// <summary>
/// Service for folder and directory operations.
/// </summary>
public interface IFolderService
{
    /// <summary>
    /// Gets all logical drives on the system.
    /// </summary>
    string[] GetLogicalDrives();

    /// <summary>
    /// Gets all directories in a given path.
    /// </summary>
    string[] GetDirectories(string path);

    /// <summary>
    /// Gets all files in a given path.
    /// </summary>
    string[] GetFiles(string path);

    /// <summary>
    /// Checks if a directory exists.
    /// </summary>
    bool DirectoryExists(string path);

    /// <summary>
    /// Creates a directory.
    /// </summary>
    void CreateDirectory(string path);

    /// <summary>
    /// Copies a folder recursively to a destination.
    /// </summary>
    void CopyFolder(string sourcePath, string destinationPath);

    /// <summary>
    /// Creates a new file at the specified path.
    /// </summary>
    void CreateFile(string filePath);
}
