using System;
using NUnit.Framework;

namespace database
{
    [TestFixture]
    public class ByeTests
    {
        [Test]
        public void ByeTest()
        {
            String expected = "Bye!";
            String actual = DbInterpretter.Execute("bye");
            Assert.AreEqual(actual, expected);
            Assert.AreEqual(DbInterpretter.isRunning, false);
        }

        [Test]
        public void ExecuteByeHelp()
        {
            String expected = "bye                           |-> quit";
            String actual = DbInterpretter.Execute("bye -h");
            Assert.AreEqual(expected, actual);
        }
    }

    [TestFixture]
    public class CreateTests
    {
        [Test]
        public void ExecuteCreateHelp()
        {
            String expected = "Create <table> (<field names>)                                    |-> Create a table on current db";
            String actual = DbInterpretter.Execute("create -h");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CreateWithoutColumns()
        {
            String expected = "Database unitTestingDB set active as empty. you can try to restore.";
            String actual = DbInterpretter.Execute("use unitTestingDB");
            Assert.AreEqual(expected, actual);
            expected = "Console error: Syntax error";
            actual = DbInterpretter.Execute("Create unitTestingTable1");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CreateWithColumns()
        {
            String expected = "Database unitTestingDB set active as empty. you can try to restore.";
            String actual = DbInterpretter.Execute("use unitTestingDB");
            Assert.AreEqual(expected, actual);
            expected = "Ok";
            actual = DbInterpretter.Execute("Create unitTestingTable1 (col1, col2)");
            Assert.AreEqual(expected, actual);
        }

        [Test]
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

    [TestFixture]
    public class UseTests
    {
        [Test]
        public void ExecuteUseHelp()
        {
            String expected = "Use DBname                    |-> Set the DBname database active (as empty)";
            String actual = DbInterpretter.Execute("use -h");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void UseWithoutParams()
        {
            String expected = "Invalid DBname";
            String actual = DbInterpretter.Execute("use");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CorrectUseOfUse()
        {
            String expected = "Database unitTestingDB set active as empty. you can try to restore.";
            String actual = DbInterpretter.Execute("use unitTestingDB");
            Assert.AreEqual(expected, actual);
        }
    }

    [TestFixture]
    public class DropTests
    {
        [Test]
        public void ExecuteDropHelp()
        {
            String expected = "Drop tbName                   |-> Drop table from current db";
            String actual = DbInterpretter.Execute("drop -h");
            Assert.AreEqual(expected, actual);
        }

        [Test]
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

        [Test]
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

    [TestFixture]
    public class DeleteTests
    {
        [Test]
        public void ExecuteDeleteHelp()
        {
            String expected = "Delete from <table name> where <condition>                        |-> Delete table from current db";
            String actual = DbInterpretter.Execute("delete -h");
            Assert.AreEqual(expected, actual);
        }

        [Test]
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

    [TestFixture]
    public class SelectTests
    {
        [Test]
        public void ExecuteSelectHelp()
        {
            String expected = "Select <*|fields list> from <table> [where <condition>]           |-> List tables from current db";
            String actual = DbInterpretter.Execute("select -h");
            Assert.AreEqual(expected, actual);
        }

        [Test]
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

    [TestFixture]
    public class HelpTests
    {
        [Test]
        public void ExecuteHelpHelp()
        {
            String expected = "help                          |-> this help";
            String actual = DbInterpretter.Execute("help -h");
            Assert.AreEqual(expected, actual);
        }
    }

    [TestFixture]
    public class SaveTests
    {
        [Test]
        public void ExecuteSaveHelp()
        {
            String expected = "Save [path]                   |-> Saves the database to a provided or default path " +
                     "\nSave tableName [path] [CSV]   |-> Saves a table from current DB to a provided or default path";
            String actual = DbInterpretter.Execute("Save -h");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SaveCurrentDatabaseTest()
        {
            String expected = "Database unitTestingDB set active as empty. you can try to restore.";
            String actual = DbInterpretter.Execute("use unitTestingDB");
            Assert.AreEqual(expected, actual);
            expected = "Database unitTestingDB saved.";
            actual = DbInterpretter.Execute("save ");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SaveDefaultFormatTest()
        {
            String expected = "Database unitTestingDB set active as empty. you can try to restore.";
            String actual = DbInterpretter.Execute("use unitTestingDB");
            Assert.AreEqual(expected, actual);
            expected = "Database unitTestingDB saved.";
            actual = DbInterpretter.Execute("save D:\\");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SaveCSVFormatTest()
        {
            String expected = "Database unitTestingDB set active as empty. you can try to restore.";
            String actual = DbInterpretter.Execute("use unitTestingDB");
            Assert.AreEqual(expected, actual);
            expected = "Database unitTestingDB saved.";
            actual = DbInterpretter.Execute("save D:\\ CSV");
            Assert.AreEqual(expected, actual);
        }
    }

    [TestFixture]
    public class RestoreTests
    {
        [Test]
        public void ExecuteRestoreHelp()
        {
            String expected = "Restore [path [CSV]]          |-> Restore the database from a provided or default path " +
                     "\nRestore tbName [path [CSV]]   |-> Restore a table from current DB from a provided or default path";
            String actual = DbInterpretter.Execute("Restore -h");
            Assert.AreEqual(expected, actual);
        }

        [Test]
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

    [TestFixture]
    public class InsertTests
    {
        [Test]
        public void ExecuteInsertHelp()
        {
            String expected = "Insert into <table name> (<fields list>) values (<values list>)   |-> Insert int table on current db";
            String actual = DbInterpretter.Execute("insert -h");
            Assert.AreEqual(expected, actual);
        }

        [Test]
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

    [TestFixture]
    public class UpdateTests
    {
        [Test]
        public void ExecuteUpdateHelp()
        {
            String expected = "Update <table name> set <field=value>+ where <condition>          |-> Update table entries on selected fields";
            String actual = DbInterpretter.Execute("update -h");
            Assert.AreEqual(expected, actual);
        }

        [Test]
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

    [TestFixture]
    public class ConvertorTests
    {
        [Test]
        public void DefaultFormatEncodeTest1()
        {
            String expected = "";
            String actual = new DefaultFormatConverter().Encode(null);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DefaultFormatEncodeTest2()
        {
            String expected = "";
            string[] parameters = { };
            String actual = new DefaultFormatConverter().Encode(parameters);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DefaultFormatEncodeTest3()
        {
            string[] parameters = { "param1", "param2", "param3"};
            String expected = "6|param1|6|param2|6|param3|";
            String actual = new DefaultFormatConverter().Encode(parameters);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CSVEncodeTest1()
        {
            String expected = "";
            String actual = new CSVformatConverter().Encode(null);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CSVEncodeTest2()
        {
            String expected = "";
            string[] parameters = { };
            String actual = new CSVformatConverter().Encode(parameters);
            Assert.AreEqual(expected, actual);
        }


        [Test]
        public void CSVEncodeTest3()
        {
            string[] parameters = { "param1", "param2", "param3" };
            String expected = "param1;param2;param3";
            String actual = new CSVformatConverter().Encode(parameters);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DefaultFormatDecodeTest1()
        {
            string[] expected = { };
            string[] actual = new DefaultFormatConverter().Decode(null);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DefaultFormatDecodeTest2()
        {
            string[] expected = { };
            string[] actual = new DefaultFormatConverter().Decode("");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DefaultFormatDecodeTest3()
        {
            string[] expected = { "param1", "param2", "param3" };
            string parameter = "6|param1|6|param2|6|param3|";
            string[] actual = new DefaultFormatConverter().Decode(parameter);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CSVDecodeTest1()
        {
            string[] expected = { };
            string[] actual = new CSVformatConverter().Decode(null);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CSVDecodeTest2()
        {
            string[] expected = { };
            string[] actual = new CSVformatConverter().Decode("");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CSVDecodeTest3()
        {
            string[] expected = { "param1", "param2", "param3" };
            string parameter = "param1;param2;param3";
            string[] actual = new CSVformatConverter().Decode(parameter);
            Assert.AreEqual(expected, actual);
        }
    }
}
