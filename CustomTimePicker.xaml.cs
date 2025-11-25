using System;
using System.Windows;
using System.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PsychoApp
{
    public partial class CustomTimePicker : UserControl
    {
        public int Hour { get; private set; } = 12;
        public int Minute { get; private set; } = 0;

        public CustomTimePicker()
        {
            InitializeComponent();
            UpdateUI();
        }

        private void UpdateUI()
        {
            HourText.Text = Hour.ToString("00");
            MinuteText.Text = Minute.ToString("00");
        }

        private void HourUp_Click(object sender, RoutedEventArgs e)
        {
            Hour = (Hour + 1) % 24;
            UpdateUI();
        }

        private void HourDown_Click(object sender, RoutedEventArgs e)
        {
            Hour = (Hour == 0 ? 23 : Hour - 1);
            UpdateUI();
        }

        private void MinuteUp_Click(object sender, RoutedEventArgs e)
        {
            Minute = (Minute + 1) % 60;
            UpdateUI();
        }

        private void MinuteDown_Click(object sender, RoutedEventArgs e)
        {
            Minute = (Minute == 0 ? 59 : Minute - 1);
            UpdateUI();
        }

        public TimeSpan GetTime() => new TimeSpan(Hour, Minute, 0);
    }
}
