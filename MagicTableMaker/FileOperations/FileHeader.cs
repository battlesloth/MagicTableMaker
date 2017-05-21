using System.Collections.Generic;

namespace MagicTableMaker.FileOperations
{
    class FileHeader
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public List<string> Columns { get; set; }
        public char Separator { get; set; }
    }
}