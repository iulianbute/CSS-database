﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace database
{
    public class Debug
    {
        public static bool on = true;
        public static void Write(string msg)
        {
            Trace.Assert(msg != null, "MSG == null");
            MethodBase method = new StackFrame(1).GetMethod();
            var type = method.DeclaringType.ToString();
            var name = method.Name;
            Contract.Ensures(type != null, "Debug.Write, type == null");
            Contract.Ensures(name != null, "Debug.Write, name == null");
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
            Contract.Requires(line != null, "line == null");
            string result = "";
            Contract.Invariant(line.Length >= 0);
            foreach (string e in line)
                result += e.Length.ToString() + separator + e + separator;
            Contract.Ensures(result != "", "DefaultFormatConverter.Encode, result == \"\"");
            return result;
        }
        public string[] Decode(string line)
        {
            Contract.Requires(line != null, "line == null");
            List<string> result = new List<string>();
            int len = 0, sepPos = 0;
            Contract.Invariant(line.Length > 0);
            while (line.Length > 0)
                try
                {
                    sepPos = line.IndexOf(separator);
                    len = Int32.Parse(line.Substring(0, sepPos));
                    result.Add(line.Substring(sepPos + 1, len));
                    line = line.Substring(sepPos + 1 + len + 1);
                }
                catch { return null; }
            Contract.Ensures(result != null, "DefaultFormatConverter.Encode, result == null");
            Contract.Ensures(result.Count != 0, "DefaultFormatConverter.Encode, result.Count == 0");
            return result.ToArray();
        }
    }
    public class CSVformatConverter : IFormatConverter
    {
        static char separator = ';';
        public string Encode(string[] line)
        {
            Trace.Assert(line != null, "line == null");
            string toReturn = String.Join(separator.ToString(), line);
            Contract.Ensures(toReturn != null, "CSVformatConverter.Encode, toReturn == null");
            return toReturn;
        }
        public string[] Decode(string line)
        {
            Trace.Assert(line != null, "line == null");
            string[] toReturn = line.Split(separator);
            Contract.Ensures(toReturn != null, "CSVformatConverter.Decode, toReturn == null");
            return toReturn;
        }
    }

    public class ColumnNames : Dictionary<string, int>
    {
        public ColumnNames(string[] names) { Contract.Requires(names != null, "ColumnNames names == null"); for (int i = 0; i < names.Length; i++) Add(names[i], i); Contract.Ensures(this.Count > 0, "ColumnNames this.Count == 0"); }
        public string[] ToArray()
        {
            Contract.Requires(Keys != null, "ColumnNames.ToArray Keys == null");
            Contract.Requires(Keys.Count != 0, "ColumnNames.ToArray Keys.Count == 0");
            List<string> result = new List<string>();
            for (int i = 0; i++ < Keys.Count; result.Add("")) Contract.Invariant(i >= 0) ;
            Contract.Invariant(Keys.Count >= 0);
            foreach (string k in Keys)
            {
                Contract.Invariant(k != null);
                result[this[k]] = k;
            }
            Contract.Ensures(result != null, "ColumnNames.ToArray result == null");
            Contract.Ensures(result.Count != 0, "ColumnNames.ToArray result.Count == 0");
            return result.ToArray();
        }
        public bool Equals(ColumnNames oth)
        {
            Trace.Assert(oth != null, "Column Names == null");
            foreach (bool eq in ToArray().Zip(oth.ToArray(), (f, s) => (f == s)))
                if (!eq) return false;
            return true;
        }
        public bool AllIn(ColumnNames oth)
        {
            Trace.Assert(oth != null, "Column Names == null");
            Contract.Invariant(Keys.Count >= 0);
            foreach (string k in Keys)
            {
                Contract.Invariant(k != null);
                if (!oth.Keys.Contains(k))
                    return false;
            }
            return true;
        }
        new public string ToString() { string toReturnString = String.Join(" ", ToArray()); Contract.Ensures(toReturnString != String.Empty, "ColumnNames.ToString toReturnString is empty"); return toReturnString; }
    }
    public class TableLine : List<string>
    {
        public class TableLineException : Exception { public TableLineException(string msg) : base(msg) { } }
        public static void check(bool cond, string except) { Contract.Requires(except != null, "TableLine.check, except == null"); Contract.Requires(except != "", "TableLine.check, except == \"\""); if (!cond) throw new TableLineException(except); }

        public ColumnNames columnName;

        public TableLine(ColumnNames names) { Contract.Requires(names != null, "TableLine.TableLine names == null"); columnName = names; Contract.Ensures(columnName != null, "TableLine.TableLine names == null"); }
        public TableLine SetContent(string[] content)
        {
            Contract.Requires(content != null, "TableLine.SetContent content == null");
            Contract.Requires(content.Length != columnName.Count, "TableLine.SetContent content.Length == columnName.Count");
            check(columnName == null || content.Length == columnName.Count, "Can't set content to line");
            Clear();
            AddRange(content);
            Contract.Ensures(this.Count > 0, "TableLine this.Count < 0");
            return this;
        }
        public string this[string aFieldName]
        {
            get
            {
                Contract.Requires(columnName == null, "TableLine.this.get columnName == null");
                Contract.Requires(columnName.Keys.Contains(aFieldName) != false, "TableLine.this.get columnName.Keys.Contains(aFieldName) is false");
                check(columnName != null, "No column names are set yet");
                check(columnName.Keys.Contains(aFieldName), "Invalid column name: " + aFieldName);
                return this[this.columnName[aFieldName]];
            }
            set
            {
                Contract.Requires(columnName == null, "TableLine.this.set columnName == null");
                check(columnName != null, "No column names are set yet");
                this[this.columnName[aFieldName]] = value;
                Contract.Ensures(this[this.columnName[aFieldName]] == value, "TableLine.this.get this[this.columnName[aFieldName]] != value");
            }
        }
        public TableLine Select(ColumnNames selColNames)
        {
            Trace.Assert(selColNames != null, "Column Names == null");
            List<string> selContent = new List<string>();
            foreach (string colName in selColNames.ToArray())
            {
                Contract.Invariant(colName != null);
                selContent.Add(this[colName]);
            }
            Contract.Ensures(selContent != null, "TableLine.Select selContent == null");
            Contract.Ensures(selContent.Count > 0, "TableLine.Select selContent.Count == 0");
            return new TableLine(selColNames).SetContent(selContent.ToArray());
        }
        new public string ToString() { Contract.Ensures(this != null); string toReturnString = String.Join(" ", this); Contract.Ensures(toReturnString != String.Empty, "TableLine.toString toReturnString is null"); return toReturnString; }
    }

    public class Table : List<TableLine>
    {
        public class TableException : Exception { public TableException(string msg) : base(msg) { } }
        public static void check(bool cond, string except) { Contract.Requires(except != null);  Contract.Requires(except != ""); if (!cond) throw new TableException(except); }
        public static int maxPrintLen = 30;

        public string name;
        public ColumnNames columnName;

        public Table(string aName, ColumnNames someColumnNames)
        {
            Trace.Assert(aName != null, "tableName == null");
            Trace.Assert(someColumnNames != null, "Column Names == null");
            name = aName;
            columnName = someColumnNames;
            Contract.Ensures(name != null);
            Contract.Ensures(columnName != null);
        }
        public Table(string filePath, IFormatConverter conv)
        {
            //Trace.Assert(conv != null, "Converter == null");
            Trace.Assert(filePath != null, "tableName == null");
            name = Path.GetFileName(filePath);
            Contract.Ensures(name != null);
            Restore(filePath, conv);
        }
        public Table AddLine(TableLine aLine)
        {
            Contract.Requires(aLine != null);
            if (aLine.columnName == null && columnName.Count == aLine.Count)
                return AddLine(aLine.ToArray());
            check(aLine.columnName.AllIn(columnName), "Can't add line to table");

            List<string> valList = new List<string>();
            string[] myColumns = columnName.ToArray();
            Contract.Invariant(myColumns.Length >= 0);
            for (int i = 0; i < myColumns.Length; i++)
            {
                Contract.Invariant(i >= 0);
                try { valList.Add(aLine[myColumns[i]]); }
                catch { valList.Add(""); }
            }
            Contract.Ensures(valList != null);

            Table responseTable = AddLine(valList.ToArray());
            Contract.Ensures(responseTable != null);
            return responseTable;
        }
        public Table AddLine(string[] aLineContent)
        {
            Trace.Assert(aLineContent != null, "aLineContent == null");
            check(columnName.Count == aLineContent.Length, "Can't add line");
            Contract.Requires(columnName != null);
            TableLine customTableLine = new TableLine(columnName).SetContent(aLineContent);
            Add(customTableLine);
            Contract.Ensures(this.Contains(customTableLine) != false);
            return this;
        }
        public bool Save(string dirPath, IFormatConverter conv = null)
        {
            Trace.Assert(dirPath != null, "dirPath == null");
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
            Trace.Assert(filePath != null, "dirPath == null");
            string line;
            try
            {
                if (conv == null) conv = new DefaultFormatConverter();
                StreamReader file = new StreamReader(filePath);
                Clear();
                columnName = new ColumnNames(conv.Decode(file.ReadLine()));
                while ((line = file.ReadLine()) != null)
                {
                    Contract.Invariant(line != null);
                    AddLine(conv.Decode(line.Trim()));
                }
            }
            catch (Exception e) { Debug.Write("Error restoring:" + name); throw new TableException(e.ToString()); }
            return true;
        }
        public Table Where(Predicate<TableLine> lineIsValid)
        {
            Contract.Requires(lineIsValid != null);
            Table result = new Table("result", columnName);
            try { result.AddRange(this.FindAll(lineIsValid)); }
            catch (Exception e) { throw new TableException(e.ToString()); }
            Contract.Ensures(result != null);
            Contract.Ensures(result.Count > 0);
            return result;
        }
        public Table Select(ColumnNames newTableColumn)
        {
            Trace.Assert(newTableColumn != null, "Column Names == null");
            Table result = new Table("result", newTableColumn);
            for (int i = 0; i < Count; i++)
            {
                Contract.Invariant(i > 0);
                result.AddLine(this[i].Select(newTableColumn));
            }
            Contract.Ensures(result != null);
            Contract.Ensures(result.Count > 0);
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
            Contract.Ensures(strResult != "");
            Contract.Ensures(name != "");
            return name + ":" + strResult;
        }
    }
    public class Database : Dictionary<String, Table>
    {
        public class DatabaseException : Exception { public DatabaseException(string msg) : base(msg) { } }
        public static void check(bool cond, string except) { Contract.Requires(except != null); Contract.Requires(except != "");  if (!cond) throw new DatabaseException(except); }
        public static string dbPrefix = "DB.";

        public string name;

        public static string[] GetKnownDBs()
        {
            return Directory.EnumerateDirectories(Directory.GetCurrentDirectory())
                .Where(d => Path.GetFileName(d).StartsWith(dbPrefix))
                .Select(d => Path.GetFileName(d).Substring(dbPrefix.Length)).ToArray();
        }
        public string GetDirName() { Contract.Requires(dbPrefix != ""); Contract.Requires(name != "");  return dbPrefix + name; }
        public Database(string dbName)
        {
            Trace.Assert(dbName != null, "dbName == null");
            name = dbName;
            Contract.Ensures(name != null);
        }
        public Database Add(Table t)
        {
            Trace.Assert(t != null, "table t == null");
            Debug.Write("Adding tb:" + t.name);
            Add(t.name, t);
            Contract.Ensures(this.Count > 0);
            Contract.Ensures(this.ContainsKey(t.name));
            return this;
        }

        public bool Save(string destDir = "", IFormatConverter conv = null)
        {
            Trace.Assert(destDir != null, "destDir == null");
            if (destDir.Length == 0) destDir = GetDirName();
            try
            {
                Directory.Delete(destDir);
            }
            catch { }
            try
            {
                Directory.CreateDirectory(destDir);
                foreach (Table table in Values)
                    table.Save(destDir, conv);
            }
            catch { throw new DatabaseException("Save to " + destDir + " failed!"); }
            return true;
        }
        public bool Restore(string srcPath = "", IFormatConverter conv = null)
        {
            Trace.Assert(srcPath != null, "srcPath == null");
            Debug.Write("Restoring db:" + name);
            if (srcPath.Length == 0) srcPath = GetDirName();
            check(Directory.Exists(srcPath), "Invalid db load path path:" + srcPath);
            Clear();
            foreach (string tablePath in Directory.EnumerateFiles(srcPath))
            {
                Contract.Invariant(tablePath != null);
                Contract.Requires(tablePath != null);
                Contract.Requires(tablePath != "");
                try { Add(new Table(tablePath, conv)); }
                catch { Debug.Write("Can't add table from " + tablePath); }
            }
            Contract.Ensures(this != null);
            Contract.Ensures(this.Count > 0);
            return true;
        }
        new public bool Remove(string t)
        {
            Contract.Requires(t != "");
            Contract.Requires(t != null);
            check(Keys.Contains(t), "Table not in db.");
            bool response = base.Remove(t);
            Contract.Ensures(base.ContainsKey(t) == false);
            return response;

        }
        new public Table this[string tName]
        {
            get
            {
                Contract.Requires(tName != null);
                Contract.Requires(tName != "");
                check(Keys.Contains(tName), "No table named " + tName);
                Table toReturn = base[tName];
                Contract.Ensures(toReturn != null);
                return toReturn;
            }
            set { Contract.Requires(tName != null); Contract.Requires(tName != ""); Contract.Requires(value != null); Add(tName, value);  Contract.Ensures(this.ContainsKey(tName)); }
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
            Contract.Requires(fCond != null);
            Contract.Requires(oper != null);
            Contract.Requires(sCond != null);
            switch (oper.Trim())
            {
                case "or": return x => fCond(x) || sCond(x);
                case "and": return x => fCond(x) && sCond(x);
            }
            return x => true;
        }
        static Predicate<TableLine> parseCondition(string strCond)
        {
            Trace.Assert(strCond != null, "strCond == null");
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
            Trace.Assert(strCond != null, "strCond == null");
            if (strCond == "") return (x => true);

            string[] myConditions = new Regex($"{condition}|{condOp}").Matches(strCond).Cast<Match>().Select(x => x.Value).ToArray();

            Predicate<TableLine> finalCond = parseCondition(myConditions[0]);
            for (int pos = 2; pos < myConditions.Length; pos += 2)
            {
                Contract.Invariant(pos > 1);
                finalCond = CombineCond(finalCond, myConditions[pos - 1], parseCondition(myConditions[pos]));
            }
            Contract.Ensures(finalCond != null);
            return finalCond;
        }

        public static string BYE_Command(string param)
        {
            Trace.Assert(param != null, "param == null");
            if (param == "-h")
                return "bye                           |-> quit";
            isRunning = false;
            return "Bye!";
        }
        public static string HELP_Command(string param)
        {
            Trace.Assert(param != null, "param == null");
            if (param == "-h")
                return "help                          |-> this help";
            string result = "";
            foreach (MethodInfo m in typeof(DbInterpretter).GetMethods()
                .Where(mi => mi.Name.EndsWith("_Command")))
                result += m.Invoke(null, new object[] { "-h" }) + "\n";
            Contract.Ensures(result != null);
            Contract.Ensures(result != "");
            return result;
        }
        public static string SAVE_Command(string param)
        {
            Trace.Assert(param != null, "param == null");
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
                Contract.Requires(conv != null);
                currentDb.Save(param, conv);
                Contract.Ensures(currentDb.name != null);
                return "Database " + currentDb.name + " saved.";
            }
            int fSpacePos = param.IndexOf(' ');
            if (fSpacePos == -1) fSpacePos = param.Length;
            string tbName = param.Substring(0, fSpacePos).Trim();
            string dstPath = param.Substring(fSpacePos).Trim();
            if (dstPath.Length > 0) currentDb[tbName].Save(dstPath, conv);
            else currentDb[tbName].Save(currentDb.GetDirName());
            Contract.Requires(tbName != null);
            return "Table " + tbName + " saved.";
        }
        public static string RESTORE_Command(string param)
        {
            Trace.Assert(param != null, "param == null");
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
                Contract.Ensures(conv != null);
                currentDb.Restore(param, conv);
                Contract.Requires(currentDb.name != null);
                return "Database " + currentDb.name + " restored.";
            }
            int fSpacePos = param.IndexOf(' ');
            if (fSpacePos == -1) fSpacePos = param.Length;
            string tbName = param.Substring(0, fSpacePos).Trim();
            string dstPath = param.Substring(fSpacePos).Trim();
            if (dstPath.Length > 0) currentDb[tbName].Restore(dstPath, conv);
            else currentDb[tbName].Restore(Path.Combine(currentDb.GetDirName(), tbName));
            Contract.Requires(tbName != null);
            return "Table " + tbName + " restored.";
        }
        public static string USE_Command(string param)
        {
            Trace.Assert(param != null, "param == null");
            if (param == "-h")
                return "Use DBname                    |-> Set the DBname database active (as empty)";
            check(param.Length == 0 || param.Split().Length > 0, "Invalid DBname");
            currentDb = new Database(param);
            Contract.Requires(currentDb.name != null);
            return "Database " + currentDb.name + " set active as empty. you can try to restore.";
        }
        public static string DROP_Command(string param)
        {
            Trace.Assert(param != null, "param == null");
            if (param == "-h")
                return "Drop tbName                   |-> Drop table from current db";
            check(currentDb != noDb, "Not allowed");
            currentDb.Remove(param);
            Contract.Requires(param != null);
            Contract.Ensures(currentDb.ContainsKey(param) == false);
            return "Table " + param + " removed";
        }
        public static string CREATE_Command(string param)
        {
            Trace.Assert(param != null, "param == null");
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
            Contract.Requires(tableName != null);
            Contract.Requires(columns != null);
            Table ct = new Table(tableName, new ColumnNames(columns.ToArray()));
            currentDb[tableName] = ct;
            Contract.Ensures(currentDb[tableName] == ct);
            return "Ok";
        }
        public static string INSERT_Command(string param)
        {
            Trace.Assert(param != null, "param == null");
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

            Contract.Requires(valuesList != null);
            Contract.Requires(fieldsList != null);
            TableLine ctl = new TableLine(new ColumnNames(fieldsList.ToArray())).SetContent(valuesList.ToArray());
            currentDb[tableName].AddLine(ctl);
            Contract.Ensures(currentDb[tableName].Contains(ctl));
            return "Ok";
        }
        public static string UPDATE_Command(string param)
        {
            Trace.Assert(param != null, "param == null");
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
                    Contract.Invariant(line != null);
                    string[] aAssign = new Regex($"{lValue}|{rValue}").Matches(m.Value).Cast<Match>().Select(x => x.Value).ToArray();
                    if (aAssign[1][0] == strSep) line[aAssign[0].Trim(compSep)] = aAssign[1].Trim(strSep);
                    else line[aAssign[0].Trim(compSep)] = line[aAssign[1].Trim(compSep)];
                }

            return "Ok";
        }
        public static string DELETE_Command(string param)
        {
            Trace.Assert(param != null, "param == null");
            if (param == "-h")
                return "Delete from <table name> where <condition>                        |-> Delete table from current db";
            check(currentDb != noDb, "Not allowed");
            string delete = $"FROM_(?<table>{term})_WHERE_(?<condition>{conditions})".Replace("_", "\\s+");

            Match matches = new Regex(delete, RegexOptions.IgnoreCase).Match(param);
            check(matches.Value.Length == param.Length, "Syntax error");
            string
                tableName = matches.Groups["table"].Value,
                cond = matches.Groups["condition"].Value;
            Predicate<TableLine> pcond = parseConditions(cond);
            Contract.Requires(pcond != null);
            currentDb[tableName].RemoveAll(pcond);
            Contract.Ensures(currentDb[tableName].FindAll(pcond).Count == 0);

            return "Ok";
        }
        public static string SELECT_Command(string param)
        {
            Trace.Assert(param != null, "param == null");
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
            {
                String response = String.Join("\n", currentDb.Keys);
                Contract.Ensures(response != null);
                return response;
            }

            if (allFields == "*")
            {
                string resultS = currentDb[tableName]
                    .Where(parseConditions(whereCond))
                    .ToString();
                Contract.Ensures(resultS != null);
                return resultS;
             
            }

            List<string> fields = new List<string>();
            foreach (Match m in new Regex($"{term}").Matches(allFields))
                fields.Add(m.Value.Trim('"', '\''));

            string result = currentDb[tableName]
                .Select(new ColumnNames(fields.ToArray()))
                .Where(parseConditions(whereCond))
                .ToString();
            Contract.Ensures(result != null);
            return result;
                
        }

        public static string Execute(string textCommand)
        {
            Trace.Assert(textCommand != null, "textCommand == null");
            textCommand = textCommand.Trim();
            string command = "", parameters = "";
            int fSpacePos = (textCommand + " ").IndexOf(' ');
            command = textCommand.Substring(0, fSpacePos).Trim().ToUpper();
            parameters = textCommand.Substring(fSpacePos).Trim();
            try
            {
                Contract.Requires(command != null);
                Contract.Requires(parameters != null);
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

    [ExcludeFromCodeCoverage]
    public class DB_ConsoleInterface
    {
        public static void Run(string[] init)
        {
            Trace.Assert(init != null, "init == null");
            Console.Out.WriteLine("Known databases:\n" + String.Join("\n", Database.GetKnownDBs()));
            foreach (string comm in init) Console.Out.WriteLine(DbInterpretter.Execute(comm));
            while (DbInterpretter.isRunning)
            {
                Console.Out.Write(DbInterpretter.currentDb.name + ">");
                Console.Out.WriteLine(DbInterpretter.Execute(Console.In.ReadLine()));
            }
        }
    }
    [ExcludeFromCodeCoverage]
    public class DB_WindowInterface
    {
        public static void Run(string[] init)
        {
            Trace.Assert(init != null, "init == null");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new DbInterface(init));
        }
    }

    class Program
    {
        public static void Main(string[] args)
        {
            DbInterpretter.isRunning = true;
            string[] init = {
                "use testDB",
                "restore",
                "select * from table1",
                "bye",
                };
            DB_ConsoleInterface.Run(init);
            //DB_WindowInterface.Run(init);
        }
    }
}
