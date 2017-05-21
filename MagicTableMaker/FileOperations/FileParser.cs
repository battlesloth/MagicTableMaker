using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MagicTableMaker.FantasyGroundsObjects;
using Newtonsoft.Json;

namespace MagicTableMaker.FileOperations
{
    public static class FileParser
    {
        public static Table ParseTableFile(FileInfo file, string projectName)
        {
            var table = new Table();
            var rows = new List<List<string>>();

            using (var reader = file.OpenText())
            {
                var first = reader.ReadLine();

                FileHeader header;
                var multiColumn = false;

                if (first.TryDeserialize(out header))
                {
                    table.Name = header.Name ?? file.Name.Replace(file.Extension, "");
                    table.Description = header.Description ?? "";
                    table.Columns = header.Columns ?? new List<string>{" "};
                    table.Category = header.Category ?? projectName;
                    if (table.Columns.Count > 1)
                    {
                        multiColumn = true;
                    }
                }
                else
                {
                    table.Name = file.Name.Replace(file.Extension, "");
                    table.Description = "";
                    table.Columns.Add(" ");
                    table.Category = projectName;
                    rows.Add(new List<string> {first});
                }
                

                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    rows.Add(multiColumn ? line.Split(header.Separator).ToList() : new List<string> {line});
                }
            }

            table.Rows = rows;

            return table;
        }

        public static List<string> ParseRulesetFile(FileInfo file)
        {
            var result = new List<string>();
            using (var reader = file.OpenText())
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    result.Add(line);
                }
            }

            return result;
        }
    }

    public static class JsonExtensions
    {
        public static bool TryDeserialize<T>(this string obj, out T result )
        {
            try
            {
                result = JsonConvert.DeserializeObject<T>(obj);
                return true;
            }
            catch (Exception e)
            {
                result = default(T);
                return false;
            }
        }
    }
}