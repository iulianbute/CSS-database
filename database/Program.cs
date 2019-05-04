using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace database
{
    public class Debug
    {
        public static bool on = true;
        public static void Write(string msg)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            var type = method.DeclaringType.ToString();
            var name = method.Name;

            if (on) Console.Out.WriteLine($"[{type}][{name}]" + msg);
        }
    }

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
        public bool AllIn(ColumnNames oth)
        {
            foreach (string k in Keys)
                if (!oth.Keys.Contains(k))
                    return false;
            return true;
        }
        new public string ToString() { return String.Join(" ", ToArray()); }
    }
    public class TableLine : List<string>
    {
        public class TableLineException : Exception { public TableLineException(string msg) : base(msg) { } }
        public static void check(bool cond, string except) { if (!cond) throw new TableLineException(except); }

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
            get
            {
                check(columnName != null, "No column names are set yet");
                check(columnName.Keys.Contains(aFieldName), "Invalid column name: " + aFieldName);
                return this[this.columnName[aFieldName]];
            }
            set
            {
                check(columnName != null, "No column names are set yet");
                this[this.columnName[aFieldName]] = value;
            }
        }
        public TableLine Select(ColumnNames selColNames)
        {
            List<string> selContent = new List<string>();
            foreach (string colName in selColNames.ToArray())
            {
                selContent.Add(this[colName]);
            }
            return new TableLine(selColNames).SetContent(selContent.ToArray());
        }
        new public string ToString() { return String.Join(" ", this); }
    }

    public class Table : List<TableLine>
    {
        public class TableException : Exception { public TableException(string msg) : base(msg) { } }
        public static void check(bool cond, string except) { if (!cond) throw new TableException(except); }
        public static int maxPrintLen = 10;

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
            if (aLine.columnName == null && columnName.Count == aLine.Count)
                return AddLine(aLine.ToArray());
            check(aLine.columnName.AllIn(columnName), "Can't add line to table");

            List<string> valList = new List<string>();
            string[] myColumns = columnName.ToArray();
            for (int i = 0; i < myColumns.Length; i++)
            {
                try { valList.Add(aLine[myColumns[i]]); }
                catch { valList.Add(""); }
            }
            return AddLine(valList.ToArray());
        }
        public Table AddLine(string[] aLineContent)
        {
            check(columnName.Count == aLineContent.Length, "Can't add line");
            Add(new TableLine(columnName).SetContent(aLineContent));
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
                    AddLine(conv.Decode(line.Trim()));
            }
            catch (Exception e) { Debug.Write("Error restoring:" + name); throw new TableException(e.ToString()); }
            return true;
        }
        public Table Where(Predicate<TableLine> lineIsValid)
        {
            Table result = new Table("result", columnName);
            try { result.AddRange(this.FindAll(lineIsValid)); }
            catch (Exception e) { throw new TableException(e.ToString()); }
            return result;
        }
        public Table Select(ColumnNames newTableColumn)
        {
            Table result = new Table("result", newTableColumn);
            for (int i = 0; i < Count; i++)
            {
                result.AddLine(this[i].Select(newTableColumn));
            }
            return result;
        }
        new public string ToString()
        {
            List<string[]> result = new List<string[]>();
            result.Add(columnName.ToArray());
            for (int i = 0; i < Count; i++)
                result.Add(this[i].ToArray());
            int colSz;
            for (int col = 0; col < result[0].Length; col++)
            {
                colSz = 0;
                for (int ln = 0; ln < result.Count; ln++)
                    if (result[ln][col].Length > colSz)
                        colSz = result[ln][col].Length;
                if (colSz > maxPrintLen)
                    colSz = maxPrintLen;
                for (int ln = 0; ln < result.Count; ln++)
                    result[ln][col] = result[ln][col].PadLeft(colSz).Substring(0, colSz);
            }
            string strResult = "";
            for (int ln = 0; ln < result.Count; ln++)
                strResult += "\n" + String.Join(" ", result[ln]);

            return name + ":" + strResult;
        }
    }
    public class Database : Dictionary<String, Table>
    {
        public class DatabaseException : Exception { public DatabaseException(string msg) : base(msg) { } }
        public static void check(bool cond, string except) { if (!cond) throw new DatabaseException(except); }
        public static string dbPrefix = "DB.";

        public string name;

        public static string[] GetKnownDBs()
        {
            return Directory.EnumerateDirectories(Directory.GetCurrentDirectory())
                .Where(d => Path.GetFileName(d).StartsWith(dbPrefix))
                .Select(d => Path.GetFileName(d).Substring(dbPrefix.Length)).ToArray();
        }
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
            Debug.Write("Adding tb:" + t.name);
            Add(t.name, t);
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
            Debug.Write("Restoring db:" + name);
            if (srcPath.Length == 0) srcPath = GetDirName();
            check(Directory.Exists(srcPath), "Invalid db load path path:" + srcPath);
            Clear();
            foreach (string tablePath in Directory.EnumerateFiles(srcPath))
            {
                try { Add(new Table(tablePath, conv)); }
                catch { Debug.Write("Can't add table from " + tablePath); }
            }
            return true;
        }
        new public bool Remove(string t)
        {
            check(Keys.Contains(t), "Table not in db.");
            return base.Remove(t);
        }
        new public Table this[string tName]
        {
            get
            {
                check(Keys.Contains(tName), "No table named " + tName);
                return base[tName];
            }
            set { Add(tName, value); }
        }
    }

    public class InterpretterException : Exception
    {
        public InterpretterException(string msg) : base(msg) { }
    }
    public class DbInterpretter
    {
        static void check(bool cond, string except) { if (!cond) throw new InterpretterException(except); }

        public static Database noDb = new Database("No DB");
        public static Database currentDb = noDb;

        public static bool isRunning = false;
        static Func<char, string> spacedWordIn = (x => $"(\\{x}[\\w ]+\\{x})");
        static Func<string, string> multi = (x => $"(?:{x}(?:_,_{x})*)".Replace("_", "\\s*"));
        static char strSep = '\'';
        static char compSep = '"';
        static string simpTerm = "(\\w+)";
        static string compTerm = spacedWordIn(compSep);
        static string strTerm = spacedWordIn(strSep);
        static string all = "(\\*)";
        static string term = $"(?:{simpTerm}|{compTerm})";
        static string lValue = term;
        static string logicOp = "(?:(==)|(!=)|( startswith )|( endswith ))";
        static string rValue = $"(?:{term}|{strTerm})";
        static string condition = $"(?:{lValue}_{logicOp}_{rValue})".Replace("_", "\\s*");
        static string condOp = "(( and )|( or ))";
        static string conditions = $"({condition}(_{condOp}_{condition})*)".Replace("_", "\\s*");
        static string assign = $"(?:{lValue}_=_{rValue})".Replace("_", "\\s*");

        static Predicate<TableLine> CombineCond(Predicate<TableLine> fCond, string oper, Predicate<TableLine> sCond)
        {
            switch (oper.Trim())
            {
                case "or": return x => fCond(x) || sCond(x);
                case "and": return x => fCond(x) && sCond(x);
            }
            return x => true;
        }
        static Predicate<TableLine> parseCondition(string strCond)
        {
            if (strCond == "") return (x => true);

            string[] aAssign = new Regex($"{lValue}|{rValue}|{logicOp}").Matches(strCond).Cast<Match>().Select(x => x.Value).ToArray();
            string
                lVal = aAssign[0],
                op = aAssign[1],
                rVal = aAssign[2];
            bool rValIsStr = (rVal[0] == strSep);
            rVal = rVal.Trim(strSep);
            switch (op)
            {
                case "==":
                    if (rValIsStr) return l => l[lVal] == (rVal);
                    return l => l[lVal] == (l[rVal]);
                case "!=":
                    if (rValIsStr) return l => l[lVal] != rVal;
                    return l => l[lVal] != l[rVal];
                case " startswith ":
                    if (rValIsStr) return l => l[lVal].StartsWith(rVal);
                    return l => l[lVal].StartsWith(l[rVal]);
                case " endswith ":
                    if (rValIsStr) return l => l[lVal].EndsWith(rVal);
                    return l => l[lVal].EndsWith(l[rVal]);
            }
            return (x => false);
        }
        static Predicate<TableLine> parseConditions(string strCond)
        {
            if (strCond == "") return (x => true);

            string[] myConditions = new Regex($"{condition}|{condOp}").Matches(strCond).Cast<Match>().Select(x => x.Value).ToArray();

            Predicate<TableLine> finalCond = parseCondition(myConditions[0]);
            for (int pos = 2; pos<myConditions.Length; pos+=2)
                finalCond = CombineCond(finalCond, myConditions[pos-1], parseCondition(myConditions[pos]));
            return finalCond;
        }

        public static string BYE_Command(string param)
        {
            if (param == "-h")
                return "bye                           |-> quit";
            isRunning = false;
            return "Bye!";
        }
        public static string HELP_Command(string param)
        {
            if (param == "-h")
                return "help                          |-> this help";
            string result = "";
            foreach (MethodInfo m in typeof(DbInterpretter).GetMethods()
                .Where(mi => mi.Name.EndsWith("_Command")))
                result += m.Invoke(null, new object[] { "-h" }) + "\n";
            return result;
        }
        public static string SAVE_Command(string param)
        {
            if (param == "-h")
                return "Save [path]                   |-> Saves the database to a provided or default path " +
                     "\nSave tableName [path] [CSV]   |-> Saves a table from current DB to a provided or default path";
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
        public static string RESTORE_Command(string param)
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
            else currentDb[tbName].Restore(Path.Combine(currentDb.GetDirName(), tbName));

            return "Table " + tbName + " restored.";
        }
        public static string USE_Command(string param)
        {
            if (param == "-h")
                return "Use DBname                    |-> Set the DBname database active (as empty)";
            check(param.Length == 0 || param.Split().Length > 0, "Invalid DBname");
            currentDb = new Database(param);

            return "Database " + currentDb.name + " set active as empty. you can try to restore.";
        }
        public static string DROP_Command(string param)
        {
            if (param == "-h")
                return "Drop tbName                   |-> Drop table from current db";
            check(currentDb != noDb, "Not allowed");
            currentDb.Remove(param);
            return "Table " + param + " removed";
        }
        public static string CREATE_Command(string param)
        {
            if (param == "-h")
                return "Create <table> (<field names>)                                    |-> Create a table on current db";

            check(currentDb != noDb, "Not allowed");
            string create = $"(?<table>{term})_\\(_(?<fields>{multi(term)})_\\)".Replace("_", "\\s*");

            Match matches = new Regex(create, RegexOptions.IgnoreCase).Match(param);
            check(matches.Value.Length == param.Length, "Syntax error");
            string
                tableName = matches.Groups["table"].Value,
                allFields = matches.Groups["fields"].Value;

            List<string> columns = new List<string>();
            foreach (Match m in new Regex($"{term}").Matches(allFields))
                columns.Add(m.Value.Trim(compSep));
            currentDb[tableName] = new Table(tableName, new ColumnNames(columns.ToArray()));

            return "Ok";
        }
        public static string INSERT_Command(string param)
        {
            if (param == "-h")
                return "Insert into <table name> (<fields list>) values (<values list>)   |-> Insert int table on current db";
            check(currentDb != noDb, "Not allowed");
            string insert = $"INTO _(?<table>{term})_\\(_(?<fields>{multi(lValue)})_\\)_VALUES_\\(_(?<values>{multi(rValue)})_\\)".Replace("_", "\\s*");

            Match matches = new Regex(insert, RegexOptions.IgnoreCase).Match(param);
            check(matches.Value.Length == param.Length, "Syntax error");
            string
                tableName = matches.Groups["table"].Value,
                fields = matches.Groups["fields"].Value,
                values = matches.Groups["values"].Value;

            List<string> fieldsList = new List<string>();
            foreach (Match m in new Regex($"{lValue}").Matches(fields))
                fieldsList.Add(m.Value.Trim(compSep));

            List<string> valuesList = new List<string>();
            foreach (Match m in new Regex($"{rValue}").Matches(values))
                valuesList.Add(m.Value.Trim(strSep));

            currentDb[tableName].AddLine(new TableLine(new ColumnNames(fieldsList.ToArray())).SetContent(valuesList.ToArray()));

            return "Ok";
        }
        public static string UPDATE_Command(string param)
        {
            if (param == "-h")
                return "Update <table name> set <field=value>+ where <condition>          |-> Update table entries on selected fields";
            check(currentDb != noDb, "Not allowed");
            string update = $"(?<table>{term})_SET_(?<assigns>{multi(assign)})_WHERE_(?<condition>{conditions})".Replace("_", "\\s+");

            Match matches = new Regex(update, RegexOptions.IgnoreCase).Match(param);
            check(matches.Value.Length == param.Length, "Syntax error");
            string
                tableName = matches.Groups["table"].Value,
                assigns = matches.Groups["assigns"].Value,
                cond = matches.Groups["condition"].Value;
            foreach (TableLine line in currentDb[tableName].Where(parseConditions(cond)))
                foreach (Match m in new Regex($"{assign}").Matches(assigns))
                {
                    string[] aAssign = new Regex($"{lValue}|{rValue}").Matches(m.Value).Cast<Match>().Select(x => x.Value).ToArray();
                    if (aAssign[1][0] == strSep) line[aAssign[0].Trim(compSep)] = aAssign[1].Trim(strSep);
                    else line[aAssign[0].Trim(compSep)] = line[aAssign[1].Trim(compSep)];
                }

            return "Ok";
        }
        public static string DELETE_Command(string param)
        {
            if (param == "-h")
                return "Delete from <table name> where <condition>                        |-> Delete table from current db";
            check(currentDb != noDb, "Not allowed");
            string delete = $"FROM_(?<table>{term})_WHERE_(?<condition>{conditions})".Replace("_", "\\s+");

            Match matches = new Regex(delete, RegexOptions.IgnoreCase).Match(param);
            check(matches.Value.Length == param.Length, "Syntax error");
            string
                tableName = matches.Groups["table"].Value,
                cond = matches.Groups["condition"].Value;
            currentDb[tableName].RemoveAll(parseConditions(cond));

            return "Ok";
        }
        public static string SELECT_Command(string param)
        {
            if (param == "-h")
                return "Select <*|fields list> from <table> [where <condition>]           |-> List tables from current db";
            check(currentDb != noDb, "Not allowed");

            string select = $"(?<selList>{all}|{multi(term)})_FROM_(?<table>{term})(?:_WHERE_(?<condition>{conditions}))?".Replace("_", "\\s+");

            Match matches = new Regex(select, RegexOptions.IgnoreCase).Match(param);
            check(matches.Value.Length == param.Length, "Syntax error");
            string
                allFields = matches.Groups["selList"].Value,
                tableName = matches.Groups["table"].Value,
                whereCond = matches.Groups["condition"].Value;

            if (tableName.Equals("TABLE_NAMES", StringComparison.OrdinalIgnoreCase))
                return String.Join("\n", currentDb.Keys);

            if (allFields == "*")
                return
                    currentDb[tableName]
                    .Where(parseConditions(whereCond))
                    .ToString();

            List<string> fields = new List<string>();
            foreach (Match m in new Regex($"{term}").Matches(allFields))
                fields.Add(m.Value.Trim('"', '\''));

            return
                currentDb[tableName]
                .Select(new ColumnNames(fields.ToArray()))
                .Where(parseConditions(whereCond))
                .ToString();
        }

        public static string Execute(string textCommand)
        {
            textCommand = textCommand.Trim();
            string command = "", parameters = "";
            int fSpacePos = (textCommand + " ").IndexOf(' ');
            command = textCommand.Substring(0, fSpacePos).Trim().ToUpper();
            parameters = textCommand.Substring(fSpacePos).Trim();
            try
            {
                MethodInfo function = typeof(DbInterpretter).GetMethod(command + "_Command");
                check(function != null, "Invalid command");
                try { return (string)function.Invoke(null, new object[] { parameters }); }
                catch (Exception e) { throw e.InnerException; }
            }
            catch (Database.DatabaseException e) { return ("Database error: " + e.Message); }
            catch (InterpretterException e) { return ("Console error: " + e.Message); }
            catch { return ("Unknown exception: " + command); }
        }
    }

    public class DB_ConsoleInterface
    {
        public static void Run(string[] init)
        {
            Console.Out.WriteLine("Known databases:\n" + String.Join("\n", Database.GetKnownDBs()));
            foreach (string comm in init) Console.Out.WriteLine(DbInterpretter.Execute(comm));
            while (DbInterpretter.isRunning)
            {
                Console.Out.Write(DbInterpretter.currentDb.name + ">");
                Console.Out.WriteLine(DbInterpretter.Execute(Console.In.ReadLine()));
            }
        }
    }
    public class DB_WindowInterface
    {
        public static void Run(string[] init)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new DbInterface(init));
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            DbInterpretter.isRunning = true;
            string[] init = {
                "use testDB",
                "restore",
                "select * from table1",
                };
            DB_ConsoleInterface.Run(init);
            //DB_WindowInterface.Run(init);
        }
    }
}
