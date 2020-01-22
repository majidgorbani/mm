using System.IO;

namespace SE.GOV.MM.Integration.Outlook
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
