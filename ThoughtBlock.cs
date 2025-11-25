using System;
using System.Collections.Generic;
using System.IO;
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

namespace PsychoApp
{
    class ThoughtBlock
    {
        protected int idOfBlock = 0;
        protected string writtedText = string.Empty;
        public string title = string.Empty;
        protected int wordsCount = 0;

        public TextBlock titleText;

        public ThoughtBlock()
        {
            
            this.wordsCount = 0;
        }

        public virtual Grid CreateDefaultThoughtBlock()
        {
            Style ThButtons = (Style)Application.Current.FindResource("ThButtons");

            Grid gridBlock = new Grid()
            {
                Name = $"ThoughtBlock{idOfBlock}",
                Width = 1248,
                Height = 906,
            };
            Rectangle bgRect = new()
            {
                Width = 1248,
                Height = 906,
                RadiusX = 24,
                RadiusY = 24,
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D7D1CD")),
                Effect = new System.Windows.Media.Effects.DropShadowEffect()
                {
                    Color = Color.FromArgb(25, 0, 0, 0),
                    ShadowDepth = 4,
                    BlurRadius = 20,
                    Direction = 270,
                    Opacity = 1,
                }
            };
            Rectangle titleRect = new()
            {
                Width = 1172,
                Height = 241,
                RadiusX = 24,
                RadiusY = 24,
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#A6A3A1")),
                Margin = new Thickness(0),
                VerticalAlignment = VerticalAlignment.Top
            };
            titleText = new()
            {
                Height = 66,
                TextWrapping = TextWrapping.Wrap,
                Text = this.title,
                FontSize = 40,
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Kaisei Tokumin"),
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 80, 0, 0),

            };
            RichTextBox richTextBox = new RichTextBox()
            {
                Width = 1128,
                Height = 577,
                BorderBrush = Brushes.Transparent,
                Background = Brushes.Transparent,
                Margin = new Thickness(left: 0, top: 230, right: 0, bottom: 0),

            };
            TextBlock textBlockCounter = new TextBlock()
            {
                Text = "Words: 0",
                FontSize = 18,
                Margin = new Thickness(60, 865, 0, 0)
            };
            Button btnBold = new Button()
            {
                Content = "B",
                FontWeight = FontWeights.Bold,
                Width = 30,
                Height = 30,
                Margin = new Thickness(1100, 860, 0, 0),
                Style = ThButtons
            };
            Button btnItalic = new Button()
            {
                Content = "I",
                FontStyle = FontStyles.Italic,
                Width = 30,
                Height = 30,
                Margin = new Thickness(1020, 860, 0, 0),
                Style = ThButtons
            };

            FlowDocument doc = new FlowDocument();
            doc.PagePadding = new Thickness(0); 

            doc.Blocks.Add(new Paragraph() { Margin = new Thickness(0) });

            richTextBox.Document = doc;

            btnBold.Click += (s, e) => MakeBold(richTextBox);
            btnItalic.Click += (s, e) => MakeItalic(richTextBox);
            richTextBox.TextChanged += (s, e) =>
            {
                string text = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd).Text;
                wordsCount = CountWords(text);
                textBlockCounter.Text = $"Words: {wordsCount}";
            };

            gridBlock.Children.Add(bgRect);
            gridBlock.Children.Add(titleRect);
            gridBlock.Children.Add(titleText);
            gridBlock.Children.Add(richTextBox);
            gridBlock.Children.Add(btnBold);
            gridBlock.Children.Add(textBlockCounter);
            gridBlock.Children.Add(btnItalic);
            Grid.SetZIndex(btnBold, 100);
            Grid.SetZIndex(btnItalic, 100);


            return gridBlock;
        }

        private int CountWords(string text)
        {
            return text.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
        }

        private void MakeItalic(RichTextBox rtb)
        {
            if (!rtb.Selection.IsEmpty)
                rtb.Selection.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Italic);
        }

        private void MakeBold(RichTextBox rtb)
        {
            if (!rtb.Selection.IsEmpty)
                rtb.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
        }

        public static string GetRichText(Grid thoughtGrid)
        {
            var rtb = thoughtGrid.Children.OfType<RichTextBox>().FirstOrDefault();
            if (rtb == null) return "";
            return new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd).Text;
        }

        public static void SetRichText(Grid thoughtGrid, string text)
        {
            var rtb = thoughtGrid.Children.OfType<RichTextBox>().FirstOrDefault();
            if (rtb != null)
            {
                rtb.Document.Blocks.Clear();
                rtb.Document.Blocks.Add(new Paragraph(new Run(text)));
            }
        }

        public static string GetRichTextXaml(Grid thoughtGrid)
        {
            var rtb = thoughtGrid.Children.OfType<RichTextBox>().FirstOrDefault();
            if (rtb == null) return null;

            TextRange range = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);

            using (var ms = new MemoryStream())
            {
                range.Save(ms, DataFormats.Xaml);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        public static void SetRichTextXaml(Grid thoughtGrid, string xaml)
        {
            var rtb = thoughtGrid.Children.OfType<RichTextBox>().FirstOrDefault();
            if (rtb == null || string.IsNullOrEmpty(xaml)) return;

            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(xaml)))
            {
                TextRange range = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
                range.Load(ms, DataFormats.Xaml);
            }
        }

        public static string GetPlainTextFromXaml(string xaml)
        {
            var stringReader = new System.IO.StringReader(xaml);
            var xmlReader = System.Xml.XmlReader.Create(stringReader);
            var doc = System.Windows.Markup.XamlReader.Load(xmlReader) as FlowDocument;

            TextRange tr = new TextRange(doc.ContentStart, doc.ContentEnd);
            return tr.Text.Trim();
        }

    }
}
