using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using PsychoApp.MusicPlayer;
using System.Threading.Tasks;

namespace PsychoApp
{
    internal class PlayServices
    {
        private static readonly HttpClient client = new HttpClient();

        public async Task<SearchResult> SearchAsync(string query)
        {
            var encoded = Uri.EscapeDataString(query);
            string url = $"https://api.deezer.com/search?q={encoded}";

            try
            {
                var json = await client.GetStringAsync(url);
                var root = JsonConvert.DeserializeObject<DeezerSearchRoot>(json);

                var result = new SearchResult();

                foreach (var d in root.data)
                {
                    result.Tracks.Add(new Track
                    {
                        Title = d.title,
                        Artist = d.artist.name,
                        Album = d.album.title,
                        PreviewUrl = d.preview,  
                        Cover = d.album.cover_xl,
                        Duration = d.duration
                    });
                }

                return result;
            }
            catch
            {
                return new SearchResult();
            }
        }
    }
}

    public class DeezerSearchRoot
    {
        public DeezerTrack[] data { get; set; }
    }

    public class DeezerTrack
    {
        public string title { get; set; }
        public int duration { get; set; }
        public string preview { get; set; }


        public DeezerArtist artist { get; set; }
        public DeezerAlbum album { get; set; }
    }

    public class DeezerArtist
    {
        public string name { get; set; }
    }

    public class DeezerAlbum
    {
        public string title { get; set; }
        public string cover { get; set; }
        public string cover_small { get; set; }
        public string cover_medium { get; set; }
        public string cover_big { get; set; }
        public string cover_xl { get; set; }
    }


