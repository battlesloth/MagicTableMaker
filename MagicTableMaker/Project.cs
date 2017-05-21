using System.Collections.Generic;
using MagicTableMaker.FantasyGroundsObjects;

namespace MagicTableMaker
{
    public class Project
    {
        public string Name { get; set; }
        public string FileName { get; set; }
        public string Category { get; set; }
        public string Author { get; set; }

        public string SourceFilePath { get; set; }
        public string OutputFilePath { get; set; }

        public string Version { get; set; }
        public string Release { get; set; }

        public List<string> Rulesets { get; set; }
        public List<string> Entries { get; set; }
        public List<IFantasyGroundsObject> StoryTemplates { get; set; }

        public List<IFantasyGroundsObject> Tables { get; set; }

        public Project()
        {
            Rulesets = new List<string>();
            Entries = new List<string>();
            StoryTemplates = new List<IFantasyGroundsObject>();
            Tables = new List<IFantasyGroundsObject>();
        }

    }
}
