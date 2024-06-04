using Syncfusion.UI.Xaml.Charts;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ForestSampleWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }

    public class ForestDataModel
    {
        public string? Country { get; set; }

        public double Value { get; set; }

        public double Percentage { get; set; }
    }

    public class ForestDataViewModel
    {
        public List<ForestDataModel> ForestDatas { get; set; }

        public List<Brush> CustomPaletteBrushes { get; set; }

        public ForestDataViewModel()
        {
            ForestDatas = new List<ForestDataModel>();

            ReadCSV();

            CustomPaletteBrushes = new List<Brush>
            {
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3269a8")), // Russia
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#63b84d")), // Brazil
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8f2214")), // Canada
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5a8fd9")), // U.S.
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#dd5d3f")), // China
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#446a7f")), // Australia
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#39536e")), // DRC
                //new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e87d3d")), // Indonesia
                //new SolidColorBrush((Color)ColorConverter.ConvertFromString("#febc38")), // Mexico
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5a8fd9")), // Colombia
                //new SolidColorBrush((Color)ColorConverter.ConvertFromString("#de6844")), // Bolivia
                //new SolidColorBrush((Color)ColorConverter.ConvertFromString("#64b44e")), // Venezuela
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#542823")), // Tanzania
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#73523c")), // Other
            };


        }

        public void ReadCSV()
        {
            Assembly executingAssembly = typeof(App).GetTypeInfo().Assembly;
            Stream inputStream = executingAssembly.GetManifestResourceStream("ForestSampleWPF.ForestData.csv");
            List<string> lines = new List<string>();
            if (inputStream != null)
            {
                string line;
                StreamReader reader = new StreamReader(inputStream);
                while ((line = reader.ReadLine()) != null)
                {
                    lines.Add(line);
                }
                lines.RemoveAt(0);

                double otherValues = 0;
                double totalValue = 0;
                foreach (var dataPoint in lines)
                {
                    string[] data = dataPoint.Split(',');

                    var value = double.Parse(data[3]);
                    if (value >= 915000)
                    {
                        ForestDatas.Add(new ForestDataModel() { Country = data[1], Value = value });
                        totalValue =  totalValue + value;
                    }
                    else
                    {
                        otherValues = otherValues + value;
                    }
                }

                totalValue = totalValue + otherValues;
                ForestDatas = ForestDatas.OrderByDescending(data => data.Value).ToList();

                ForestDatas.Add(new ForestDataModel() { Country = "Others", Value = otherValues });

                AddPercentage(totalValue);
            }
        }

        private void AddPercentage(double totalValue)
        {
            foreach (var dataPoint in ForestDatas)
            {
                var percentage = (dataPoint.Value / totalValue * 100);
                dataPoint.Percentage = percentage;
            }
        }
    }

    public class Convertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var countryName = (string)value;
            var stackpannel = parameter as StackPanel;

            if (countryName != null)
            {
                if (countryName != "Others")
                {
                    var image =  countryName.ToLower() + ".png";
                    return image;
                }
            }

            return string.Empty;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }

    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string label = parameter as string;
            if (label != "image")
            {
                return value?.ToString() == "Others" ? Visibility.Visible : Visibility.Collapsed;
            }
            return value?.ToString() == "Others" ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}