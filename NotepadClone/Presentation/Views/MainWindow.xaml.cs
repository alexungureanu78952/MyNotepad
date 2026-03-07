using NotepadClone.Presentation.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;

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

    private void FolderTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        ViewModel.SelectedTreeNode = e.NewValue as TreeNodeViewModel;
    }

    private void FolderTreeView_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        var element = e.OriginalSource as DependencyObject;
        while (element != null && element is not TreeViewItem)
        {
            element = VisualTreeHelper.GetParent(element);
        }

        if (element is TreeViewItem item)
        {
            item.IsSelected = true;
            item.Focus();
        }
    }
}