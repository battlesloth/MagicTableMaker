using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml;
using MagicTableMaker.FantasyGroundsObjects;
using MagicTableMaker.FileOperations;

namespace MagicTableMaker
{
    internal class ProjectRunner
    {

        private Project project;

        public ProjectRunner(Project project)
        {
            this.project = project;
        }

        public void Run()
        {
            project.Entries.Add("table");

            project.Tables = new List<IFantasyGroundsObject>();

            var files = new DirectoryInfo(project.SourceFilePath).GetFiles();

            foreach (var fileInfo in files)
            {
                Console.WriteLine($"Processing {fileInfo.Name}.");

                if (fileInfo.Name.ToLower().StartsWith("ruleset"))
                {
                    project.Rulesets = FileParser.ParseRulesetFile(fileInfo);
                }
                else
                {
                    project.Tables.Add(FileParser.ParseTableFile(fileInfo, project.Name));
                }
            }

            if (project.Rulesets.Count == 0)
            {
                project.Rulesets.Add("Any");
            }

            var helper = new FantasyGroundsXmlHelper();

            var def = helper.GenerateDefinitionXml(project);
            var db = helper.GenerateDbXml(project.Version, project.Release, project);

            var xmlSettings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "\t",
                Encoding = Encoding.GetEncoding("iso-8859-1"),
                NewLineOnAttributes = false
            };

            Console.WriteLine("Writing definition file.");
            using (var writer = XmlWriter.Create($"{project.OutputFilePath}\\{project.FileName}\\definition.xml", xmlSettings))
            {
                def.Save(writer);
            }

            Console.WriteLine("Writing db file.");
            using (var writer = XmlWriter.Create($"{project.OutputFilePath}\\{project.FileName}\\db.xml", xmlSettings))
            {
                db.Save(writer);
            }

            Console.WriteLine("Zipping files.");

            if (File.Exists($"{project.OutputFilePath}\\{project.FileName}.zip"))
            {
                File.Delete($"{project.OutputFilePath}\\{project.FileName}.zip");
            }

           
            ZipFile.CreateFromDirectory($"{project.OutputFilePath}\\{project.FileName}", $"{project.OutputFilePath}\\{project.FileName}.zip");

            if (File.Exists($"{project.OutputFilePath}\\{project.FileName}.mod"))
            {
                File.Delete($"{project.OutputFilePath}\\{project.FileName}.mod");
            }

            File.Move($"{project.OutputFilePath}\\{project.FileName}.zip", $"{project.OutputFilePath}\\{project.FileName}.mod");

            Console.WriteLine("Cleaning up temp files.");

            Directory.Delete($"{project.OutputFilePath}\\{project.FileName}", true);
            File.Delete($"{project.OutputFilePath}\\{project.FileName}.zip");


            Console.WriteLine("Process complete.");
        }
    }
}
