using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace LoadRunnerClient
{
	/// <summary>
	/// Converts an Incoming <see cref="bool"/> to a <see cref="Visibility"/>
	/// </summary>
	public class BoolToVisibilityConverter : IValueConverter
	{
		/// <summary>
		/// Converting bool to Visibility
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="culture"></param>
		/// <returns>either <see cref="Visibility.Visible"/> or <see cref="Visibility.Collapsed"/></returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool check;
			if (value == null)
			{
				check = false;
			}
			else
			{
				check = (bool)value;
			}
			return check == true ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Convert(value, targetType, parameter, culture);
		}
	}

	/// <summary>
	/// Converts a incoming <see cref="bool"/> to its opposite
	/// </summary>
	public class InverseBooleanConverter : IValueConverter
	{

		public object Convert(object value, Type targetType, object parameter,
			System.Globalization.CultureInfo culture)
		{
			return !(bool)value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Convert(value, targetType, parameter, culture);
		}
	}

}