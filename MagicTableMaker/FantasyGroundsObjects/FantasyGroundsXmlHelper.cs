using System.Collections.Generic;
using System.Xml.Linq;

namespace MagicTableMaker.FantasyGroundsObjects
{
    public class FantasyGroundsXmlHelper
    {
        private readonly XAttribute stringAttr = new XAttribute("type", "string");
        private readonly XAttribute numberAttr = new XAttribute("type", "number");
        private readonly XAttribute formattedAttr = new XAttribute("type", "formattedtest");


        private readonly Dictionary<string, string> entryTypes = new Dictionary<string, string>
        {
            {"storytemplate", "Story Templates"},
            {"table", "Tables" }
        };

        public XDocument GenerateDefinitionXml(Project project)
        {
            var xRoot = new XElement("root");

            var xName = new XElement("name", project.Name);
            var xCat = new XElement("category", project.Category);
            var xAuthor = new XElement("author", project.Author);

            xRoot.Add(xName, xCat, xAuthor);

            foreach (var ruleset in project.Rulesets)
            {
                xRoot.Add(new XElement("ruleset", ruleset));
            }
           
            return new XDocument(new XDeclaration("1.0", "ISO-8859-1", ""), xRoot);
        }

        public XDocument GenerateDbXml(string version, string release, Project project)
        {
            var xRoot = new XElement("root");
            xRoot.Add(new XAttribute("version",version), new XAttribute("release",release));
            xRoot.Add(GenerateLibrary(project.FileName, project.Name, project.Category, project.Entries));
            xRoot.Add(AddLists(project));
            if (project.StoryTemplates.Count > 0)
            {
                xRoot.Add(AddStoryTemplates(project.StoryTemplates));
            }
            if (project.Tables.Count > 0)
            {
                xRoot.Add(AddTables(project.Tables));
            }

            return new XDocument(new XDeclaration("1.0", "ISO-8859-1", ""), xRoot);
        }

        #region Library

        public XElement GenerateLibrary(string fileName, string projName, string category, List<string> entires )
        {
            var xCat = new XElement("categoryname", category);
            xCat.Add(stringAttr);
            var xName = new XElement("name", fileName);
            xName.Add(stringAttr);

            var xEntries = new XElement("entries");

            foreach (var entry in entires)
            {
                xEntries.Add(GenerateEntry(projName, entry, entryTypes[entry]));
            }

            var element = new XElement(fileName);
            element.Add(new XAttribute("static", "true"));
            element.Add(xCat);
            element.Add(xName);
            element.Add(xEntries);
            
            var library = new XElement("library", element);

            return library;
        }

        public XElement GenerateEntry(string projName, string type, string typeName)
        {
            var xClass = new XElement("class", "referenceindexsorted");
            var xRecName = new XElement("recordname", $"lists.{type}@{projName}");
            var xLibLink = new XElement("librarylink");
            xLibLink.Add(new XAttribute("type", "windowreference"));
            xLibLink.Add(xClass);
            xLibLink.Add(xRecName);
            var xName = new XElement("name", typeName);
            xName.Add(stringAttr);

            var result = new XElement(type);
            result.Add(xLibLink);
            result.Add(xName);

            return result;
        }

        #endregion

        #region Lists

        public XElement AddLists(Project project)
        {
            var result = new XElement("lists");

            if (project.StoryTemplates.Count > 0)
            {
                result.Add(GenerateList("storytemplate", "Story Templates", project.Name, project.StoryTemplates));
            }
            if (project.Tables.Count > 0)
            {
                result.Add(GenerateList("table", "Tables", project.Name, project.Tables));
            }

            return result;
        }

        public XElement GenerateList(string type, string name, string projName, List<IFantasyGroundsObject> fgObjs)
        {
            var result = new XElement(type);
            var xName = new XElement("name", name);
            xName.Add(stringAttr);
            var xIndex = new XElement("index");

            var count = 1;
            foreach (var fgObj in fgObjs)
            {
                xIndex.Add(GenerateListLink(count, projName, fgObj.GetClass(), fgObj.GetName()));
                count++;
            }
            result.Add(xName, xIndex);

            return result;
        }

        public XElement GenerateListLink(int id, string projName, string className, string linkName)
        {
            var xLinkList = new XElement("listlink");
            xLinkList.Add(new XAttribute("type", "windworeference"));

            var xClass = new XElement("class", className);
            var xRecName = new XElement("recordname", $"{(className == "table" ? "tables" : className)}.id-{id:D5}@{projName}");

            xLinkList.Add(xClass, xRecName);

            var xName = new XElement("name", linkName);
            xName.Add(stringAttr);

            var result = new XElement($"id-{id:D5}");

            result.Add(xLinkList, xName);

            return result;
        }

