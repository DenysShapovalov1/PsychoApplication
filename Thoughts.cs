using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;


namespace PsychoApp
{
    class Thoughts : ThoughtBlock
    {

        public int Id { get; set; }
        private string name = "My";
        private DateTime createDate = DateTime.Now;
        public DateTime CreateDate => createDate;
        public Grid ThoughtBlock { get; set; }

        public event Action<Thoughts> OnThoughtSelected;
        public event Action<Thoughts> OnThoughtDeleted;

        public Thoughts() { }

        public Thoughts(int id)
        {
            Id = id;
            name = $"My {Id} thought";
            createDate = DateTime.Now;

            title = name;

            ThoughtBlock = CreateDefaultThoughtBlock();
        }

        public Grid CreateThoughtRectangle()
        {
            Grid grid = new()
            {
                Width = 258,
                Height = 43,
                DataContext = this

            };
            Rectangle thoughtRectangle = new()
            {
                Width = 258,
                Height = 43,
                Fill = new SolidColorBrush(Color.FromArgb(53, 146, 146, 146)),
                RadiusX = 6,
                RadiusY = 6,
                Margin = new Thickness(1),
                Cursor = System.Windows.Input.Cursors.Hand
            };
            TextBlock textBlock = new()
            {
                Text = this.name,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10, 0, 0, 0),
                Foreground = Brushes.White,
                FontSize = 18,
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Kaisei Tokumin"),
                Cursor = System.Windows.Input.Cursors.Hand
            };
            TextBox nameTextBox = new()
            {
                Text = this.name,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10, 0, 0, 0),
                Foreground = Brushes.Black,
                FontSize = 18,
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Kaisei Tokumin"),
                Cursor = System.Windows.Input.Cursors.Hand,
                Visibility = Visibility.Collapsed
            };
            Button editNameBtn = new()
            {
                Width = 20,
                Height = 20,
                FontFamily = new FontFamily("Segoe Fluent Icons"),
                FontSize = 18,
                Content = "\ue70f",
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6A6A6A")),
                BorderBrush = Brushes.Transparent,
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, 30, 0),
                Cursor = System.Windows.Input.Cursors.Hand,
            };
            Button deleteBtn = new()
            {
                Width = 20,
                Height = 20,
                FontFamily = new FontFamily("Segoe Fluent Icons"),
                FontSize = 18,
                Content = "\uE74D",
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF5050")),
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, 7, 0),
                Cursor = System.Windows.Input.Cursors.Hand
            };
            grid.Children.Add(thoughtRectangle);
            grid.Children.Add(textBlock);
            grid.Children.Add(editNameBtn);
            grid.Children.Add(deleteBtn);
            grid.Children.Add(nameTextBox);

            grid.MouseLeftButtonUp += (s, e) => OnThoughtSelected?.Invoke(this);

            editNameBtn.Click += (s, e) =>
            {
                e.Handled = true;
                nameTextBox.Text = this.name;
                nameTextBox.Text = this.title;
                nameTextBox.Visibility = Visibility.Visible;
                nameTextBox.Focus();
                nameTextBox.SelectAll();
            };

            nameTextBox.LostFocus += (s, e) =>
            {
                this.name = nameTextBox.Text;
                this.title = nameTextBox.Text;
                nameTextBox.Visibility = Visibility.Collapsed;
                textBlock.Text = this.name;
                titleText.Text = this.title;

            };

            deleteBtn.Click += (s, e) =>
            {
                e.Handled = true;
                OnThoughtDeleted?.Invoke(this);
            };

            return grid;
        }

    }
}
