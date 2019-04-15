using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace database
{
    public interface IFormatConverter
    {
        string Encode(string[] line);
        string[] Decode(string line);
    }
    public class DefaultFormatConverter : IFormatConverter
    {
        static char separator = '|';
        public string Encode(string[] line)
        {
            string result = "";
            foreach (string e in line)
                result += e.Length.ToString() + separator + e + separator;
            return result;
        }
        public string[] Decode(string line)
        {
            List<string> result = new List<string>();
            int len = 0, sepPos = 0;
            while (line.Length > 0)
                try
                {
                    sepPos = line.IndexOf(separator);
                    len = Int32.Parse(line.Substring(0, sepPos));
                    result.Add(line.Substring(sepPos + 1, len));
                    line = line.Substring(sepPos + 1 + len + 1);
                }
                catch { }
            return result.ToArray();
        }
    }
    public class CSVformatConverter : IFormatConverter
    {
        static char separator = ';';
        public string Encode(string[] line)
        {
            return String.Join(separator.ToString(), line);
        }
        public string[] Decode(string line)
        {
            return line.Split(separator);
        }
    }

    public class ColumnNames : Dictionary<string, int>
    {
        public ColumnNames(string[] names) { for (int i = 0; i < names.Length; i++) Add(names[i], i); }
        public string[] ToArray()
        {
            List<string> result = new List<string>();
            for (int i = 0; i++ < Keys.Count; result.Add("")) ;
            foreach (string k in Keys)
                result[this[k]] = k;
            return result.ToArray();
        }
        public bool Equals(ColumnNames oth)
        {
            foreach (bool eq in ToArray().Zip(oth.ToArray(), (f, s) => (f == s)))
                if (!eq) return false;
            return true;
        }
        new public string ToString() { return String.Join(" ", ToArray()); }
    }
    public class TableLine : List<string>
    {
        public class TableLineException : Exception { public TableLineException(string msg) : base(msg) { } }
        public bool DEBUG = true;
        public void print(string msg) { Console.Out.WriteLine(msg); }
        public void debug(string msg) { if (DEBUG) Console.Out.WriteLine("[TbL]" + msg); }
        public void check(bool cond, string except) { if (!cond) throw new TableLineException(except); }

        public ColumnNames columnName;

        public TableLine(ColumnNames names) { columnName = names; }
        public TableLine SetContent(string[] content)
        {
            check(columnName == null || content.Length == columnName.Count, "Can't set content to line");
            Clear();
            AddRange(content);
            return this;
        }
        public string this[string aFieldName]
        {
            get { return this[this.columnName[aFieldName]]; }
            set { this[this.columnName[aFieldName]] = value; }
        }
        new public string ToString() { return String.Join(" ", this); }
    }

    public class Table : List<TableLine>
    {
        public class TableException : Exception { public TableException(string msg) : base(msg) { } }
        public bool DEBUG = true;
        public void print(string msg) { Console.Out.WriteLine(msg); }
        public void debug(string msg) { if (DEBUG) Console.Out.WriteLine("[Tb]" + msg); }
        public void check(bool cond, string except) { if (!cond) throw new TableException(except); }

        public string name;
        public ColumnNames columnName;

        public Table(string aName, ColumnNames someColumnNames)
        {
            name = aName;
            columnName = someColumnNames;
        }
        public Table(string filePath, IFormatConverter conv)
        {
            name = Path.GetFileName(filePath);
            Restore(filePath, conv);
        }
        public Table AddLine(TableLine aLine)
        {
            check((aLine.columnName == null && columnName.Count == aLine.Count) || (aLine.columnName.Equals(columnName)), "Can't add line to table");
            Add(aLine);
            return this;
        }
        public Table AddLine(string[] aLineContent)
        {
            check(columnName.Count == aLineContent.Length, "Can't add line");
            AddLine(new TableLine(columnName).SetContent(aLineContent));
            return this;
        }
        public bool Save(string dirPath, IFormatConverter conv = null)
        {
            try
            {
                Directory.CreateDirectory(dirPath);
                if (conv == null) conv = new DefaultFormatConverter();
                StreamWriter file = new StreamWriter(Path.Combine(dirPath, name));
                file.WriteLine(conv.Encode(columnName.ToArray()));
                for (int i = 0; i < Count; i++)
                    file.WriteLine(conv.Encode(this[i].ToArray()));
                file.Close();
            }
            catch (Exception e) { throw new TableException(e.ToString()); }
            return true;
        }
        public bool Restore(string filePath, IFormatConverter conv = null)
        {
            string line;
            try
            {
                if (conv == null) conv = new DefaultFormatConverter();
                StreamReader file = new StreamReader(filePath);
                Clear();
                columnName = new ColumnNames(conv.Decode(file.ReadLine()));
                while ((line = file.ReadLine()) != null)
                    AddLine(conv.Decode(line));
            }
            catch (Exception e) { throw new TableException(e.ToString()); }
            return true;
        }
        public Table Where(Predicate<TableLine> lineIsValid)
        {
            Table result = new Table("result", columnName);
            try { result.AddRange(this.FindAll(lineIsValid)); }
            catch (Exception e) { throw new TableException(e.ToString()); }
            return result;
        }
        public Table Select(ColumnNames newColumns) { throw new NotImplementedException(); }
        new public string ToString()
        {
            List<string> result = new List<string>();
            result.Add(columnName.ToString());
            for (int i = 0; i < Count; i++)
                result.Add(i.ToString() + ": " + this[i].ToString());
            return String.Join("\n", result);
        }
    }
    public class Database : Dictionary<String, Table>
    {
        public class DatabaseException : Exception { public DatabaseException(string msg) : base(msg) { } }
        public bool DEBUG = true;
        public void print(string msg) { Console.Out.WriteLine(msg); }
        public void debug(string msg) { if (DEBUG) Console.Out.WriteLine("[DB]" + msg); }
        public void check(bool cond, string except) { if (!cond) throw new DatabaseException(except); }

        static string dbPrefix = "DB.";
        public string name;
        
        public string GetDirName() { return dbPrefix + name; }
        public string GetDbName(string path)
        {
            string fileName = Path.GetFileName(path);
            if (fileName.StartsWith(dbPrefix))
                return fileName.Substring(dbPrefix.Length);
            return fileName;
        }
        public Database(string dbName) { name = dbName; }
        public Database Add(Table t)
        {
            this[t.name] = t;
            return this;
        }
        public bool Save(string destDir = "", IFormatConverter conv = null)
        {
            if (destDir.Length == 0) destDir = GetDirName();
            try
            {
                Directory.Delete(destDir);
                Directory.CreateDirectory(destDir);
                foreach (Table table in Values)
                    table.Save(destDir, conv);
            }
            catch { throw new DatabaseException("Save to " + destDir + " failed!"); }
            return true;
        }
        public bool Restore(string srcPath = "", IFormatConverter conv = null)
        {
            if (srcPath.Length == 0) srcPath = GetDirName();
            try
            {
                check(Directory.Exists(srcPath), "Invalid db load path path:" + srcPath);
                Clear();
                foreach (string tablePath in Directory.EnumerateFiles(srcPath))
                    try { Add(new Table(tablePath, conv)); }
                    catch (Exception e){ debug("Can't add table from " + tablePath + ":\n  " + e.Message); }
            }
            catch { throw new DatabaseException("Can't retore from " + srcPath); }
            return true;
        }
        new public bool Remove(string t)
        {
            check (Keys.Contains(t), "Table not in db.");
            return base.Remove(t);
        }
    }

    public class DbConsole
    {
        public void print(string msg) { Console.Out.WriteLine(msg); }
        public void debug(string msg) { if (DEBUG) Console.Out.WriteLine("[DBconsole]" + msg); }
        public void check(bool cond, string except) { if (!cond) throw new ConsoleException(except); }

        public static bool DEBUG = true;
        public class ConsoleException : Exception
        {
            public ConsoleException(string msg) : base(msg) { }
        }
        public static Database noDb = new Database("No DB");
        public Database currentDb = noDb;

        public string HELP_Command(string param)
        {
            if (param == "-h")
                return "bye                           |-> quit";
            string result = "";
            foreach (MethodInfo m in this.GetType().GetMethods()
                .Where(mi => mi.Name.EndsWith("_Command")))
                result += m.Invoke(this, new object[] { "-h" }) + "\n";
            return result;
        }
        public string SAVE_Command(string param)
        {
            if (param == "-h")
                return "Save [path]                   |-> Saves the database to a provided or default path " +
                     "\nSave tableName [path]         |-> Saves a table from current DB to a provided or default path";
            check(currentDb != noDb, "Not allowed");
            if (param == "") { currentDb.Save(); return "Database " + currentDb.name + " saved."; }
            IFormatConverter conv = null;
            if (param.EndsWith(" CSV"))
            {
                conv = new CSVformatConverter();
                param = param.Substring(0, param.Length - 4);
            }
            if (param[1] == ':')
            {
                currentDb.Save(param, conv);
                return "Database " + currentDb.name + " saved.";
            }
            int fSpacePos = param.IndexOf(' ');
            if (fSpacePos == -1) fSpacePos = param.Length;
            string tbName = param.Substring(0, fSpacePos).Trim();
            string dstPath = param.Substring(fSpacePos).Trim();
            if (dstPath.Length > 0) currentDb[tbName].Save(dstPath, conv);
            else currentDb[tbName].Save(currentDb.GetDirName());

            return "Table " + tbName + " saved.";
        }
        public string RESTORE_Command(string param)
        {
            if (param == "-h")
                return "Restore [path [CSV]]          |-> Restore the database from a provided or default path " +
                     "\nRestore tbName [path [CSV]]   |-> Restore a table from current DB from a provided or default path";
            check(currentDb != noDb, "Not allowed");
            if (param == "") { currentDb.Restore(); return "Database " + currentDb.name + " restored."; }
            IFormatConverter conv = null;
            if (param.EndsWith(" CSV"))
            {
                conv = new CSVformatConverter();
                param = param.Substring(0, param.Length - 4);
            }
            if (param[1] == ':')
            {
                currentDb.Restore(param, conv);
                return "Database " + currentDb.name + " restored.";
            }
            int fSpacePos = param.IndexOf(' ');
            if (fSpacePos == -1) fSpacePos = param.Length;
            string tbName = param.Substring(0, fSpacePos).Trim();
            string dstPath = param.Substring(fSpacePos).Trim();
            if (dstPath.Length > 0) currentDb[tbName].Restore(dstPath, conv);
            else currentDb[tbName].Restore(currentDb.GetDirName());

            return "Table " + tbName + " restored.";
        }
        public string CONNECT_Command(string param)
        {
            if (param == "-h")
                return "Connect DBname                |-> Set the DBname database active (as empty)";
            check(param.Length == 0 || param.Split().Length > 0, "Invalid DBname");
            currentDb = new Database(param);

            return "Database " + currentDb.name + " set active as empty. you can try to restore.";
        }
        public string DROP_Command(string param)
        {
            if (param == "-h")
                return "Drop tbName                   |-> drop table from current db";
            check(currentDb != noDb, "Not allowed");
            currentDb.Remove(param);
            return "Table " + param + " removed";
        }

        public string CREATE_Command0(string param) { throw new NotImplementedException(); }
        public string INSERT_Command0(string param) { throw new NotImplementedException(); }
        public string UPDATE_Command0(string param) { throw new NotImplementedException(); }
        public string DELETE_Command0(string param) { throw new NotImplementedException(); }

        public string SELECT_Command(string param)
        {
            if (param == "-h")
                return "Select * from TABLE_NAMES     |-> List tables from current db";
            //check(currentDb != noDb, "Not allowed");
            // work in progress
            string knownTerm = "(\\w+)";
            string strTerm = "(\"([\\w ]+)\")";
            string term = $"({knownTerm}|{strTerm})";
            string terms = $"({term}(\\s*,\\s*{term})*)";
            string fieldsList = $"((\\*)|{terms})";
            string op = "(=|(!=)|(\\sbeginswith\\s)|(\\sendswith\\s))";
            string condition = $"({knownTerm}\\s*{op}\\s*{term})";
            Regex commFormat = new Regex($"{fieldsList}\\s+from\\s+{term}(\\s*where\\s*{condition})?", RegexOptions.IgnoreCase);
            MatchCollection commFormatMatch = Regex.Matches(param, term);
            foreach (Match aMatch in commFormatMatch) {
                debug("new capt");
                foreach (Capture aCapt in aMatch.Captures )
                {
                    debug("Capt " + aCapt.Index + ":" + aCapt.Value);
                }
            }
            //if (commFormatMatch.Success) debug("Match:"+commFormatMatch.Captures);
            return String.Join("\n", currentDb.Keys);
        }

        public void run()
        {
            string textCommand = "", command = "", parameters = "";
            Console.Out.Write(currentDb.name + ">");
            while ((textCommand = Console.In.ReadLine().Trim()) != "bye")
                try
                {
                    int fSpacePos = textCommand.IndexOf(' ');
                    if (fSpacePos == -1) fSpacePos = textCommand.Length;
                    command = textCommand.Substring(0, fSpacePos).Trim().ToUpper();
                    parameters = textCommand.Substring(fSpacePos).Trim();
                    try
                    {
                        Console.Out.WriteLine((string)this.GetType().GetMethod(command + "_Command")
                            .Invoke(this, new object[] { parameters }));
                    }
                    catch (Exception e) { throw e.InnerException; }
                }
                catch (Database.DatabaseException e) { Console.Out.WriteLine("Database error: " + e.Message); }
                catch (DbConsole.ConsoleException e) { Console.Out.WriteLine("Console error: " + e.Message); }
                catch (Exception e)
                {
                    Console.Out.WriteLine("Invalid command: " + command);
                    try { debug(e.InnerException.Message); }
                    catch { debug(e.Message); }
                }
                finally { Console.Out.Write(currentDb.name + ">"); }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //Dictionary<String, Database> dbs = Database.GetDBs();
            /*
            ColumnNames col1 = new ColumnNames(new string[] { "C1", "C2", "C3" });
            ColumnNames col2 = new ColumnNames(new string[] { "C1", "C2", "C3" });
            Database newdb = new Database("testDB");
            dbs[newdb.name]= newdb;
            Table t1 = new Table("table1", col1)
                .AddLine(new TableLine(col1).SetContent(new string[] { "p11", "p12", "p13" }))
                .AddLine(new TableLine(col1).SetContent(new string[] { "p21", "p22", "p23" }));
            Table t2 = new Table("table2", col2)
                .AddLine(new TableLine(col2).SetContent(new string[] { "o11", "o12", "o13" }))
                .AddLine(new TableLine(col2).SetContent(new string[] { "o21", "o22", "o23" }));
            newdb.Add(t1).Add(t2);
            Console.Out.WriteLine(newdb.Save());
            */
            //Console.Out.WriteLine("dbs:" + String.Join(",", dbs.Keys));
            /*
            Database db = new Database("testDB");
            Console.Out.WriteLine("[testDB] tables: " + String.Join(",", db.Keys));
            db.Save("D:\\MyDocuments\\Desktop\\test", new CSVformatConverter());
            db.Restore("D:\\MyDocuments\\Desktop\\test", new CSVformatConverter());
            Console.Out.WriteLine("[testDB] tables: " + String.Join(",", db.Keys));
            Table filterResult = db["table1"].Filter(entry => entry["C1"].Equals("p21"));
            Console.Out.WriteLine(filterResult.ToString());
            filterResult.Save("D:\\MyDocuments\\Desktop\\test", new CSVformatConverter());
            */
            new DbConsole().run();
        }
    }
}
