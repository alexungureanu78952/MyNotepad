using NotepadClone.Application.Interfaces;
using System.Windows;

namespace NotepadClone.Infrastructure.Services;

public class ClipboardService : IClipboardService
{
    public void SetText(string text)
    {
        Clipboard.SetText(text);
    }

    public string? GetText()
    {
        return Clipboard.ContainsText() ? Clipboard.GetText() : null;
    }
}
