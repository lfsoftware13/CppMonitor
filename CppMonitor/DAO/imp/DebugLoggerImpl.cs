using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Data.SQLite;

using NanjingUniversity.CppMonitor.Common;


namespace NanjingUniversity.CppMonitor.DAO.imp
{
    class DebugLoggerImpl : ILoggerDao
    {

        public DebugLoggerImpl()
        {
            handlers = new Dictionary<string, LogHandler>();
        }

        public bool LogInfo(string target, List<KeyValuePair<String, Object>> list)
        {
            if (!handlers.Keys.Contains(target)) 
            {
                return defaultLogHandler(target, list);
            }
            else
            {
                var log = handlers[target];
                return log(target, list);
            }
        }

        public int returnKeyAfterLogInfo(string target, List<KeyValuePair<String, Object>> list)
        {
            LogInfo(target, list);
            var db = DBHelper.getInstance();
            var conn = db.getConnection();

            var sql = "select last_insert_rowid()";
            var cmd = new SQLiteCommand(sql, conn);
            var result = cmd.ExecuteReader();
            result.Read();
            int id = int.Parse(result[0].ToString());

            db.returnConnection();

            return id;
        }

        public void clearLog()
        {
            DBHelper dbHelper = DBHelper.getInstance();
            SQLiteConnection conn = dbHelper.getConnection();
            foreach (string tableName in tableNames)
            {
                string sql = "delete from " + tableName + ";";
                string sql2 = "update sqlite_sequence SET seq = 0 where name ='" + tableName + "';";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.ExecuteNonQuery();
                SQLiteCommand command2 = new SQLiteCommand(sql2, conn);
                command2.ExecuteNonQuery();
            }
            dbHelper.returnConnection();
        }

        public int GetBreakpointId(string tag)
        {
            var db = DBHelper.getInstance();
            var conn = db.getConnection();

            string sql = "select * from breakpoint where (tag = '" + tag + "')";
            var cmd = new SQLiteCommand(sql, conn);
            var result = cmd.ExecuteReader();

            if (!result.Read()) throw new NoSuchBreakpointException("No such breakpoint: " + tag);
            int id = int.Parse(result["id"].ToString());

            result.Close();
            db.returnConnection();

            return id;
        }

        public int CreateNewDebugEvent(List<KeyValuePair<string, object>> vals)
        {
            string debug_target = "";
            foreach (var pair in vals)
            {
                if (pair.Key.Equals("debug_type"))
                {
                    debug_target = pair.Value.ToString();
                }
            }
            List<KeyValuePair<string, object>> list = new List<KeyValuePair<string, object>>();
            list.Add(new KeyValuePair<string, object>("timestamp", DateTime.Now));
            list.Add(new KeyValuePair<string, object>("debug_target", debug_target));
            return returnKeyAfterLogInfo(debug_target, list);
        }

