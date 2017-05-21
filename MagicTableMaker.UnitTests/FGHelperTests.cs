using System;
using System.Collections.Generic;
using MagicTableMaker.FantasyGroundsObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MagicTableMaker.UnitTests
{
    [TestClass]
    public class FGHelperTests
    {
        private FantasyGroundsXmlHelper helper;

        [TestInitialize]
        public void Initialize()
        {
            helper = new FantasyGroundsXmlHelper();
        }


        [TestMethod]
        public void CreateEntry()
        {

            var result = helper.GenerateEntry("Test Proj", "storytemplate", "Story Template");

            var test = result.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(test));
        }

        [TestMethod]
        public void CreateLibrary()
        {
            var result = helper.GenerateLibrary("testproj", "Test Proj",
                "Random Generators", new List<string> { "storytemplate", "table" });

            var test = result.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(test));
        }

        [TestMethod]
        public void CreateDoc()
        {
            var project = new Project();

            project.FileName = "testproj";
            project.Name = "Test Proj";
            project.Category = "Random Tables";
            project.Entries = new List<string> { "table" };
            project.Tables = new List<IFantasyGroundsObject>
            {
                    new Table
                    {
                        Name = "Test Table 1",
                        Description = "This i a test table",
                        Category = project.Category,
                        Columns = new List<string>{"Column Title"},
                        Notes = "<p>notes</p>",
                        Rows = new List<List<string>>
                        {
                            new List<string>{"Alpha"},
                            new List<string>{"Beta"},
                            new List<string>{"Delta"},
                            new List<string>{"Gamma"},
                            new List<string>{"Omega"},
                        }
                    }

            };



            var result = helper.GenerateDbXml("3.3", "7|CoreRPG:3", project);

            var test = result.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(test));
        }
    }
}
