using database;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using System.Linq;

namespace Tests
{
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

        [TestMethod]
        public void ExecuteByeHelp()
        {
            String expected = "bye                           |-> quit";
            String actual = DbInterpretter.Execute("bye -h");
            Assert.AreEqual(expected, actual);
        }
    }

    [TestClass]
    public class CreateTests
    {
        [TestMethod]
        public void ExecuteCreateHelp()
        {
            String expected = "Create <table> (<field names>)                                    |-> Create a table on current db";
            String actual = DbInterpretter.Execute("create -h");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CreateWithoutColumns()
        {
            String expected = "Database unitTestingDB set active as empty. you can try to restore.";
            String actual = DbInterpretter.Execute("use unitTestingDB");
            Assert.AreEqual(expected, actual);
            expected = "Console error: Syntax error";
            actual = DbInterpretter.Execute("Create unitTestingTable1");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CreateWithColumns()
        {
            String expected = "Database unitTestingDB set active as empty. you can try to restore.";
            String actual = DbInterpretter.Execute("use unitTestingDB");
            Assert.AreEqual(expected, actual);
            expected = "Ok";
            actual = DbInterpretter.Execute("Create unitTestingTable1 (col1, col2)");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CreateWithoutParamsTest()
        {
            String expected = "Database unitTestingDB set active as empty. you can try to restore.";
            String actual = DbInterpretter.Execute("use unitTestingDB");
            Assert.AreEqual(expected, actual);
            expected = "Ceva Eroare??????? TO DO!";
            actual = DbInterpretter.Execute("create");
            Assert.AreEqual(expected, actual);
        }
    }

    [TestClass]
    public class UseTests
    {
        [TestMethod]
        public void ExecuteUseHelp()
        {
            String expected = "Use DBname                    |-> Set the DBname database active (as empty)";
            String actual = DbInterpretter.Execute("use -h");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UseWithoutParams()
        {
            String expected = "Invalid DBname";
            String actual = DbInterpretter.Execute("use");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CorrectUseOfUse()
        {
            String expected = "Database unitTestingDB set active as empty. you can try to restore.";
            String actual = DbInterpretter.Execute("use unitTestingDB");
            Assert.AreEqual(expected, actual);
        }
    }

    [TestClass]
    public class DropTests
    {
        [TestMethod]
        public void ExecuteDropHelp()
        {
            String expected = "Drop tbName                   |-> Drop table from current db";
            String actual = DbInterpretter.Execute("drop -h");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DropTableTest()
        {
            String expected = "Database unitTestingDB set active as empty. you can try to restore.";
            String actual = DbInterpretter.Execute("use unitTestingDB");
            Assert.AreEqual(expected, actual);
            expected = "Ok";
            actual = DbInterpretter.Execute("Create unitTestingTable1 (col1, col2)");
            Assert.AreEqual(expected, actual);
            expected = "Table unitTestingTable1 removed";
            actual = DbInterpretter.Execute("drop unitTestingTable1");
            Assert.AreEqual(expected, actual);
            expected = "Database error: No table named unitTestingTable1";
            actual = DbInterpretter.Execute("Select * from unitTestingTable1");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DropWithoutParamsTest()
        {
            String expected = "Database unitTestingDB set active as empty. you can try to restore.";
            String actual = DbInterpretter.Execute("use unitTestingDB");
            Assert.AreEqual(expected, actual);
            expected = "Database error: Table not in db.";
            actual = DbInterpretter.Execute("drop");
            Assert.AreEqual(expected, actual);
        }
    }

    [TestClass]
    public class DeleteTests
    {
        [TestMethod]
        public void ExecuteDeleteHelp()
        {
            String expected = "Delete from <table name> where <condition>                        |-> Delete table from current db";
            String actual = DbInterpretter.Execute("delete -h");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DeleteWithoutParamsTest()
        {
            String expected = "Database unitTestingDB set active as empty. you can try to restore.";
            String actual = DbInterpretter.Execute("use unitTestingDB");
            Assert.AreEqual(expected, actual);
            expected = "Database error: No table named ";
            actual = DbInterpretter.Execute("delete");
            Assert.AreEqual(expected, actual);
        }
    }

    [TestClass]
    public class SelectTests
    {
        [TestMethod]
        public void ExecuteSelectHelp()
        {
            String expected = "Select <*|fields list> from <table> [where <condition>]           |-> List tables from current db";
            String actual = DbInterpretter.Execute("select -h");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SelectWithoutParamsTest()
        {
            String expected = "Database unitTestingDB set active as empty. you can try to restore.";
            String actual = DbInterpretter.Execute("use unitTestingDB");
            Assert.AreEqual(expected, actual);
            expected = "Database error: No table named ";
            actual = DbInterpretter.Execute("select");
            Assert.AreEqual(expected, actual);
        }
    }

    [TestClass]
    public class HelpTests
    {
        [TestMethod]
        public void ExecuteHelpHelp()
        {
            String expected = "help                          |-> this help";
            String actual = DbInterpretter.Execute("help -h");
            Assert.AreEqual(expected, actual);
        }
    }

    [TestClass]
    public class SaveTests
    {
        [TestMethod]
        public void ExecuteSaveHelp()
        {
            String expected = "Save [path]                   |-> Saves the database to a provided or default path " +
                     "\nSave tableName [path] [CSV]   |-> Saves a table from current DB to a provided or default path";
            String actual = DbInterpretter.Execute("Save -h");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SaveWithoutParamsTest()
        {
            String expected = "Database unitTestingDB set active as empty. you can try to restore.";
            String actual = DbInterpretter.Execute("use unitTestingDB");
            Assert.AreEqual(expected, actual);
            expected = "Database error: Save to DB.unitTestingDB failed!";
            actual = DbInterpretter.Execute("save");
            Assert.AreEqual(expected, actual);
        }
    }

    [TestClass]
    public class RestoreTests
    {
        [TestMethod]
        public void ExecuteRestoreHelp()
        {
            String expected = "Restore [path [CSV]]          |-> Restore the database from a provided or default path " +
                     "\nRestore tbName [path [CSV]]   |-> Restore a table from current DB from a provided or default path";
            String actual = DbInterpretter.Execute("Restore -h");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void RestoreWithoutParamsTest()
        {
            String expected = "Database unitTestingDB set active as empty. you can try to restore.";
            String actual = DbInterpretter.Execute("use unitTestingDB");
            Assert.AreEqual(expected, actual);
            expected = "Database error: Invalid db load path path:DB.unitTestingDB";
            actual = DbInterpretter.Execute("restore");
            Assert.AreEqual(expected, actual);
        }
    }

    [TestClass]
    public class InsertTests
    {
        [TestMethod]
        public void ExecuteInsertHelp()
        {
            String expected = "Insert into <table name> (<fields list>) values (<values list>)   |-> Insert int table on current db";
            String actual = DbInterpretter.Execute("insert -h");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void InsertWithoutParamsTest()
        {
            String expected = "Database unitTestingDB set active as empty. you can try to restore.";
            String actual = DbInterpretter.Execute("use unitTestingDB");
            Assert.AreEqual(expected, actual);
            expected = "Database error: No table named ";
            actual = DbInterpretter.Execute("insert");
            Assert.AreEqual(expected, actual);
        }
    }

    [TestClass]
    public class UpdateTests
    {
        [TestMethod]
        public void ExecuteUpdateHelp()
        {
            String expected = "Update <table name> set <field=value>+ where <condition>          |-> Update table entries on selected fields";
            String actual = DbInterpretter.Execute("update -h");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UpdateWithoutParamsTest()
        {
            String expected = "Database unitTestingDB set active as empty. you can try to restore.";
            String actual = DbInterpretter.Execute("use unitTestingDB");
            Assert.AreEqual(expected, actual);
            expected = "Database error: No table named ";
            actual = DbInterpretter.Execute("update");
            Assert.AreEqual(expected, actual);
        }
    }
}