        public void ensureTableExist()
        {
            
            List<string> ddls = new List<string>();
            ddls.Add("CREATE TABLE IF NOT EXISTS debug_info ( id INTEGER PRIMARY KEY,time char[22], timestamp DATETIME DEFAULT current_time NOT NULL, debug_target TEXT NOT NULL )");
            ddls.Add("CREATE TABLE IF NOT EXISTS debug_break ( id INTEGER PRIMARY KEY,time char[22], break_reason TEXT NOT NULL, breakpoint_id INTEGER,breakpoint_last_hit INTEGER ,debug_target TEXT NOT NULL);");
            ddls.Add("CREATE TABLE IF NOT EXISTS breakpoint ( id INTEGER PRIMARY KEY, tag TEXT, condition TEXT, condition_type TEXT, " + 
                "current_hits INT DEFAULT 0, file TEXT NOT NULL, file_column INT NOT NULL, file_line INT NOT NULL, function_name TEXT, location_type TEXT NOT NULL );");
            ddls.Add("CREATE TABLE IF NOT EXISTS debug_run ( id INTEGER PRIMARY KEY,time char[22], run_type TEXT NOT NULL, breakpoint_last_hit INTEGER ,debug_target TEXT);");
            ddls.Add("CREATE TABLE IF NOT EXISTS exception ( id INTEGER PRIMARY KEY, type TEXT, name TEXT, descrption TEXT, code INT, action TEXT NOT NULL);");
            ddls.Add("CREATE TABLE IF NOT EXISTS debug_exception_thrown ( id INTEGER PRIMARY KEY,time char[22], exception_id INTEGER NOT NULL,debug_target TEXT NOT NULL );");
            ddls.Add("CREATE TABLE IF NOT EXISTS debug_exception_not_handled ( id INTEGER PRIMARY KEY,time char[22], exception_id INTEGER NOT NULL,debug_target TEXT NOT NULL );");

            SQLiteConnection conn = new SQLiteConnection("Data Source=" + AddressCommon.DBFilePath);
            conn.Open();
            foreach (string sql in ddls)
            {
                var cmd = new SQLiteCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            conn.Close();
        }

        private bool defaultLogHandler(string tableName, List<KeyValuePair<string, object>> list)
        {
            var dbHelper = DBHelper.getInstance();
            try
            {
                
                string keys = "", vals = "";
                foreach (var pair in list)
                {
                    if (!keys.Equals("")) 
                    {
                        keys += ", ";
                        vals += ", ";
                    }
                    keys += pair.Key;
                    vals += "@" + pair.Key;
                }
                if(tableName.StartsWith("debug")){
                    keys += ",time";
                    vals += ", @time";
                }
               
                SQLiteConnection conn = dbHelper.getConnection();
                string sql = String.Format("insert into {0} ({1}) values ({2})", tableName, keys, vals);
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                if (tableName.StartsWith("debug"))
                {
                    cmd.Parameters.Add(new SQLiteParameter("@time", DateTime.Now.ToString()));
                }
                foreach (var pair in list)
                {
                    cmd.Parameters.Add(new SQLiteParameter("@" + pair.Key, pair.Value.ToString()));
                }
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Debug.Print(e.StackTrace);
                return false;
            }
            finally
            {
                dbHelper.returnConnection();
            }
            return true;
        }

        private bool logDebugRun(List<KeyValuePair<string, object>> vals)
        {
            int id = CreateNewDebugEvent(vals);
            vals.Add(new KeyValuePair<string, object>("id", id));
            return defaultLogHandler("debug_run", vals);
        }

        private bool logDebugBreak(List<KeyValuePair<string, object>> vals)
        {
            int id = CreateNewDebugEvent(vals);
            vals.Add(new KeyValuePair<string, object>("id", id));
            return defaultLogHandler("debug_break", vals);
        }

        private bool logDebugExceptionThrown(List<KeyValuePair<string, object>> vals)
        {
            int id = CreateNewDebugEvent(vals);
            vals.Add(new KeyValuePair<string, object>("id", id));
            return defaultLogHandler("debug_exception_thrown", vals);
        }

        private bool logDebugExceptionNotHandled(List<KeyValuePair<string, object>> vals)
        {
            int id = CreateNewDebugEvent(vals);
            vals.Add(new KeyValuePair<string, object>("id", id));
            return defaultLogHandler("debug_exception_not_handled", vals);
        }

        private bool logException(List<KeyValuePair<string, object>> vals)
        {
            return defaultLogHandler("exception", vals);
        }

        private bool logBreakpoint(List<KeyValuePair<string, object>> vals)
        {
            return defaultLogHandler("breakpoint", vals);
        }

        private string[] tableNames = new string[] {
            "debug_info", "debug_break", "breakpoint", "debug_run", "exception", "debug_exception_thrown", "debug_exception_not_handled",
        };

        private Dictionary<string, LogHandler> handlers;

        public delegate bool LogHandler(string tableName, List<KeyValuePair<string, object>> vals); 
    }

    public class NoSuchBreakpointException : Exception 
    {
        public NoSuchBreakpointException() : base() { }
        public NoSuchBreakpointException(string message) : base(message) { }
    }
}
