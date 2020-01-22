using System.IO;

namespace MinaMeddelanden.Sydnarke
{
    public class ListItem
    {
        public ListItem(string name, FileInfo value)
        {
            Name = name;
            Value = value;
        }
        public FileInfo Value { get; set; }
        public string Name { get; set; }
    }
}
