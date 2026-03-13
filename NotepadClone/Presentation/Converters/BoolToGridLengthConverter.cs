using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NotepadClone.Presentation.Converters;

public class BoolToGridLengthConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not bool boolValue)
        {
            return new GridLength(0);
        }

        if (!boolValue)
        {
            return new GridLength(0);
        }

        if (parameter is string rawValue && double.TryParse(rawValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var width))
        {
            return new GridLength(width);
        }

        return new GridLength(1, GridUnitType.Auto);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is GridLength gridLength)
        {
            return gridLength.Value > 0;
        }

        return false;
    }
}