        #endregion

        #region Story Templates

        public XElement AddStoryTemplates(List<IFantasyGroundsObject> storyTemplates)
        {      
            var categorized = new Dictionary<string, List<XElement>>();

            var count = 1;
            foreach (var storyTemplate in storyTemplates)
            {
                var el = GenerateStoryTemplate(count, (StoryTemplate) storyTemplate);

                if (!categorized.ContainsKey(storyTemplate.GetCategory()))
                {
                    categorized[storyTemplate.GetCategory()] = new List<XElement>();
                }

                categorized[storyTemplate.GetCategory()].Add(el);
            }

            return CategorizeFgElements("storytemplate", categorized);
        }

        public XElement GenerateStoryTemplate(int id, StoryTemplate storyTemplate)
        {
            var xName = new XElement("name", storyTemplate.Name);
            xName.Add(stringAttr);

            var xText = new XElement("text", storyTemplate.Body);
            xText.Add(formattedAttr);

            var result = new XElement($"id-{id:D5}");
            result.Add(xName);
            result.Add(xText);

            return result;
        }

        #endregion

        #region Tables

        public XElement AddTables(List<IFantasyGroundsObject> tables)
        {
            var categorized = new Dictionary<string, List<XElement>>();

            var count = 1;
            foreach (var table in tables)
            {
                var el = GenerateTableEntry(count, (Table) table);

                if (!categorized.ContainsKey(table.GetCategory()))
                {
                    categorized[table.GetCategory()] = new List<XElement>();
                }

                categorized[table.GetCategory()].Add(el);

                count++;
            }

            return CategorizeFgElements("tables", categorized);
        }

        public  XElement GenerateTableEntry(int id, Table table)
        {
            var result = new XElement($"id-{id:D5}");

            var xDesc = new XElement("description", table.Description);
            xDesc.Add(stringAttr);

            var xDice = new XElement("dice");
            xDice.Add(new XAttribute("type","dice"));

            var xHideRes = new XElement("hiderollresults", 0);
            xHideRes.Add(numberAttr);

            result.Add(xDesc, xDice, xHideRes);

            var count = 1;

            foreach (var key in table.Columns)
            {
                var el = new XElement($"labelcol{count}", key);
                el.Add(stringAttr);
                result.Add(el);
                count++;
            }

            var xMod = new XElement("mod", 0);
            xMod.Add(numberAttr);

            var xName = new XElement("name", table.Name);
            xName.Add(stringAttr);

            var xNotes = new XElement("notes",table.Notes);
            xNotes.Add(formattedAttr);
            
            var xResCols = new XElement("resultscols", table.Columns.Count);
            xResCols.Add(numberAttr);

            var xTableOffset = new XElement("table_positionoffset", 0);
            xTableOffset.Add(numberAttr);

            result.Add(xMod, xName, xNotes, xResCols, xTableOffset);

            var xRows = new XElement("tablerows");

            count = 1;
            foreach (var row in table.Rows)
            {
                xRows.Add(GenerateTableRow(count, row));
                count++;
            }

            result.Add(xRows);

            return result;
        }

        public XElement GenerateTableRow(int id, List<string> rowValues)
        {
            var xFromRng = new XElement("fromrange", id);
            xFromRng.Add(numberAttr);

            var xResults = new XElement("results");

            var count = 1;

            foreach (var value in rowValues)
            {
                xResults.Add(GenerateRowColumn(count, value));
                count++;
            }

            var xToRng = new XElement("torange", 0);
            xToRng.Add(numberAttr);

            var result = new XElement($"id-{id:D5}");
            result.Add(xFromRng, xResults, xToRng);

            return result;
        }

        public XElement GenerateRowColumn(int id, string value)
        {
            var xResult = new XElement("result", value);
            xResult.Add(stringAttr);

            var xClass = new XElement("class");
            var xRecName = new XElement("recordname");

            var xResLink = new XElement("resultlink");
            xResLink.Add(new XAttribute("type","windowreference"), xClass, xRecName);

            var result = new XElement($"id-{id:D5}");
            result.Add(xResult, xResLink);

            return result;
        }

        #endregion

        #region Common Methods
        

        public XElement CategorizeFgElements(string type, Dictionary<string, List<XElement>> categorized)
        {
            var result = new XElement(type);

            foreach (var category in categorized.Keys)
            {
                var xCat = new XElement("category");
                xCat.Add(new XAttribute("name", category), new XAttribute("baseicon", "0"), new XAttribute("decalicon", "0"));

                xCat.Add(categorized[category]);

                result.Add(xCat);
            }

            return result;
        }
        #endregion
    }
}
