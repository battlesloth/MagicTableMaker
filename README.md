# MagicTableMaker
Command Line app for munging text files into tables in Fantasy Grounds 

Welcome to Battle Sloth Software's Magic Table Maker for Fantasy Grounds (not affiliated with Smite Works).
Use at your own risk, no warranty implied, not responsible if you are eaten by a Grue, etc., etc.

Command Line Arguments:
   Required: Source file path (where your stuff goes in):    -s "C:\directory name"
   Required: Output file path (where your stuff comes out):    -o "C:\directory name"
   Required: Project name (name of your module):    -p "project name"
   Required: Filename (what your module file will be named):    -f "file name"
   Optional: Author (You or your company's name):    -a "Your name here"
   Optional: Category (Module category):    -c "Category name"
   Optional: Version (Version of Fantasy Grounds supported. Defaults to 3.3):    -v "3.3"
   Optional: Release (Release of Fantasy Grounds supported. Defaults to 7|CoreRPG:3):    -r "7|CoreRPG:3"


Example Usage: >MagicTableMaker.exe -o "C:\test" -s "C:\test\tables" -p "Battle Sloth Table" -f "bs_tables" -a "Battle Sloth Software"
   
This app reads any .txt documents in from the source directory and packages them into a .mod file. The app creates a table entry for each line in the document. By default it names the table after the file name and creates a single column table. You can add a file header JSON string to the first line to control how the app processes the txt file.

Example: {"Name":"NPC Names","Description":"A bunch of names","Category":"My Tables","Columns":["Male","Female"],"Separator":"|"}

This will create a table with the name NPC Names with a description of A bunch of names. It will be a two column table with the columns being Male and Female. In the Tables dialog in Fantasy Grounds, this table will be in the My Tables list. It expects the dataset in the txt file to have the columns split by the '|' character None of these fields are required and any not present will use defaults.

The defaults are:
    Name - txt filename
    Description - blank
    Category - project name
    Columns - blank
    Separator - If no separator is specified, it assumes single column table

If you want to specify a ruleset, add a ruleset.txt file to your source directory and put each ruleset you want to specify on a separate line of the text file. Otherwise it defaults to ruleset = Any
