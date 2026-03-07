using NotepadClone.Application.Interfaces;
using System.IO;

namespace NotepadClone.Infrastructure.Services;

public class FolderService : IFolderService
{
    public string[] GetLogicalDrives()
    {
        return Directory.GetLogicalDrives();
    }

    public string[] GetDirectories(string path)
    {
        try
        {
            return Directory.GetDirectories(path);
        }
        catch
        {
            return Array.Empty<string>();
        }
    }

    public string[] GetFiles(string path)
    {
        try
        {
            return Directory.GetFiles(path);
        }
        catch
        {
            return Array.Empty<string>();
        }
    }

    public bool DirectoryExists(string path)
    {
        return Directory.Exists(path);
    }

    public void CreateDirectory(string path)
    {
        Directory.CreateDirectory(path);
    }

    public void CopyFolder(string sourcePath, string destinationPath)
    {
        if (!Directory.Exists(destinationPath))
        {
            Directory.CreateDirectory(destinationPath);
        }

        foreach (var file in Directory.GetFiles(sourcePath))
        {
            var fileName = Path.GetFileName(file);
            var destFile = Path.Combine(destinationPath, fileName);
            File.Copy(file, destFile, overwrite: true);
        }

        foreach (var dir in Directory.GetDirectories(sourcePath))
        {
            var dirName = Path.GetFileName(dir);
            var destDir = Path.Combine(destinationPath, dirName);
            CopyFolder(dir, destDir);
        }
    }

    public void CreateFile(string filePath)
    {
        File.Create(filePath).Dispose();
    }
}
