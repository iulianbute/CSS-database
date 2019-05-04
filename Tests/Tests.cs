using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using database;

namespace Tests
{
    [TestClass]
    public class ExecuteHelpTests
    {
        [TestMethod]
        public void ExecuteSelectHelp()
        {
            String expected = "Select <*|fields list> from <table> [where <condition>]           |-> List tables from current db";
            String actual = DbInterpretter.Execute("select -h");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ExecuteDeleteHelp()
        {
            String expected = "Delete from <table name> where <condition>                        |-> Delete table from current db";
            String actual = DbInterpretter.Execute("delete -h");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ExecuteUpdateHelp()
        {
            String expected = "Update <table name> set <field=value>+ where <condition>          |-> Update table entries on selected fields";
            String actual = DbInterpretter.Execute("update -h");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ExecuteInsertHelp()
        {
            String expected = "Insert into <table name> (<fields list>) values (<values list>)   |-> Insert int table on current db";
            String actual = DbInterpretter.Execute("insert -h");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ExecuteCreateHelp()
        {
            String expected = "Create <table> (<field names>)                                    |-> Create a table on current db";
            String actual = DbInterpretter.Execute("create -h");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ExecuteDropHelp()
        {
            String expected = "Drop tbName                   |-> Drop table from current db";
            String actual = DbInterpretter.Execute("drop -h");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ExecuteUseHelp()
        {
            String expected = "Use DBname                    |-> Set the DBname database active (as empty)";
            String actual = DbInterpretter.Execute("use -h");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ExecuteRestoreHelp()
        {
            String expected = "Restore [path [CSV]]          |-> Restore the database from a provided or default path " +
                     "\nRestore tbName [path [CSV]]   |-> Restore a table from current DB from a provided or default path";
            String actual = DbInterpretter.Execute("Restore -h");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ExecuteSaveHelp()
        {
            String expected = "Save [path]                   |-> Saves the database to a provided or default path " +
                     "\nSave tableName [path] [CSV]   |-> Saves a table from current DB to a provided or default path";
            String actual = DbInterpretter.Execute("Save -h");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ExecuteHelpHelp()
        {
            String expected = "help                          |-> this help";
            String actual = DbInterpretter.Execute("help -h");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ExecuteByeHelp()
        {
            String expected = "bye                           |-> quit";
            String actual = DbInterpretter.Execute("bye -h");
            Assert.AreEqual(expected, actual);
        }
    }

    [TestClass]
    public class RunInitTests
    {
        [TestMethod]
        public void ConsoleInterfaceInitTest()
        {
            DbInterpretter.isRunning = false;
            string[] init = {
                "use testDB",
                "restore",
                "select * from table1",
                };
            DB_ConsoleInterface.Run(init);
        }

        [TestMethod]
        public void WindowInterfaceInitTest()
        {
            DbInterpretter.isRunning = false;
            string[] init = {
                "use testDB",
                "restore",
                "select * from table1",
                };
            DB_WindowInterface.Run(init);
        }
    }

    [TestClass]
    public class ByeTests
    {
        [TestMethod]
        public void ByeTest()
        {
            String expected = "Bye!";
            String actual = DbInterpretter.Execute("bye");
            Assert.AreEqual(actual, expected);
            Assert.AreEqual(DbInterpretter.isRunning, false);
        }
    }

}
