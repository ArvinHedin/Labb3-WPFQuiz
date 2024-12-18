using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Labb3.Converters
{
    public class FeedbackVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length >= 3 &&
                values[0] is bool showFeedback &&
                values[1] is int correctIndex &&
                values[2] is string buttonIndex)
            {
                if (showFeedback && correctIndex == int.Parse(buttonIndex.ToString()))
                    return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class WrongAnswerVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length >= 3 &&
                values[0] is bool showFeedback &&
                values[1] is int correctIndex &&
                values[2] is string buttonIndex)
            {
                if (showFeedback && correctIndex != int.Parse(buttonIndex.ToString()))
                    return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}