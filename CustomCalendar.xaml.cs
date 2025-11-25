using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PsychoApp
{
    /// <summary>
    /// Логика взаимодействия для CustomCalendar.xaml
    /// </summary>
    public partial class CustomCalendar : UserControl
    {
        private DateTime _currentDate = DateTime.Today;

        public event Action<DateTime> OnDateSelected;

        public CustomCalendar()
        {
            InitializeComponent();
            BuildCalendar();
        }

        private void BuildCalendar()
        {
            DaysGrid.Children.Clear();
            MonthText.Text = _currentDate.ToString("MMMM yyyy");

            DateTime first = new DateTime(_currentDate.Year, _currentDate.Month, 1);
            int skip = (int)first.DayOfWeek;
            if (skip == 0) skip = 7;

            for (int i = 1; i < skip; i++)
                DaysGrid.Children.Add(new TextBlock());

            int days = DateTime.DaysInMonth(_currentDate.Year, _currentDate.Month);

            for (int day = 1; day <= days; day++)
            {
                int currentDay = day;
                Button b = new Button
                {
                    Content = day.ToString(),
                    Width = 26,
                    Height = 26,
                    BorderBrush = Brushes.Black,
                    Background = Brushes.Transparent,
                    Cursor = Cursors.Hand,
                    Style = FindResource("RoundButton") as Style
                };

                if (new DateTime(_currentDate.Year, _currentDate.Month, day) == _currentDate)
                {
                    b.Background = new SolidColorBrush(Color.FromRgb(180, 230, 220));
                }

                b.Click += (s, e) =>
                {
                    _currentDate = new DateTime(_currentDate.Year, _currentDate.Month, currentDay);
                    OnDateSelected?.Invoke(_currentDate);
                    BuildCalendar();
                };

                DaysGrid.Children.Add(b);

            }
        }

        private void PrevMonth_Click(object sender, RoutedEventArgs e)
        {
            _currentDate = new DateTime(_currentDate.Year, _currentDate.Month, 1).AddMonths(-1);
            BuildCalendar();
        }

        private void NextMonth_Click(object sender, RoutedEventArgs e)
        {
            _currentDate = new DateTime(_currentDate.Year, _currentDate.Month, 1).AddMonths(1);
            BuildCalendar();
        }
    }
}
