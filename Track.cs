using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychoApp.MusicPlayer
{
    public class Track
    {
        public int Id {  get; set; }
        public string Title { get; set; }

        public string Preview { get; set; }

        public string Link { get; set; }

        public string Artist { get; set; }

        public string Album { get; set; }

        public string PreviewUrl { get; set; }   
        public int Duration { get; set; }

        public string Cover {  get; set; }


    }
}
