using System;
using System.Collections.Generic;
using System.IO;
using FGHelper.FileHelpers;
using FGHelper.Projects;
using NDesk.Options;

namespace MagicTableMaker
{
    class Program
    {
        static readonly string CurrentVersion = "3.3";
        static readonly string CurrentRelease = "7|CoreRPG:3";

        static void Main(string[] args)
        {
            var showHelp = false;

            string outputPath = null;
            string source = null;
            string projectName = null;
            string filename = null;
            var author = "";
            var category = "";
            var version = CurrentVersion;
            var release = CurrentRelease;

            var options = new OptionSet()
            {
                {"o|output=","Output path", o => outputPath = o },
                {"s|source=","source path", s => source = s },
                {"p|project=", "Project name", p => projectName = p },
                {"f|filename=", "Output file name", f => filename = f },
                {"a|author=", "Module author", a => author = a },
                {"c|category=", "Module category", c => category = c },
                {"v|version=", $"Fantasy Grounds version - defaults to {CurrentVersion}", v => version = v },
                {"r|release=", $"Fantasy Grounds release - defaults to {CurrentRelease}", r => release = r },
                {"h|help|?",  "Show help message and exit", v => showHelp = v != null  }
            };

            List<string> extra;
            try
            {
                extra = options.Parse(args);
            }
            catch (OptionException e)
            {
                Console.WriteLine($"Exception parsing arguments. ex: {e.Message}");
                Environment.Exit(0);
            }

            if (args.Length == 0 || showHelp)
            {
                ShowHelpScreen();
                Environment.Exit(0);
            }
            
            VerifyArgs(outputPath, source, projectName, filename);

            if (!Directory.Exists(source))
            {
                Console.WriteLine($"{source} directory does not exist");
                Environment.Exit(0);
            }
            
            var path = Path.Combine(outputPath, filename);
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Cannot create output folder {outputPath}. ex: {e.Message}");
                    Environment.Exit(0);
                }            
            }

            var project = new Project
            {
                FileName = filename,
                Name = projectName,
                Author = author,
                Category = category,
                Version = version,
                Release = release
            };

            project = ProjectRunner.RunFileBasedProject(project, source);

            var result = FileWriter.WriteModFile(project, outputPath);

            Console.WriteLine($"Process complete.  File output: {result}");
        }

        private static void ShowHelpScreen()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Welcome to ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Battle Sloth Software's");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(" Magic Table Maker");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("for Fantasy Grounds (not affiliated with Smite Works).");
            Console.WriteLine("Use at your own risk, no warranty implied, not responsible");
            Console.WriteLine("if you are eaten by a Grue, etc., etc.");
            Console.WriteLine();
            Console.WriteLine("Command Line Arguments:");
            WriteArgLine(@"Source file path (where your stuff goes in):    -s ""C:\directory name""", true);
            WriteArgLine(@"Output file path (where your stuff comes out):    -o ""C:\directory name""", true);  
            WriteArgLine(@"Project name (name of your module):    -p ""project name""", true);
            WriteArgLine(@"Filename (what your module file will be named):    -f ""file name""", true);
            WriteArgLine(@"Author (You or your company's name):    -a ""Your name here""", false);
            WriteArgLine(@"Category (Module category):    -c ""Category name""", false);
            WriteArgLine($@"Version (Version of Fantasy Grounds supported. Defaults to {CurrentVersion}):    -v ""{CurrentVersion}""", false);
            WriteArgLine($@"Release (Release of Fantasy Grounds supported. Defaults to {CurrentRelease}):    -r ""{CurrentRelease}""", false);
            Console.WriteLine();
            Console.WriteLine("Press any key for next page...");
            var key = Console.ReadKey();
            Console.WriteLine();
            Console.WriteLine("This app reads any .txt documents in from the source directory and");
            Console.WriteLine("packages them into a .mod file. The app creates a table entry for");
            Console.WriteLine("each line in the document. By default it names the table after the file");
            Console.WriteLine("name and creates a single column table. You can add a file header JSON");
            Console.WriteLine("string to the first line to control how the app processes the txt file.");
            Console.WriteLine("");
            Console.WriteLine(@"Example: {""Name"":""NPC Names"",""Description"":""A bunch of names"",""Category"":""My Tables"",""Columns"":[""Male"",""Female""],""Separator"":""|""}");
            Console.WriteLine("");   
            Console.WriteLine("This will create a table with the name NPC Names with a description of A");
            Console.WriteLine("bunch of names. It will be a two column table with the columns being Male");
            Console.WriteLine("and Female. In the Tables dialog in Fantasy Grounds, this table will be");
            Console.WriteLine("in the My Tables list. It expects the dataset in the txt file to have the");
            Console.WriteLine("columns split by the '|' character");
            Console.WriteLine("None of these fields are required and any not present will use defaults.");
            Console.WriteLine();
            Console.WriteLine("The defaults are:");
            Console.WriteLine("    Name - txt filename");
            Console.WriteLine("    Description - blank");
            Console.WriteLine("    Category - project name");
            Console.WriteLine("    Columns - blank");
            Console.WriteLine("    Separator - If no separator is specified, it assumes single column table");
            Console.WriteLine();
            Console.WriteLine("If you want to specify a ruleset, add a ruleset.txt file to your source");
            Console.WriteLine("directory and put each ruleset you want to specify on a separate line of");
            Console.WriteLine("the text file. Otherwise it defaults to ruleset = Any");
        }

        private static void WriteArgLine(string value, bool required)
        {
            Console.ForegroundColor = required ? ConsoleColor.Red : ConsoleColor.Green;
            Console.Write($"   { (required ? "Required: " : "Optional: ")}");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(value);
        }

        private static void VerifyArgs(string outputPath, string source, string projectName, string filename)
        {
            var fail = false;
            var sb = new List<string>();

            if (outputPath == null)
            {
                sb.Add(@"   Output file path argument required. -o ""C:\directory name""");
                fail = true;
            }

            if (source == null)
            {
                sb.Add(@"   Source file path argument required. -s ""C:\directory name""");
                fail = true;
            }

            if (projectName == null)
            {
                sb.Add(@"   Project Name argument required. -p ""project name""");
                fail = true;
            }

            if (filename == null)
            {
                sb.Add(@"   Output filename argument required. -f ""file name""");
                fail = true;
            }

            if (fail)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error processing table files:");
                Console.ForegroundColor = ConsoleColor.White;
                foreach (var s in sb)
                {
                    Console.WriteLine(s);
                }
                Console.WriteLine("use -h for help");

                Environment.Exit(0);
            }
        }
    }

}
