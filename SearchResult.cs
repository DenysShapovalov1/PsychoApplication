using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychoApp.MusicPlayer
{
    public class SearchResult
    {
        public List<Track> Tracks { get; set; }

        public SearchResult()
        {
            Tracks = new List<Track>();
        }
    }
}
