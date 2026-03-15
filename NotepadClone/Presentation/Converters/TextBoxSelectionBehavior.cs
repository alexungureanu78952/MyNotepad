using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace NotepadClone.Presentation.Converters;

public static class TextBoxSelectionBehavior
{
    public static readonly DependencyProperty SelectionStartProperty =
        DependencyProperty.RegisterAttached(
            "SelectionStart",
            typeof(int),
            typeof(TextBoxSelectionBehavior),
            new PropertyMetadata(-1));

    public static readonly DependencyProperty SelectionLengthProperty =
        DependencyProperty.RegisterAttached(
            "SelectionLength",
            typeof(int),
            typeof(TextBoxSelectionBehavior),
            new PropertyMetadata(0));

    public static readonly DependencyProperty SelectionRequestIdProperty =
        DependencyProperty.RegisterAttached(
            "SelectionRequestId",
            typeof(int),
            typeof(TextBoxSelectionBehavior),
            new PropertyMetadata(0, OnSelectionRequestIdChanged));

    public static int GetSelectionStart(DependencyObject obj) => (int)obj.GetValue(SelectionStartProperty);
    public static void SetSelectionStart(DependencyObject obj, int value) => obj.SetValue(SelectionStartProperty, value);

    public static int GetSelectionLength(DependencyObject obj) => (int)obj.GetValue(SelectionLengthProperty);
    public static void SetSelectionLength(DependencyObject obj, int value) => obj.SetValue(SelectionLengthProperty, value);

    public static int GetSelectionRequestId(DependencyObject obj) => (int)obj.GetValue(SelectionRequestIdProperty);
    public static void SetSelectionRequestId(DependencyObject obj, int value) => obj.SetValue(SelectionRequestIdProperty, value);

    private static void OnSelectionRequestIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not TextBox textBox)
        {
            return;
        }

        var start = GetSelectionStart(textBox);
        var length = GetSelectionLength(textBox);

        if (start < 0 || length <= 0)
        {
            return;
        }

        textBox.Dispatcher.BeginInvoke(new Action(() =>
        {
            if (start > textBox.Text.Length)
            {
                return;
            }

            var safeLength = Math.Min(length, textBox.Text.Length - start);
            if (safeLength <= 0)
            {
                return;
            }

            textBox.Focus();
            textBox.Select(start, safeLength);

            var lineIndex = textBox.GetLineIndexFromCharacterIndex(start);
            textBox.ScrollToLine(lineIndex);
        }), DispatcherPriority.Background);
    }
}
