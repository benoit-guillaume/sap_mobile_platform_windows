using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace RKT_JSONService.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return Visibility.Collapsed;
            }

            return ((bool)value) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class CarrierIdToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return string.Empty;
            }

            var airlineId = (string)value;
            switch (airlineId)
            {
                case "AA":
                    return "Images/AA.png";
                case "AB":
                    return "Images/AB.jpg";
                case "AC":
                    return "Images/AC.jpg";
                case "AF":
                    return "Images/AF.png";
                case "AZ":
                    return "Images/AZ.jpg";
                case "BA":
                    return "Images/BA.png";
                case "CO":
                    return "Images/CO.jpg";
                case "DL":
                    return "Images/DL.jpg";
                case "FJ":
                    return "Images/FJ.jpg";
                case "JL":
                    return "Images/JL.jpg";
                case "LH":
                    return "Images/LH.png";
                case "NG":
                    return "Images/NG.png";
                case "NW":
                    return "Images/NW.jpg";
                case "QF":
                    return "Images/QF.png";
                case "SA":
                    return "Images/SA.png";
                case "SQ":
                    return "Images/SQ.jpg";
                case "SR":
                    return "Images/SR.png";
                case "UA":
                    return "Images/UA.jpg";

                default:
                    return "Images/UA.jpg";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToInverseVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return Visibility.Visible;
            }

            return ((bool)value) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToInverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return !((bool)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
