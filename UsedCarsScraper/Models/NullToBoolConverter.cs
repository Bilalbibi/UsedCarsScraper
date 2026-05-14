using System.Globalization;
using System.Windows.Data;

namespace UsedCarsScraper.Models
{
    public class NullToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null; // Returns true if Make is selected, false otherwise
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}