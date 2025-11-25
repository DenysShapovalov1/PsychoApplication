using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
namespace PsychoApp
{
    public class OptimizeDay
    {
        public DateTime OptimizeDate { get; set; }

        public Dictionary<string, List<DayTask>> TasksByBlock = new()
        {
            {"Night1", new List<DayTask>()},
            {"Morning", new List<DayTask>()},
            {"Noon", new List<DayTask>()},
            {"Evening", new List<DayTask>()},
            {"Night2", new List<DayTask>()}
        };

        public OptimizeDay(DateTime date) 
        {
            OptimizeDate = date;

        }

        public virtual Grid CreatedOptimizedDayBlock()
        {
            Grid OptimizeGrid = new Grid()
            {
                Width = 1250,
                Height = 906
            };
            Rectangle optimizeDayBlock = new()
            {
                Width = 1250,
                Height = 906,
                RadiusX = 24,
                RadiusY = 24,
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD7D1CD")),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Effect = new System.Windows.Media.Effects.DropShadowEffect()
                {
                    Color = Color.FromArgb(25, 0, 0, 0),
                    ShadowDepth = 4,
                    BlurRadius = 20,
                    Direction = 270,
                    Opacity = 1,
                }
            };

            Rectangle headeroptimizeDayBlock = new()
            {
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1D1D1D")),
                RadiusX = 24,
                RadiusY = 24,
                Width= 1250,
                Height = 62,
                VerticalAlignment= VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            TextBlock dateText = new TextBlock
            {
                Text =  OptimizeDate.ToString("dddd, d MMMM"),
                Foreground = Brushes.White,
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, 15, 0, 0),
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Kaisei Tokumin"),
            };

            Separator nightBlockTimeSeparator = new Separator()
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1D1D1D")),
                Margin = new Thickness(0,  0, 0, 650),
                Width = 725,
                HorizontalAlignment = HorizontalAlignment.Left,
            };
            TextBlock nightTimeBlock = new()
            {
                Text = "0:00 - 5:59(Night)",
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(5, 100, 0, 0),
                FontSize = 18,
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Kaisei Tokumin"),
            };

            Separator morningBlockTimeSeparator = new Separator()
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1D1D1D")),
                Margin = new Thickness(0, 0, 0, 350),
                Width = 725,
                HorizontalAlignment = HorizontalAlignment.Left,
            };
            TextBlock morningTimeBlock = new()
            {
                Text = "6:00 - 11:59(Morning)",
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(5, 250, 0, 0),
                FontSize = 18,
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Kaisei Tokumin"),
            };


            Separator noonBlockTimeSeparator = new Separator()
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1D1D1D")),
                Margin = new Thickness(0, 0, 0, 50),
                Width = 725,
                HorizontalAlignment = HorizontalAlignment.Left,
            };
            TextBlock noonTimeBlock = new()
            {
                Text = "12:00 - 16:59(Day/Noon)",
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(5, 400, 0, 0),
                FontSize = 18,
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Kaisei Tokumin"),
            };

            Separator eveningBlockTimeSeparator = new Separator()
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1D1D1D")),
                Margin = new Thickness(0, 0, 0, -250),
                Width = 725,
                HorizontalAlignment = HorizontalAlignment.Left,
            };
            TextBlock eveningTimeBlock = new()
            {
                Text = "17:00 - 10:59(Evening)",
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(5, 550, 0, 0),
                FontSize = 18,
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Kaisei Tokumin"),
            };


            Separator nightSecBlockTimeSeparator = new Separator()
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1D1D1D")),
                Margin = new Thickness(0, 0, 0, -550),
                Width = 725,
                HorizontalAlignment = HorizontalAlignment.Left,
            };
            TextBlock nightSecTimeBlock = new()
            {
                Text = "21:00 - 23:59(Night)",
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(5, 700, 0, 0),
                FontSize = 18,
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Kaisei Tokumin"),
            };


            OptimizeGrid.Children.Add(optimizeDayBlock);
            OptimizeGrid.Children.Add(headeroptimizeDayBlock);
            OptimizeGrid.Children.Add(dateText);
            OptimizeGrid.Children.Add(nightBlockTimeSeparator);
            OptimizeGrid.Children.Add(nightTimeBlock);
            OptimizeGrid.Children.Add(morningBlockTimeSeparator);
            OptimizeGrid.Children.Add(morningTimeBlock);
            OptimizeGrid.Children.Add(noonBlockTimeSeparator);
            OptimizeGrid.Children.Add(noonTimeBlock);
            OptimizeGrid.Children.Add(eveningBlockTimeSeparator);
            OptimizeGrid.Children.Add(eveningTimeBlock);
            OptimizeGrid.Children.Add(nightSecBlockTimeSeparator);
            OptimizeGrid.Children.Add(nightSecTimeBlock);

            return OptimizeGrid;

        }
    }
}
