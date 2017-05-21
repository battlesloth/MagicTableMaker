using System.Collections.Generic;

namespace MagicTableMaker.FantasyGroundsObjects
{
    public class Table : IFantasyGroundsObject
    {
        public string Class => "tables";
        public string Name { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
        public List<string> Columns { get; set; }
        public List<List<string>> Rows { get; set; }

        public Table()
        {
            Columns = new List<string>();
            Rows = new List<List<string>>();
        }

        public string GetName()
        {
            return Name;
        }

        public string GetClass()
        {
            return Class;
        }

        public string GetCategory()
        {
            return Category;
        }
    }
}