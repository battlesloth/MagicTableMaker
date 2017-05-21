namespace MagicTableMaker.FantasyGroundsObjects
{
    public class StoryTemplate : IFantasyGroundsObject
    {
        public string Class => "storytemplate";
        public string Name { get; set; }
        public string Category { get; set; }
        public string Body { get; set; }

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
