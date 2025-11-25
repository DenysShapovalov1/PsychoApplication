using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychoApp
{
    public class SavedChat
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<SavedMessage> Messages { get; set; }
    }

    public class SavedMessage
    {
        public string Role { get; set; }
        public string Content { get; set; }
    }

}
