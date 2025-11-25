using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychoApp
{
    public class SavedThought
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string RichTextXaml { get; set; }
        public DateTime CreationDate { get; set; }
    }

}
