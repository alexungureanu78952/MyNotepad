using Microsoft.Win32;
using NotepadClone.Application.Interfaces;
using System.Windows;

namespace NotepadClone.Infrastructure.Services;

public class DialogService : IDialogService
{
    public string? ShowOpenFileDialog(string filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*", string defaultExt = ".txt")
    {
        var dialog = new OpenFileDialog
        {
            Filter = filter,
            DefaultExt = defaultExt
        };

        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }

    public string? ShowSaveFileDialog(string filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*", string defaultExt = ".txt")
    {
        var dialog = new SaveFileDialog
        {
            Filter = filter,
            DefaultExt = defaultExt
        };

        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }

    public bool? ShowYesNoCancelDialog(string message, string title)
    {
        var result = MessageBox.Show(message, title, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

        return result switch
        {
            MessageBoxResult.Yes => true,
            MessageBoxResult.No => false,
            _ => null
        };
    }

    public void ShowMessage(string message, string title)
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
