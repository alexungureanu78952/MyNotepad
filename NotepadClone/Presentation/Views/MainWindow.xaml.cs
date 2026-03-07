using NotepadClone.Presentation.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace NotepadClone.Presentation.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private MainViewModel ViewModel => (MainViewModel)DataContext;

    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    private void FolderTreeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        // Get the selected node and invoke command
        if (FolderTreeView.SelectedItem is TreeNodeViewModel node)
        {
            ViewModel.TreeNodeDoubleClickCommand.Execute(node);
        }
    }
}