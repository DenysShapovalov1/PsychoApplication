using Microsoft.VisualBasic.ApplicationServices;
using NAudio.Wave;
using Newtonsoft.Json;
using PsychoApp.ChatAI;
using PsychoApp.MusicPlayer;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using static NAudio.Wave.WaveInterop;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace PsychoApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public DateTime dateTime;

        DispatcherTimer timer;

        private Playlist CurrentPlaylist;

        DispatcherTimer clockTimer;

        private bool _isPlaying = false;

        private string _elapsed = "00:00";
        private string _remaining = "00:00";

        private readonly PlayServices _service = new PlayServices();

        private ObservableCollection<PsychoApp.MusicPlayer.Track> _tracks = new ObservableCollection<PsychoApp.MusicPlayer.Track>();

        private PsychoApp.MusicPlayer.Track _selectedTrack;

        private bool _ignoreSelectionChanged = false;

        private Dictionary<Thoughts, Grid> thoughtsAndBlocks = [];
        private Dictionary<DateTime, List<Thoughts>> thoughtsByDate = [];
        private Dictionary<DateTime, List<OptimizeDay>> optimizeDays = [];

        private Button[] buttons;

        private OpenRouterChat ai = new OpenRouterChat();

        public List<Chat> Chats = new List<Chat>();
        public Chat CurrentChat;

        private string thoughtsSavePath = "thoughts.json";

        private string chatsSavePath = "chats.json";

        public List<Playlist> Playlists = new List<Playlist>();
        private string playlistsSavePath = "playlists.json";

        private List<object> chatHistory = new List<object>()


        {
          new { role = "system", content =
                "You are a highly knowledgeable and compassionate AI assistant, specialized ONLY in psychology. " +
                "You can discuss topics such as human behavior, emotions, mental health, coping strategies, and other psychological subjects. " +
                "Your responses should always be logical, clear, concise, and helpful." +
                "Please avoid any conversation that goes beyond psychology. " +
                "Refuse to engage in discussions on any topics outside of psychology, including but not limited to: " +
                "18+ content, illegal activities, explicit material, politics, religion, or anything inappropriate." +
                "If a user asks about anything outside of psychology, politely tell them that you can only discuss psychology topics." +
                "Only answer in English." +
                "Do not repeat yourself." +
                "If asked about 18+ or explicit content, immediately refuse to engage and redirect the conversation to a safe, non-explicit subject." +
                "You must focus exclusively on the user's personal psychological experiences, emotions, thoughts, behaviors, and mental patterns." +
                "If the user asks about general topics, you must redirect the conversation back to the user’s own psychological processes." +
                "You may provide psychological insights only when they directly relate to the user and their inner world." +
                "Do not provide information, education, or explanations outside the user’s personal psychological context." +
                "If a question is not about the user’s psychology, politely refuse and ask the user how the topic relates to their emotional or psychological experience." +
                "You are allowed to analyze the user's saved Thoughts when the user requests it." + 
                "You will receive their thoughts inside a system message starting with “User thoughts data" + 
                "Use this data to provide psychological analysis, emotional insights, and gentle recommendations." +
                "Never repeat the raw text. Never diagnose"


            }
        };

        public MainWindow()
        {
            InitializeComponent();

            clockTimer = new DispatcherTimer();
            clockTimer.Interval = TimeSpan.FromSeconds(1); 
            clockTimer.Tick += ClockTimer_Tick;
            clockTimer.Start();


            LoadChats();

            LoadThoughts();

            SetupTimer();

            buttons = new Button[] { WaterIconBtn, ForestIconBtn, RainIconBtn, FireplaceIconBtn, BirdIconBtn, WindIconBtn, NightIconBtn, RailsIconBtn    };

            foreach (var btn in buttons)
            {
                btn.Click += AnyBtnClicked;
            }

            TracksList.ItemsSource = _tracks;

            CustomCalendar.OnDateSelected += (selectedDate) =>
                {
                    if (thoughtsByDate.ContainsKey(selectedDate))
                    {
                        var thoughtsOnDate = thoughtsByDate[selectedDate];
                        foreach (var thought in thoughtsOnDate)
                            ThoughtSelected(thought);
                    }
                    else
                    {
                        MessageBox.Show("Thoughts on this date don't exists.");
                    }
                };

            optimizeCalendar.OnDateSelected += date =>
            {
                OptimizeDay optimizeDay = new OptimizeDay(date);
                optimizeCalendarGrid.Children.Clear();
                optimizeCalendarGrid.Children.Add(optimizeDay.CreatedOptimizedDayBlock());

            };

            dateTime = DateTime.Now;

            PanelsCloseAndOpen(MainCanvas, optimaseDayCanvas, Home_Btn);

            PanelsCloseAndOpen(thoughtsPanel, MainCanvas, MyThoughts_Btn);

            PanelsCloseAndOpen(SearchMusicCanvas, MainCanvas, musicBtn);

            PanelsCloseAndOpen(AICanvas, MainCanvas, chatWithAI_Btn);

            PanelsCloseAndOpen(thoughtsPanel, MainCanvas, writeThoughts_Btn);

            PanelsCloseAndOpen(optimaseDayCanvas, MainCanvas, optimizeDayBtn);

            PanelsCloseAndOpen(MainCanvas, SearchMusicCanvas, HomeBtn_MusicPlayer);

            PanelsCloseAndOpen(MainCanvas, AICanvas, HomeBtn_AiPanel);

            PanelsCloseAndOpen(MainCanvas, thoughtsPanel, HomeBtn_ThoughtPanel);

            Deselect(MainTextBox, MainInputTextBlock);

            Deselect(ChatTextBox, ChatInputTextBlock);

            Deselect(SearchBox, SearchTextBlock);

            ToggleONFullScreen(FullScreen_Btn);

            ToggleOffFullScreen(FullOffScreenWhite_Btn);
            ToggleOffFullScreen(FullOffScreenBlack_Btn);


            Loaded += MainWindow_Loaded;

            TurnOffMusic(HomeBtn_MusicPlayer);

        }

        protected override void OnClosing(CancelEventArgs e)
        {
            SaveThoughts();
            SaveChats();
            base.OnClosing(e);
        }

        public void SaveThoughts()
        {
            List<SavedThought> saved = new List<SavedThought>();

            foreach (var pair in thoughtsAndBlocks)
            {
                var thought = pair.Key;
                var grid = pair.Value;

                saved.Add(new SavedThought
                {
                    Id = thought.Id,
                    Title = thought.title,
                    CreationDate = thought.CreateDate,
                    RichTextXaml = ThoughtBlock.GetRichTextXaml(grid)
                });
            }

            File.WriteAllText(thoughtsSavePath,
                Newtonsoft.Json.JsonConvert.SerializeObject(saved, Formatting.Indented));
        }

        public void SaveChats()
        {
            List<SavedChat> data = new();

            foreach (var chat in Chats)
            {
                var saved = new SavedChat
                {
                    Id = chat.Id,
                    Name = chat.Name,
                    Messages = new List<SavedMessage>()
                };

                foreach (dynamic msg in chat.History)
                {
                    saved.Messages.Add(new SavedMessage
                    {
                        Role = msg.role,
                        Content = msg.content
                    });
                }

                data.Add(saved);
            }

            File.WriteAllText(chatsSavePath,
                Newtonsoft.Json.JsonConvert.SerializeObject(data, Formatting.Indented));
        }


        public void LoadThoughts()
        {
            if (!File.Exists(thoughtsSavePath)) return;

            var json = File.ReadAllText(thoughtsSavePath);
            var saved = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SavedThought>>(json);
            if (saved == null) return;

            foreach (var s in saved)
            {
                Thoughts t = new Thoughts(s.Id);
                t.title = s.Title;

                t.ThoughtBlock = t.CreateDefaultThoughtBlock();

                ThoughtBlock.SetRichTextXaml(t.ThoughtBlock, s.RichTextXaml);

                t.OnThoughtSelected += ThoughtSelected;
                t.OnThoughtDeleted += ThoughtDeleted;

                thoughtsAndBlocks.Add(t, t.ThoughtBlock);

                ThoughtStackPanel.Children.Add(t.CreateThoughtRectangle());
                thoughtBlock.Children.Add(t.ThoughtBlock);
                t.ThoughtBlock.Visibility = Visibility.Collapsed;

                if (!thoughtsByDate.ContainsKey(s.CreationDate.Date))
                    thoughtsByDate[s.CreationDate.Date] = new List<Thoughts>();
                thoughtsByDate[s.CreationDate.Date].Add(t);
                IsEmptyText.Visibility = Visibility.Collapsed;
            }
        }

        public void LoadChats()
        {
            if (!File.Exists(chatsSavePath))
                return;

            var json = File.ReadAllText(chatsSavePath);
            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SavedChat>>(json);

            if (data == null) return;

            foreach (var c in data)
            {
                Chat chat = new Chat();
                chat.Id = c.Id;
                chat.Name = c.Name;

                chat.History = new List<object>();
                foreach (var msg in c.Messages)
                {
                    chat.History.Add(new { role = msg.Role, content = msg.Content });
                }

                chat.OnChatSelected += ChatSelected;
                chat.OnChatDeleted += ChatDeleted;

                Chats.Add(chat);

                var tile = chat.CreateChatRectangle(chat);
                ChatStackPanel.Children.Add(tile);
            }

            if (Chats.Count > 0)
                ChatSelected(Chats[0]);
        }


        private void AnyBtnClicked(object sender, RoutedEventArgs e)
        {
            Button clickedBtn = sender as Button;

            TryBtn.Visibility = Visibility.Visible;
            ArrowWithBtn.Visibility = Visibility.Visible;
        }

        private void NewChat_Click(object sender, RoutedEventArgs e)
        {
            var chat = new Chat
            {
                Name = "Chat " + (Chats.Count + 1),
            };

            chat.OnChatSelected += ChatSelected;
            chat.OnChatDeleted += ChatDeleted;

            Chats.Add(chat);

            var tile = chat.CreateChatRectangle(chat);
            ChatStackPanel.Children.Add(tile);

            ChatSelected(chat);
        }

        private void ChatSelected(Chat chat)
        {
            CurrentChat = chat;
            LoadChatHistory();
        }

        private void LoadChatHistory()
        {
            MessagesPanel.Children.Clear();
            if (CurrentChat == null) return;

            foreach (dynamic msg in CurrentChat.History)
            {
                AddMessage(
                    msg.role == "user" ? "You" : "PsychoApp AI",
                    msg.content
                );
            }
        }

        private void ChatDeleted(Chat chat)
        {
            Chats.Remove(chat);

            UIElement toRemove = null;
            foreach (UIElement element in ChatStackPanel.Children)
            {
                if (element is Grid g && g.Tag == chat)
                {
                    toRemove = element;
                    break;
                }
            }
            if (toRemove != null)
                ChatStackPanel.Children.Remove(toRemove);

            if (CurrentChat == chat)
            {
                CurrentChat = null;
                MessagesPanel.Children.Clear();
            }

            SaveChats();
        }

        private void SetupTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!Player.NaturalDuration.HasTimeSpan) return;

            TimeSpan current = Player.Position;
            TimeSpan total = Player.NaturalDuration.TimeSpan;

            if (current >= total - TimeSpan.FromMilliseconds(200))
            {
                _elapsed = total.ToString(@"mm\:ss");
                TestBlock.Text = $"{_selectedTrack.Title}\n{_selectedTrack.Artist}\n{_elapsed} / {_remaining}";
                timer.Stop();
                return;
            }

            _elapsed = current.ToString(@"mm\:ss");
            TestBlock.Text = $"{_selectedTrack.Title}\n{_selectedTrack.Artist}\n{_elapsed} / {_remaining}";
        }

        //private void CreatePlaylist_Click(object sender, RoutedEventArgs e)
        //{
        //    int id = Playlists.Count > 0 ? Playlists.Max(p => p.Id) + 1 : 1;

        //    Playlist playlist = new Playlist
        //    {
        //        Id = id,
        //        Name = "Playlist " + id
        //    };

        //    Playlists.Add(playlist);

        //    var tile = playlist.CreateBlockForPlaylist();
        //    //tile.MouseLeftButtonDown += PlaylistTile_Click;

        //    PlaylistStackPanel.Children.Add(tile);

            
        //}


        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            string query = SearchBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(query)) return;
            TracksList.Visibility = Visibility.Visible;

            var result = await _service.SearchAsync(query);

            if (result?.Tracks == null || !result.Tracks.Any())
            {
                MessageBox.Show("Song doesn't find.");
                return;
            }
            var currentTrack = _selectedTrack;

            _ignoreSelectionChanged = true;

            _tracks.Clear();

            if (currentTrack != null)
                _tracks.Add(currentTrack);

            foreach (var track in result.Tracks)
            {
                if (_tracks.All(t => t.PreviewUrl != track.PreviewUrl))
                    _tracks.Add(track);
            }

            TracksList.SelectedItem = currentTrack;
            _ignoreSelectionChanged = false;
        }

        private async void TracksList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (_ignoreSelectionChanged) return;

            _selectedTrack = TracksList.SelectedItem as MusicPlayer.Track;

            if (_selectedTrack == null)
            {
                return;
            }

            Player.Stop();
            timer.Stop();
            Player.Source = new Uri(_selectedTrack.PreviewUrl);
            FullScreen_Btn.IsEnabled = true;

            Player.MediaOpened -= Player_MediaOpened;
            Player.MediaOpened += Player_MediaOpened;

            Player.Play();

            _elapsed = "00:00";

            _isPlaying = true;
            PlayIcon.Text = "\uE769";


            if (!string.IsNullOrEmpty(_selectedTrack.Cover))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(_selectedTrack.Cover);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                TestImg.Source = bitmap;

                bitmap.DownloadCompleted += (s, ev) => ApplyGradientFromImage();
            }
            else
            {
                TestImg.Source = null;
                TestBlock.Text = "";
                GradientRect.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD7D1CD"));
            }

            var duration = await GetTrackDurationAsync(_selectedTrack.PreviewUrl);

            if (duration != TimeSpan.Zero)
            {
                _remaining = duration.ToString(@"mm\:ss");
            }
            else
            {
                _remaining = "00:00";
            }

            TestBlock.Text = $"{_selectedTrack.Title}\n{_selectedTrack.Artist}\n{_elapsed} / {_remaining}";
        }

        private async Task<TimeSpan> GetTrackDurationAsync(string url)
        {
            try
            {
                return await Task.Run(async () =>
                {
                    using var client = new HttpClient();
                    using var stream = await client.GetStreamAsync(url);
                    using var memStream = new MemoryStream();
                    await stream.CopyToAsync(memStream);
                    memStream.Position = 0;

                    using var reader = new Mp3FileReader(memStream);
                    return reader.TotalTime;
                });
            }
            catch
            {
                return TimeSpan.Zero;
            }
        }


        private void Player_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (Player.NaturalDuration.HasTimeSpan)
            {
                timer.Start();
            }
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedTrack == null || string.IsNullOrEmpty(_selectedTrack.PreviewUrl))
            {
                return;
            }
            if (!_isPlaying)
            {
                if (Player.Source == null || Player.Source.ToString() != _selectedTrack.PreviewUrl)
                {
                    Player.Source = new Uri(_selectedTrack.PreviewUrl);
                }

                Player.Play();
                _isPlaying = true;

                PlayIcon.Text = "\uE769";  
            }
            else
            {
                Player.Pause();
                _isPlaying = false;

                PlayIcon.Text = "\uE768"; 
            }
        }

        private  void ToggleONFullScreen(Button button)
        {
            var textInfo = CultureInfo.CurrentCulture.TextInfo;

            button.Click -= ToggleHandler;
            button.Click += ToggleHandler;

            void ToggleHandler(object sender, RoutedEventArgs e)
            {
                GradientFullScreenRect.Fill = GradientRect.Fill;
                FullScreenImg.Source = TestImg.Source;
                FullScreenGrid.Visibility = Visibility.Visible;
                TitleMusicFullScreen.Text = $"{textInfo.ToTitleCase(_selectedTrack.Title.ToLower())} - {textInfo.ToTitleCase(_selectedTrack.Artist.ToLower())}";
            }
        }

        private  void ToggleOffFullScreen(Button button)
        {
            button.Click -= ToggleHandler;
            button.Click += ToggleHandler;
            void ToggleHandler(object sender, RoutedEventArgs e)
            {
                FullScreenGrid.Visibility = Visibility.Hidden;
            }
        }

        private  void TurnOffMusic(Button button) => button.Click += (s, e) => { Player.Stop(); };

        private void ClockTimer_Tick(object sender, EventArgs e)
        {
            dateTime = DateTime.Now;

            CountTheTime(TimeBlock);
            CountTheTimeOfDay(TimeOfDayBlock);

            CountTheTime(TimeBlock1);
            CountTheTimeOfDay(TimeOfDayBlock1);

            CountTheTime(TimeBlock2);
            CountTheTimeOfDay(TimeOfDayBlock2);

            CountTheTime(TimeBlock3);
            CountTheTimeOfDay(TimeOfDayBlock3);
        }

        private void CountTheTime(TextBlock timeBlock) => timeBlock.Text = $"November {dateTime.Day}, {dateTime.Hour}:{dateTime:mm}";
        private void CountTheTimeOfDay(TextBlock timeOfDayBlock)
        {

            if (dateTime.Hour >= 6 && dateTime.Hour < 12)
                timeOfDayBlock.Text = "Good morning,";
            else if (dateTime.Hour >= 12 && dateTime.Hour < 18)
                timeOfDayBlock.Text = "Good day,";
            else if (dateTime.Hour >= 18 && dateTime.Hour < 22)
                timeOfDayBlock.Text = "Good evening,";
            else
                timeOfDayBlock.Text = "Good night,";
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainCanvas.Focus();
        }

        private void Deselect(TextBox textBox, TextBlock textBlock)
        {
            textBox.GotFocus += (s, e) =>
            {
                textBlock.Visibility = Visibility.Hidden;
            };

            textBox.LostFocus += (s, e) =>
            {
                if (MainTextBox.Text == "")
                {
                    textBox.Visibility = Visibility.Visible;
                }

            };
        }

        public void AddThought()
        {
            var existingIds = thoughtsAndBlocks.Keys.Select(t => t.Id).ToHashSet();
            int newId = 1;
            while (existingIds.Contains(newId))
                newId++;

            Thoughts thought = new Thoughts(newId);

            thought.OnThoughtSelected += ThoughtSelected;
            thought.OnThoughtDeleted += ThoughtDeleted;

            thoughtsAndBlocks.Add(thought, thought.ThoughtBlock);

            ThoughtStackPanel.Children.Add(thought.CreateThoughtRectangle());
            thoughtBlock.Children.Add(thought.ThoughtBlock);

            foreach (var kv in thoughtsAndBlocks)
                kv.Value.Visibility = Visibility.Collapsed;

            thought.ThoughtBlock.Visibility = Visibility.Visible;

            if (!thoughtsByDate.ContainsKey(thought.CreateDate.Date))
                thoughtsByDate[thought.CreateDate.Date] = new List<Thoughts>();
            thoughtsByDate[thought.CreateDate.Date].Add(thought);

            if (ThoughtStackPanel.Children.Count > 0)
            {
                IsEmptyText.Visibility = Visibility.Collapsed;
            }
        }

        private void ThoughtDeleted(Thoughts toDelete)
        {
            if (thoughtsAndBlocks.ContainsKey(toDelete))
                thoughtsAndBlocks.Remove(toDelete);

            UIElement rectangle = null;
            foreach (UIElement child in ThoughtStackPanel.Children)
            {
                if (child is Grid g && g.DataContext == toDelete)
                {
                    rectangle = child;
                    break;
                }
            }

            if (rectangle != null)
                ThoughtStackPanel.Children.Remove(rectangle);

            if (thoughtBlock.Children.Contains(toDelete.ThoughtBlock))
                thoughtBlock.Children.Remove(toDelete.ThoughtBlock);

            if (ThoughtStackPanel.Children.Count == 0)
                IsEmptyText.Visibility = Visibility.Visible;

            SaveThoughts();
        }

        private void ThoughtSelected(Thoughts selected)
        {
            foreach (var block in thoughtsAndBlocks)
                block.Value.Visibility = Visibility.Collapsed;

            selected.ThoughtBlock.Visibility = Visibility.Visible;
        }

        private void PanelsCloseAndOpen(Canvas firstCanvas, Canvas secondCanvas, Button closeBtn)
        {
            closeBtn.Click -= CloseBtnHandler;
            closeBtn.Click += CloseBtnHandler;
            void CloseBtnHandler(object sender, RoutedEventArgs e)
            {
                if (secondCanvas.Visibility == Visibility.Visible)
                {
                    firstCanvas.Visibility = Visibility.Visible;
                    secondCanvas.Visibility = Visibility.Hidden;
                }
            }

        }

        private void CloseAndOpenCalendar(object sender, RoutedEventArgs e)
        {
            if (CustomCalendar.Visibility == Visibility.Hidden)
            {
                CustomCalendar.Visibility = Visibility.Visible;
            }
            else
            {
                CustomCalendar.Visibility = Visibility.Hidden;
            }
        }

        private void Create_Thought(object sender, RoutedEventArgs e)
        {
             AddThought();
        }

        private void Quit(object sender, RoutedEventArgs e) => Application.Current.Shutdown();


        Func<Color, double> GetLuminance = color => 0.299 * color.R + 0.587 * color.G + 0.114 * color.B;

        private void ApplyGradientFromImage()
        {

            if (TestImg.Source is BitmapSource bitmap)
            {
                Color color1 = GetPixelColor(bitmap, 0, 0);
                Color color2 = GetPixelColor(bitmap, bitmap.PixelWidth - 1, bitmap.PixelHeight - 1);

                var avgR = (color1.R + color2.R) / 2;
                var avgG = (color1.G + color2.G) / 2;
                var avgB = (color1.B + color2.B) / 2;

                var avgCol = Color.FromRgb((byte)avgR, (byte)avgG, (byte)avgB);

                Color textColor = GetLuminance(avgCol) < 128 ? Colors.White : Colors.Black;

                if (GetLuminance(avgCol) < 128)
                {
                    FullOffScreenWhite_Btn.Visibility = Visibility.Visible;
                    FullOffScreenBlack_Btn.Visibility = Visibility.Hidden;

                }
                else
                {
                    FullOffScreenBlack_Btn.Visibility = Visibility.Visible;
                    FullOffScreenWhite_Btn.Visibility = Visibility.Hidden;
                }

                HeaderSearchingMusic.Foreground = new SolidColorBrush(textColor);
                TracksList.Foreground = new SolidColorBrush(textColor);
                TracksList.BorderBrush = new SolidColorBrush(textColor);
                TitleMusicFullScreen.Foreground = new SolidColorBrush(textColor);

                if (!(GradientRect.Fill is LinearGradientBrush gradient))
                {
                    gradient = new LinearGradientBrush();
                    gradient.StartPoint = new Point(0, 0);
                    gradient.EndPoint = new Point(1, 1);
                    gradient.GradientStops.Add(new GradientStop(color1, 0));
                    gradient.GradientStops.Add(new GradientStop(color2, 1));
                    GradientRect.Fill = gradient;

                    RenderOptions.SetBitmapScalingMode(GradientRect, BitmapScalingMode.HighQuality);
                    RenderOptions.SetEdgeMode(GradientRect, EdgeMode.Unspecified);
                }
                else
                {
                    var anim1 = new ColorAnimation(color1, TimeSpan.FromSeconds(1));
                    var anim2 = new ColorAnimation(color2, TimeSpan.FromSeconds(1));

                    gradient.GradientStops[0].BeginAnimation(GradientStop.ColorProperty, anim1);
                    gradient.GradientStops[1].BeginAnimation(GradientStop.ColorProperty, anim2);
                }
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            SaveThoughts();
            SaveChats();
        }

        private Color GetPixelColor(BitmapSource bitmap, int x, int y)
        {
            var wb = new WriteableBitmap(bitmap);
            byte[] pixels = new byte[4];
            wb.CopyPixels(new Int32Rect(x, y, 1, 1), pixels, 4, 0);
            return Color.FromArgb(pixels[3], pixels[2], pixels[1], pixels[0]);
        }

        //------------------------------Animations------------------------------//
        private void MainWindow_Loaded(object sender, RoutedEventArgs e) => StartParticleAnimations();

        private void StartParticleAnimations()
        {
            AnimateParticle(Particle1Transform, ParticlesCanvas.Children[0] as UIElement, 0);
            AnimateParticle(Particle2Transform, ParticlesCanvas.Children[1] as UIElement, 6);
            AnimateParticle(Particle3Transform, ParticlesCanvas.Children[2] as UIElement, 12);
            AnimateParticle(Particle4Transform, ParticlesCanvas.Children[3] as UIElement, 3);
            AnimateParticle(Particle5Transform, ParticlesCanvas.Children[4] as UIElement, 18);
            AnimateParticle(Particle6Transform, ParticlesCanvas.Children[5] as UIElement, 9);

        }
        private void AnimateParticle(TranslateTransform trans, UIElement element, double delay)
        {
            var height = ActualHeight;
            var animY = new DoubleAnimation(height + 100, -100, TimeSpan.FromSeconds(25))
            { BeginTime = TimeSpan.FromSeconds(delay), RepeatBehavior = RepeatBehavior.Forever };
            var animX = new DoubleAnimation(-50, 50, TimeSpan.FromSeconds(25))
            { BeginTime = TimeSpan.FromSeconds(delay), RepeatBehavior = RepeatBehavior.Forever };

            var opacity = new DoubleAnimation(0, 0.8, TimeSpan.FromSeconds(6))
            { BeginTime = TimeSpan.FromSeconds(delay + 5), AutoReverse = true, RepeatBehavior = RepeatBehavior.Forever };

            trans.BeginAnimation(TranslateTransform.YProperty, animY);
            trans.BeginAnimation(TranslateTransform.XProperty, animX);
            element.BeginAnimation(UIElement.OpacityProperty, opacity);
        }

        //-----------------------AI-----------------------------------------------------------//
        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentChat == null)
            {
                NewChat_Click(sender, e);
            }

            string message = ChatTextBox.Text.Trim();
            if (string.IsNullOrEmpty(message)) return;
            AddMessage("You", message);

            CurrentChat.History.Add(new { role = "user", content = message });

            if (message.ToLower().Contains("analizuj myśli") ||
                message.ToLower().Contains("analizuj moje myśli") ||
                message.ToLower().Contains("analyze my thoughts") ||
                message.ToLower().Contains("analyze thoughts"))
            {
                StringBuilder thoughtsText = new StringBuilder();

                foreach (var pair in thoughtsAndBlocks)
                {
                    var block = pair.Value;
                    string xaml = ThoughtBlock.GetRichTextXaml(block);
                    string plainText = ThoughtBlock.GetPlainTextFromXaml(xaml);

                    if (!string.IsNullOrWhiteSpace(plainText))
                        thoughtsText.AppendLine("• " + plainText);
                }

                CurrentChat.History.Add(new
                {
                    role = "system",
                    content = "User thoughts data (do NOT repeat raw text):\n" + thoughtsText.ToString()
                });
            }

            ChatTextBox.Text = "";
            ChatTextBox.IsEnabled = false;
            ChatMsgButtonGo.IsEnabled = false;

            ThinkingBlock.Visibility = Visibility.Visible;

            string answer = await ai.SendMessageAsync(CurrentChat.History);

            ThinkingBlock.Visibility = Visibility.Collapsed;

            CurrentChat.History.Add(new { role = "assistant", content = answer });

            AddMessage("PsychoApp AI", answer);

            ChatTextBox.IsEnabled = true;
            ChatMsgButtonGo.IsEnabled = true;
            ChatTextBox.Focus();
        }


        private void SendFromMain_Click(object sender, RoutedEventArgs e)
        {
            string text = MainTextBox.Text.Trim();
            if (string.IsNullOrEmpty(text))
                return;

            Chat newChat = new Chat
            {
                Name = "Chat " + (Chats.Count + 1)
            };

            newChat.OnChatSelected += ChatSelected;
            newChat.OnChatDeleted += ChatDeleted;

            Chats.Add(newChat);

            var tile = newChat.CreateChatRectangle(newChat);
            ChatStackPanel.Children.Add(tile);

            ChatSelected(newChat);
            MainCanvas.Visibility = Visibility.Hidden;
            AICanvas.Visibility = Visibility.Visible;

            ChatTextBox.Text = text;
            MainTextBox.Text = "";

            SendButton_Click(ChatMsgButtonGo, null);
        }



        private void AddMessage(string sender, string message)
        {
            var messagePanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(10)
            };
            Ellipse avatar = new Ellipse
            {
                Width = 30,
                Height = 30,
                VerticalAlignment = VerticalAlignment.Top,
                
                Effect = new System.Windows.Media.Effects.DropShadowEffect()
                {
                    Color = Color.FromArgb(25, 0, 0, 0),
                    ShadowDepth = 0,
                    BlurRadius = 20,
                    Opacity = 1,
                }
            };
            TextBlock textBlock = new TextBlock
            {   
                Text = message,
                FontSize = 14,
                Foreground = Brushes.Black,
                TextWrapping = TextWrapping.Wrap,
                Width = 200,
                VerticalAlignment = VerticalAlignment.Center,
            };

            if (sender == "You")
            {
                avatar.Margin = new Thickness(5);
                messagePanel.HorizontalAlignment = HorizontalAlignment.Right;
                avatar.Fill = Brushes.LightGreen;
                textBlock.TextAlignment = TextAlignment.Right;
                messagePanel.Children.Add(textBlock);
                messagePanel.Children.Add(avatar);
            }
            else
            {
                avatar.Margin = new Thickness(5);
                messagePanel.HorizontalAlignment = HorizontalAlignment.Left;
                avatar.Fill = Brushes.LightGray;
                textBlock.TextAlignment = TextAlignment.Left;
                textBlock.Width = 400;
                messagePanel.Children.Add(avatar);
                messagePanel.Children.Add(textBlock);
            }

            MessagesPanel.Children.Add(messagePanel);

            if (MessagesPanel.Parent is ScrollViewer scroll)
            {
                scroll.ScrollToEnd();
            }
        }
    }
}
