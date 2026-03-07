using NotepadClone.Application.Interfaces;
using System.IO;

namespace NotepadClone.Infrastructure.Services;

public class FileService : IFileService
{
    public string ReadFile(string filePath)
    {
        return File.ReadAllText(filePath);
    }

    public void WriteFile(string filePath, string content)
    {
        File.WriteAllText(filePath, content);
    }

    public bool FileExists(string filePath)
    {
        return File.Exists(filePath);
    }
}
