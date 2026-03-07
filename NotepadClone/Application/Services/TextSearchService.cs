using NotepadClone.Application.Interfaces;
using System.Text;

namespace NotepadClone.Application.Services;

public class TextSearchService : ITextSearchService
{
    public int CountOccurrences(string source, string searchText, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(searchText))
        {
            return 0;
        }

        var count = 0;
        var index = 0;

        while (index <= source.Length - searchText.Length)
        {
            var foundIndex = source.IndexOf(searchText, index, comparison);
            if (foundIndex < 0)
            {
                break;
            }

            count++;
            index = foundIndex + searchText.Length;
        }

        return count;
    }

    public bool ReplaceFirst(string source, string searchText, string replacementText, out string result, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        result = source;

        if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(searchText))
        {
            return false;
        }

        var foundIndex = source.IndexOf(searchText, comparison);
        if (foundIndex < 0)
        {
            return false;
        }

        result = string.Concat(
            source.AsSpan(0, foundIndex),
            replacementText,
            source.AsSpan(foundIndex + searchText.Length));

        return true;
    }

    public int ReplaceAll(string source, string searchText, string replacementText, out string result, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        result = source;

        if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(searchText))
        {
            return 0;
        }

        var sb = new StringBuilder(source.Length);
        var count = 0;
        var currentIndex = 0;

        while (currentIndex <= source.Length - searchText.Length)
        {
            var foundIndex = source.IndexOf(searchText, currentIndex, comparison);
            if (foundIndex < 0)
            {
                break;
            }

            sb.Append(source, currentIndex, foundIndex - currentIndex);
            sb.Append(replacementText);
            currentIndex = foundIndex + searchText.Length;
            count++;
        }

        if (count == 0)
        {
            return 0;
        }

        sb.Append(source, currentIndex, source.Length - currentIndex);
        result = sb.ToString();
        return count;
    }
}
