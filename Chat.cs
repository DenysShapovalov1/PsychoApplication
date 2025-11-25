using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PsychoApp.ChatAI
{
    public class Chat
    {

        private static int counterID = 0;
        public int Id { get; set; } = 0;

        public string Name { get; set; }

        public List<object> History { get; set; } = new List<object>();

        public event Action<Chat> OnChatSelected;
        public event Action<Chat> OnChatDeleted;

        public Chat()
        {
            Id = counterID++;
        }

        public Grid CreateChatRectangle(Chat chat)
        {

            var currentChat = chat;

            Grid grid = new()
            {
                Width = 258,
                Height = 43,
                Tag = chat,   
                Cursor = Cursors.Hand
            };

            Rectangle bg = new()
            {
                Width = 258,
                Height = 43,
                Fill = new SolidColorBrush(Color.FromArgb(53, 146, 146, 146)),
                RadiusX = 6,
                RadiusY = 6,
                Margin = new Thickness(1)
            };

            TextBlock textBlock = new()
            {
                Text = this.Name,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10, 0, 0, 0),
                Foreground = Brushes.White,
                FontSize = 18,
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Kaisei Tokumin"),
                Cursor = System.Windows.Input.Cursors.Hand
            };

            TextBox editBox = new()
            {
                Text = chat.Name,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10, 0, 0, 0),
                Foreground = Brushes.Black,
                FontSize = 18,
                FontFamily = new FontFamily("Segoe UI"),
                Visibility = Visibility.Collapsed
            };

            Button editBtn = new()
            {
                Width = 20,
                Height = 20,
                FontFamily = new FontFamily("Segoe Fluent Icons"),
                FontSize = 18,
                Content = "\uE70F",
                Foreground = new SolidColorBrush(Color.FromRgb(106, 106, 106)),
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, 30, 0),
                Cursor = Cursors.Hand
            };

            Button deleteBtn = new()
            {
                Width = 20,
                Height = 20,
                FontFamily = new FontFamily("Segoe Fluent Icons"),
                FontSize = 18,
                Content = "\uE74D",
                Foreground = new SolidColorBrush(Color.FromRgb(255, 80, 80)),
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, 7, 0),
                Cursor = Cursors.Hand
            };

            grid.Children.Add(bg);
            grid.Children.Add(textBlock);
            grid.Children.Add(editBtn);
            grid.Children.Add(deleteBtn);
            grid.Children.Add(editBox);

            textBlock.IsHitTestVisible = false;
            editBox.IsHitTestVisible = false;

            bg.MouseLeftButtonUp += (s, e) =>
            {
                OnChatSelected?.Invoke(currentChat);
            };
            textBlock.Text = currentChat.Name;
            editBox.Text = currentChat.Name;

            editBtn.Click += (s, e) =>
            {
                editBox.Text = currentChat.Name;
                editBox.Visibility = Visibility.Visible;
                editBox.Focus();
                editBox.SelectAll();
            };

            editBox.LostFocus += (s, e) =>
            {
                currentChat.Name = editBox.Text;
                textBlock.Text = currentChat.Name;
                editBox.Visibility = Visibility.Collapsed;
            };

            deleteBtn.Click += (s, e) =>
            {
                e.Handled = true;
                OnChatDeleted?.Invoke(currentChat);
            };

            return grid;
        }
    }
}
