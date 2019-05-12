using System;
using NUnit.Framework;
using NMock;
using System.Diagnostics.CodeAnalysis;

namespace database
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
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
    [ExcludeFromCodeCoverage]
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
            expected = "Ok";
            actual = DbInterpretter.Execute("create");
            Assert.AreEqual(expected, actual);
        }
    }

    [TestFixture]
    [ExcludeFromCodeCoverage]
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
    [ExcludeFromCodeCoverage]
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
    [ExcludeFromCodeCoverage]
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
    [ExcludeFromCodeCoverage]
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

        [Test]
        public void SelectWithoutTableTest()
        {
            String expected = "Database unitTestingDB set active as empty. you can try to restore.";
            String actual = DbInterpretter.Execute("use unitTestingDB");
            Assert.AreEqual(expected, actual);
            expected = "Database error: No table named unitTestingDB";
            actual = DbInterpretter.Execute("select * from unitTestingDB");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SimpleSelectTest()
        {
            String expected = "Database unitTestingDB set active as empty. you can try to restore.";
            String actual = DbInterpretter.Execute("use unitTestingDB");
            Assert.AreEqual(expected, actual);
            expected = "Ok";
            actual = DbInterpretter.Execute("create tab (col1, col2)");
            Assert.AreEqual(expected, actual);
            expected = "result:\ncol1 col2";
            actual = DbInterpretter.Execute("Select * from tab");
            Assert.AreEqual(expected, actual);
            expected = "Ok";
            actual = DbInterpretter.Execute("Insert into tab (col1, col2) values (content1, content2)");
            Assert.AreEqual(expected, actual);
            expected = "result:\n    col1     col2\ncontent1 content2";
            actual = DbInterpretter.Execute("Select * from tab");
            Console.WriteLine(expected);
            Console.WriteLine(actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SelectWithWhereTest1()
        {
            String expected = "Database unitTestingDB set active as empty. you can try to restore.";
            String actual = DbInterpretter.Execute("use unitTestingDB");
            Assert.AreEqual(expected, actual);
            expected = "Ok";
            actual = DbInterpretter.Execute("create tab (col1, col2)");
            Assert.AreEqual(expected, actual);
            expected = "result:\ncol1 col2";
            actual = DbInterpretter.Execute("Select * from tab");
            Assert.AreEqual(expected, actual);
            expected = "Ok";
            actual = DbInterpretter.Execute("Insert into tab (col1, col2) values (content1, content2)");
            Assert.AreEqual(expected, actual);
            expected = "result:\n    col1     col2\ncontent1 content2";
            actual = DbInterpretter.Execute("Select * from tab where col1=='content1' and col2 endswith 'nt2'");
            Assert.AreEqual(expected, actual);
        }


        [Test]
        public void SelectWithWhereTest2()
        {
            String expected = "Database unitTestingDB set active as empty. you can try to restore.";
            String actual = DbInterpretter.Execute("use unitTestingDB");
            Assert.AreEqual(expected, actual);
            expected = "Ok";
            actual = DbInterpretter.Execute("create tab (col1, col2)");
            Assert.AreEqual(expected, actual);
            expected = "result:\ncol1 col2";
            actual = DbInterpretter.Execute("Select * from tab");
            Assert.AreEqual(expected, actual);
            expected = "Ok";
            actual = DbInterpretter.Execute("Insert into tab (col1, col2) values (content1, content2)");
            Assert.AreEqual(expected, actual);
            expected = "result:\n    col1     col2\ncontent1 content2";
            actual = DbInterpretter.Execute("Select * from tab where col1!='content1' or col2 endswith 'nt2'");
            Assert.AreEqual(expected, actual);

            expected = "result:\n    col1     col2\ncontent1 content2";
            actual = DbInterpretter.Execute("Select * from tab where col1=='content1' or col2 startswith 'nt2'");
            Assert.AreEqual(expected, actual);
        }


        [Test]
        public void SelectWithWhereTest3()
        {
            String expected = "Database unitTestingDB set active as empty. you can try to restore.";
            String actual = DbInterpretter.Execute("use unitTestingDB");
            Assert.AreEqual(expected, actual);
            expected = "Ok";
            actual = DbInterpretter.Execute("create tab (col1, col2)");
            Assert.AreEqual(expected, actual);
            expected = "result:\ncol1 col2";
            actual = DbInterpretter.Execute("Select * from tab");
            Assert.AreEqual(expected, actual);
            expected = "Ok";
            actual = DbInterpretter.Execute("Insert into tab (col1, col2) values (content1, content2)");
            Assert.AreEqual(expected, actual);

            expected = "result:\n    col1     col2\ncontent1 content2";
            actual = DbInterpretter.Execute("Select * from tab where col1!='c'");
            //Assert.AreEqual(expected, actual);

            expected = "result:\n    col1     col2\ncontent1 content2";
            actual = DbInterpretter.Execute("Select * from tab where col1=='content1'");
            //Assert.AreEqual(expected, actual);

            expected = "result:\n    col1     col2\ncontent1 content2";
            actual = DbInterpretter.Execute("Select * from tab where col1 startswith 'c'");
            // Assert.AreEqual(expected, actual);

            expected = "result:\n    col1     col2\ncontent1 content2";
            actual = DbInterpretter.Execute("Select * from tab where col1 endswith '1'");
            //Assert.AreEqual(expected, actual);

        }

        [Test]
        public void SelectWithWhereTest4()
        {
            String expected = "Database unitTestingDB set active as empty. you can try to restore.";
            String actual = DbInterpretter.Execute("use unitTestingDB");
            Assert.AreEqual(expected, actual);
            expected = "Ok";
            actual = DbInterpretter.Execute("create tab (col1, col2)");
            Assert.AreEqual(expected, actual);
            expected = "result:\ncol1 col2";
            actual = DbInterpretter.Execute("Select * from tab");
            Assert.AreEqual(expected, actual);
            expected = "Ok";
            actual = DbInterpretter.Execute("Insert into tab (col1, col2) values ('content1', 'content2')");
            Assert.AreEqual(expected, actual);

            expected = "result:\ncol1 col2";
            actual = DbInterpretter.Execute("Select * from tab where col1!='content1'");
            Assert.AreEqual(expected, actual);

            expected = "result:\ncol1 col2";
            actual = DbInterpretter.Execute("Select * from tab where col1=='content12'");
            Assert.AreEqual(expected, actual);

            expected = "result:\ncol1 col2";
            actual = DbInterpretter.Execute("Select * from tab where col1 startswith 'c2'");
            Assert.AreEqual(expected, actual);

            expected = "result:\ncol1 col2";
            actual = DbInterpretter.Execute("Select * from tab where col1 endswith '12'");
            Assert.AreEqual(expected, actual);




            expected = "result:\ncol1 col2";
            actual = DbInterpretter.Execute("Select * from tab where col1!=col1");
            Assert.AreEqual(expected, actual);

            expected = "result:\ncol1 col2";
            actual = DbInterpretter.Execute("Select * from tab where col1==col2");
            Assert.AreEqual(expected, actual);

            expected = "result:\ncol1 col2";
            actual = DbInterpretter.Execute("Select * from tab where col1 startswith col2");
            Assert.AreEqual(expected, actual);

            expected = "result:\ncol1 col2";
            actual = DbInterpretter.Execute("Select * from tab where col1 endswith col2");
            Assert.AreEqual(expected, actual);

        }
    }


    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class HelpTests
    {
        [Test]
        public void ExecuteHelpHelp()
        {
            String expected = "help                          |-> this help";
            String actual = DbInterpretter.Execute("help -h");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ExecuteHelpNotHelp()
        {
            String actual = DbInterpretter.Execute("help");
            Assert.Greater(actual.Length, 0);

        }
    }

    [TestFixture]
    [ExcludeFromCodeCoverage]
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
            actual = DbInterpretter.Execute("save");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SaveDefaultFormatTest()
        {
            DbInterpretter.Execute("use unitTestingDB");
            DbInterpretter.Execute("create tab (col1, col2)");
            DbInterpretter.Execute("Insert into tab (col1, col2) values (content1, content2)");

            string expected = "Database unitTestingDB saved.";
            string actual = DbInterpretter.Execute("save");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SaveCSVFormatTest()
        {
            String expected = "Database unitTestingDB set active as empty. you can try to restore.";
            String actual = DbInterpretter.Execute("use unitTestingDB");
            Assert.AreEqual(expected, actual);
            expected = "Database unitTestingDB saved.";
            actual = DbInterpretter.Execute("save D:\\Facultate_Master\\ISS\\sem2\\CSS\\Project\\CSS-database\\database\\bin\\Debug\\csv CSV");
            Assert.AreEqual(expected, actual);
        }
    }

    [TestFixture]
    [ExcludeFromCodeCoverage]
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
    [ExcludeFromCodeCoverage]
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
    [ExcludeFromCodeCoverage]
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
        [Test]
        public void UpdateWithParamsTest()
        {
            DbInterpretter.Execute("use unitTestingDB");
            DbInterpretter.Execute("create tab (col1, col2)");
            DbInterpretter.Execute("Insert into tab (col1, col2) values (content1, content2)");

            String expected = "Ok";
            String actual = DbInterpretter.Execute("update tab set col2='new' where col1=='content1'");
            Assert.AreEqual(expected, actual);
        }
    }

    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class ConvertorTests
    {
        [Test]
        public void DefaultFormatEncodeTest1()
        {
            String expected = "0||";
            String actual = new DefaultFormatConverter().Encode(new string[] { "" });
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
            string[] parameters = { "param1", "param2", "param3" };
            String expected = "6|param1|6|param2|6|param3|";
            String actual = new DefaultFormatConverter().Encode(parameters);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CSVEncodeTest1()
        {
            String expected = "";
            String actual = new CSVformatConverter().Encode(new string[] { "" });
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CSVEncodeTest2()
        {
            String expected = "str1;str2";
            string[] parameters = { "str1", "str2" };
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
            string[] expected = { "str1", "str2" };
            string[] actual = new CSVformatConverter().Decode("str1;str2");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CSVDecodeTest2()
        {
            string[] expected = { "" };
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

    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class TableTests
    {
        private MockFactory mockFactory;

        [Test]
        public void addLineTest1()
        {
            string[] cols = { "col1", "col2", "col3" };
            ColumnNames colNames = new ColumnNames(cols);
            Assert.AreEqual(colNames.ToString(), "col1 col2 col3");
            Assert.AreNotEqual(colNames.ToString(), "col1 col2 col33");
            Table testTable1 = new Table("testTable", colNames);
            string[] tableLine = new string[] { "col1Content", "col2Content", "col3Content" };
            testTable1.AddLine(tableLine);
            String expected = "testTable:\n       col1        col2        col3\ncol1Content col2Content col3Content";
            String actual = testTable1.ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void addLineTest2()
        {
            string[] cols = { "col1", "col2", "col3" };
            ColumnNames colNames = new ColumnNames(cols);
            Assert.AreEqual(colNames.ToString(), "col1 col2 col3");
            Assert.AreNotEqual(colNames.ToString(), "col1 col2 col33");
            Table testTable1 = new Table("testTable", colNames);
            string[] tableLine = new string[] { "col1Content", "col2Content", "col3Content" };
            TableLine tl = new TableLine(colNames);
            tl = tl.SetContent(tableLine);
            TableLine tl2 = tl.Select(colNames);
            testTable1.AddLine(tl2);
            String expected = "testTable:\n       col1        col2        col3\ncol1Content col2Content col3Content";
            String actual = testTable1.ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SaveRestoreTableTest1()
        {
            string[] cols = { "col1", "col2", "col3" };
            Table testTable1 = new Table("testTable", new ColumnNames(cols));
            string[] tableLine = new string[] { "col1Content", "col2Content", "col3Content" };
            testTable1.AddLine(tableLine);
            mockFactory = new MockFactory();
            var converter = mockFactory.CreateMock<IFormatConverter>();
            converter.Expects.One.MethodWith(_ => _.Encode(cols)).WillReturn("col1;col2;col3");
            converter.Expects.One.MethodWith(_ => _.Encode(tableLine)).WillReturn("col1Content;col2Content;Col3Content");
            bool saved = testTable1.Save("D:\\", converter.MockObject);
            converter.Expects.One.MethodWith(_ => _.Decode("col1;col2;col3")).WillReturn(cols);
            converter.Expects.One.MethodWith(_ => _.Decode("col1Content;col2Content;Col3Content")).WillReturn(tableLine);
            Table testTable2 = new Table("D:\\testTable", converter.MockObject);

            mockFactory.VerifyAllExpectationsHaveBeenMet();
            Assert.AreEqual(saved, true);
            Assert.AreEqual(testTable1.ToString(), testTable2.ToString());
        }
    }
    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class RunTests
    {
        [Test]
        public void ConsoleInterfaceTest()
        {
            DbInterpretter.isRunning = false;
            string[] init = {
                "use testDB",
                "restore",
                "select * from table1",
                };
            DB_ConsoleInterface.Run(init);
        }

        [Test]
        public void WindowInterfaceTest()
        {
            DbInterpretter.isRunning = true;
            string[] init = {
                "use testDB",
                "restore",
                "select * from table1",
                };
            DB_WindowInterface.Run(init);
        }
    }

    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class ColumnNamesTest
    {
        [Test]
        public void ColumnNamesEqualsTestReturnFalse()
        {
            string[] numeCol1 = { "col11", "col12", "col13" };
            ColumnNames columnName1 = new ColumnNames(numeCol1);

            string[] numeCol2 = { "col21", "col22", "col23" };
            ColumnNames columnName2 = new ColumnNames(numeCol2);
            Assert.AreEqual(columnName1.Equals(columnName2), false);
        }

        [Test]
        public void ColumnNamesEqualsTestReturnTrue()
        {
            string[] numeCol1 = { "col1", "col2", "col3" };
            ColumnNames columnName1 = new ColumnNames(numeCol1);

            string[] numeCol2 = { "col1", "col2", "col3" };
            ColumnNames columnName2 = new ColumnNames(numeCol2);
            Assert.AreEqual(columnName1.Equals(columnName2), true);
        }

        [Test]
        public void ColumnNamesAllInTestReturnFalse()
        {
            string[] numeCol1 = { "col1", "col2", "col3" };
            ColumnNames columnName1 = new ColumnNames(numeCol1);

            string[] numeCol2 = { "col1", "col2", "col4" };
            ColumnNames columnName2 = new ColumnNames(numeCol2);
            Assert.AreEqual(columnName1.AllIn(columnName2), false);

        }
    }

    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class TableExceptionTest
    {
        [Test]
        public void TableExceptionTest1()
        {
            Table.TableException dex = Assert.Throws<Table.TableException>(delegate { throw new Table.TableException("Done"); });
            Assert.That(dex.Message, NUnit.Framework.Is.EqualTo("Done"));
        }

        [Test]
        public void TableLineExceptionTest1()
        {
            TableLine.TableLineException dex = Assert.Throws<TableLine.TableLineException>(delegate { throw new TableLine.TableLineException("Done"); });
            Assert.That(dex.Message, NUnit.Framework.Is.EqualTo("Done"));
        }
    }

    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class TestMainCode
    {
        [Test]
        public void TestProgram()
        {
            bool ok = false;
            try
            {
                string[] voidString = { "" };
                Program.Main(voidString);
                ok = true;
            }
            catch { }
            Assert.AreEqual(ok, true);
        }
    }

    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class TableTest
    {
        [Test]
        public void TestTableSelect()
        {
            string[] numeCol1 = { "col11", "col12", "col13" };
            ColumnNames columnName1 = new ColumnNames(numeCol1);

            Table testTable = new Table("testTable", columnName1);
            testTable.Add(new TableLine(columnName1).SetContent(new string[] {"t1", "t2", "t3"}));
            Table result = testTable.Select(columnName1);
            Assert.AreEqual(result.ToString(), "result:\ncol11 col12 col13\n   t1    t2    t3");
        }

        [Test]
        public void TestTableToString()
        {
            string[] numeCol1 = { "col11", "col12", "col13" };
            ColumnNames columnName1 = new ColumnNames(numeCol1);

            Table testTable = new Table("testTable", columnName1);
            testTable.Add(new TableLine(columnName1).SetContent(new string[] { "t1", "t2", "t3" }));
            Table result = testTable.Select(columnName1);
            Assert.AreEqual(testTable[0].ToString(), "t1 t2 t3");
        }

        [Test]
        public void TestTableCheck()
        {
            Table.TableException dex = Assert.Throws<Table.TableException>(delegate { Table.check(false, "DoneTesting"); });
            Assert.That(dex.Message, NUnit.Framework.Is.EqualTo("DoneTesting"));
        }

        [Test]
        public void TestTableAddLine()
        {
            string[] numeCol1 = { "col11", "col12", "col13" };
            ColumnNames columnName1 = new ColumnNames(numeCol1);

            TableLine customTableLine = new TableLine(null).SetContent(new string[] {"t1", "t2", "t3"});
            Table customTable = new Table("customTable", columnName1);
            Assert.AreEqual(customTable.AddLine(customTableLine).ToString(), "customTable:\ncol11 col12 col13\n   t1    t2    t3"); 

        }

        [Test]
        public void TestTableSave()
        {
            string[] numeCol1 = { "col11", "col12", "col13" };
            ColumnNames columnName1 = new ColumnNames(numeCol1);

            Table testTable = new Table("testTable", columnName1);
            Table.TableException dex = Assert.Throws <Table.TableException>(delegate { testTable.Save("X:\\"); });
            Assert.Greater(dex.Message.Length, 0);

        }

        [Test]
        public void TestTableRestore()
        {
            string[] numeCol1 = { "col11", "col12", "col13" };
            ColumnNames columnName1 = new ColumnNames(numeCol1);

            Table testTable = new Table("testTable", columnName1);
            Table.TableException dex = Assert.Throws<Table.TableException>(delegate { testTable.Restore("X:\\"); });
            Assert.Greater(dex.Message.Length, 0);

        }

        [Test]
        public void TestTableToString2()
        {
            string[] numeCol1 = { "col11", "col12", "col12345678901234568789125297547328573298579832759832759832798573987539827598327598732985739875983729857398573982759832759873298753298759832798" };
            ColumnNames columnName1 = new ColumnNames(numeCol1);

            Table testTable = new Table("testTable", columnName1);
            Assert.AreEqual(testTable.ToString(), "testTable:\ncol11 col12 col123456789012345687891252975");
        }

        [Test]
        public void TestAddLines()
        {
            string[] numeCol1 = { "col11", "col12", "col13" };
            ColumnNames columnName1 = new ColumnNames(numeCol1);

            Table testTable = new Table("testTable", columnName1);

            string[] numeCol2 = { "col11", "col12" };
            ColumnNames columnName2 = new ColumnNames(numeCol2);
            TableLine tableLine = new TableLine(columnName2);

            Assert.AreEqual(testTable.AddLine(tableLine).ToString(), "testTable:\ncol11 col12 col13\n                 ");
        }

        [Test]
        public void TestWhere()
        {
            string[] numeCol1 = { "col11", "col12", "col13" };
            ColumnNames columnName1 = new ColumnNames(numeCol1);

            Table testTable = new Table("testTable", columnName1);
            Table.TableException dex = Assert.Throws<Table.TableException>(delegate { testTable.Where(null); });
            Assert.Greater(dex.Message.Length, 0);
        }

    }

    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class DbInterpreterTests
    {
        [Test]
        public void RestoreTest()
        {
                String expected = "Database error: No table named unitTestingDB";
                String actual = DbInterpretter.Execute("restore unitTestingDB");

                Assert.AreEqual(expected, actual);

                DbInterpretter.Execute("use unitTestingDB");
                actual = DbInterpretter.Execute("restore");
                Assert.AreEqual("Database unitTestingDB restored.", actual);
        }

        [Test]
        public void SaveCommandTest()
        {
            DbInterpretter.Execute("use unitTestingDB");
            DbInterpretter.Execute("create tab (col1, col2)");
            DbInterpretter.Execute("Insert into tab (col1, col2) values (content1, content2)");
            //Assert.AreEqual(DbInterpretter.Execute("save d:\\"), "Database unitTestingDB saved.");
            Assert.AreEqual(DbInterpretter.Execute("save tab d:\\"), "Table tab saved.");
        }
    }

    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class DataBaseTests
    {
        [Test]
        public void TestSave1()
        {
            Database db = new Database("tempName");
            db.Add(new Table("temp", new ColumnNames(new string[] { "Ingrid", "Uli", "Valeriu", "Gigi" })));
            Database.DatabaseException dex = Assert.Throws<Database.DatabaseException>(delegate { db.Save("X:\\"); });
           // db.Save("D:\\Facultate_Master\\ISS\\sem2\\CSS\\Project\\CSS-database\\database\\bin\\Debug\\.DB");
            Assert.That(dex.Message, NUnit.Framework.Is.EqualTo("Save to X:\\ failed!"));
        }

        [Test]
        public void TestRestore()
        {
            Database db = new Database("tempName");
            Database.DatabaseException dex = Assert.Throws<Database.DatabaseException>(delegate { db.Restore("D:\\Facultate_Master\\ISS\\sem2\\CSS\\Project\\CSS-database\\database\\bin\\Debug\\tempDatabase"); });
            Assert.That(dex.Message, NUnit.Framework.Is.EqualTo("Can't add table from D:\\Facultate_Master\\ISS\\sem2\\CSS\\Project\\CSS-database\\database\\bin\\Debug\\tempDatabase\\temp"));
        }
    }
}
