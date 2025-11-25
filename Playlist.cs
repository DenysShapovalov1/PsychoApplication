using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PsychoApp.MusicPlayer
{
    public class Playlist
    {
        private static int IdCounter = 0;
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Track> Tracks { get; set; } = [];

        public event Action<Playlist> OnPlaylistDeleted;
        public event Action<Playlist> OnPlaylistRenamed;


        public Grid PlaylistTileGrid;
        public TextBlock TitleBlock;
        public Border TrackListBorder;
        public ScrollViewer TrackScroll;
        public StackPanel TrackPanel;

        public event Action<Playlist>? OnPlaylistSelected;

        private void RaiseSelected()
        {
            OnPlaylistSelected?.Invoke(this);
        }


        public Playlist()
        {
            Id = IdCounter++;
            Name = $"Playlist {Id}";
            Tracks = new List<Track>();
        }

        public void AddTrack(Track track)
        {
            if (!Tracks.Contains(track))
                Tracks.Add(track);
        }

        public void RemoveTrack(Track track)
        {
            if (Tracks.Contains(track))
                Tracks.Remove(track);
        }

        public Grid CreateBlockForPlaylist()
        {
            Grid grid = new()
            {
                Height = 90,
                Width = 88,
                VerticalAlignment = VerticalAlignment.Center
            };
            Rectangle rec = new()
            {
                Height = 85,
                Width = 85,
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#313131")),
                RadiusX = 5,
                RadiusY = 5,
                Margin = new Thickness(1),
            };
            TextBlock block = new()
            {
                Text = Name,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            grid.MouseLeftButtonDown += (s, e) =>
            {
                RaiseSelected();
            };


            grid.Children.Add(rec);
            grid.Children.Add(block);
            Canvas.SetLeft(grid, 195);
            Canvas.SetTop(grid, 330);
            Canvas.SetZIndex(grid, 1000);
            return grid;
        }


        public Grid CreatePlaylistTile()
        {
            PlaylistTileGrid = new Grid
            {
                Width = 1435,
                Height = 777,
                Tag = this
            };

            Rectangle background = new Rectangle
            {
                Width = 1435,
                Height = 777,
                RadiusX = 19,
                RadiusY = 19,
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D7D1CD")),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Effect = new DropShadowEffect
                {
                    BlurRadius = 28,
                    RenderingBias = RenderingBias.Performance,
                    Direction = 270,
                    Color = (Color)ColorConverter.ConvertFromString("#3F000000"),
                    ShadowDepth = 0,
                    Opacity = 1
                }
            };

            TitleBlock = new TextBlock
            {
                Text = Name,
                FontSize = 32,
                Margin = new Thickness(40, 40, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Foreground = Brushes.Black
            };

            TrackPanel = new StackPanel
            {
                Margin = new Thickness(40, 120, 40, 40)
            };

            TrackScroll = new ScrollViewer
            {
                Content = TrackPanel,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };

            TrackListBorder = new Border
            {
                Margin = new Thickness(40, 120, 40, 40),
                Background = Brushes.Transparent,
                Child = TrackScroll
            };

            PlaylistTileGrid.Children.Add(background);
            PlaylistTileGrid.Children.Add(TitleBlock);
            PlaylistTileGrid.Children.Add(TrackListBorder);

            PlaylistTileGrid.MouseLeftButtonDown += (s, e) =>
            {
                OnPlaylistSelected?.Invoke(this);
            };

            Canvas.SetLeft(PlaylistTileGrid, 314);
            Canvas.SetTop(PlaylistTileGrid, 224);

            return PlaylistTileGrid;
        }

        public void AddTrackToUI(Track track)
        {
            if (TrackPanel == null) return;

            TextBlock tb = new TextBlock
            {
                Text = $"{track.Title} — {track.Artist}",
                FontSize = 18,
                Margin = new Thickness(10)
            };

            TrackPanel.Children.Add(tb);
        }

        public void Rename(string newName)
        {
            Name = newName;
            if (TitleBlock != null)
                TitleBlock.Text = newName;

            OnPlaylistRenamed?.Invoke(this);
        }
    }

}

