using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace NotepadClone.Presentation.Views;

public partial class AboutWindow : Window
{
    public AboutWindow(string studentName, string groupName, string institutionalEmail)
    {
        InitializeComponent();

        StudentNameText.Text = studentName;
        GroupNameText.Text = groupName;

        EmailHyperlink.Inlines.Clear();
        EmailHyperlink.Inlines.Add(institutionalEmail);
        EmailHyperlink.NavigateUri = new Uri($"mailto:{institutionalEmail}");
    }

    private void EmailHyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri)
        {
            UseShellExecute = true
        });

        e.Handled = true;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
