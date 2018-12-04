using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Data.SQLite;

using NanjingUniversity.CppMonitor.Util.Common;


namespace NanjingUniversity.CppMonitor.DAO.imp
{
    class DebugLoggerImpl : ILoggerDao
    {

        public DebugLoggerImpl()
        {
            handlers = new Dictionary<string, LogHandler>();
            handlers["breakpoint"] = logBreakpoint;
            handlers["exception"] = logException;
            handlers["local_varialble"] = logLocalVarialbles;
            handlers["breakpoint_event"] = logBreakpointEvent;
            handlers["debug_info"] = logDebugInfo;

            tableNamesWithIdentiedId = new List<string>()
            {
                 "debug_info","breakpoint_event",
            };

            string[] tableNames = new string[] {
                "debug_info", "breakpoint", "exception", "breakpoint_event", "local_variable"
            };
            tableNameList = tableNames;
        }

        public override bool LogInfo(string target, List<KeyValuePair<String, Object>> list)
        {
            if (handlers.Keys.Contains(target)) 
            {
                var log = handlers[target];
                return log(list);
            }
            return false;
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

        /// <summary>
        /// 在基表中新建一个 debug event 项目，并且返回这个项目的 id.
        /// </summary>
        /// <param name="vals">属性列表</param>
        /// <returns>新建项的 id</returns>
        public int CreateNewDebugEvent(string type, List<KeyValuePair<string, object>> vals)
        {
            string debug_target = "";
            string configName = "";
            for (int i = 0; i < vals.Count; ++i)
            {
                var pair = vals[i];
                if (pair.Key.Equals("debug_target"))
                {
                    debug_target = pair.Value + "";
                    vals.Remove(pair);
                    --i;
                    continue;
                }
                if (pair.Key.Equals("config_name"))
                {
                    configName = pair.Value + "";
                    vals.Remove(pair);
                    --i;
                    continue;
                } 
            }
            List<KeyValuePair<string, object>> list = new List<KeyValuePair<string, object>>();
            list.Add(new KeyValuePair<string, object>("timestamp", DateTime.Now.ToString()));
            list.Add(new KeyValuePair<string, object>("debug_target", debug_target));
            list.Add(new KeyValuePair<string, object>("type", type));
            list.Add(new KeyValuePair<string, object>("config_name", configName));
            return returnKeyAfterLogInfo("debug_info", list);
        }

        public override void ensureTableExist()
        {
            
            List<string> ddls = new List<string>();
            ddls.Add("CREATE TABLE IF NOT EXISTS debug_info (id INTEGER PRIMARY KEY,type TEXT NOT NULL, exception_id INTEGER, timestamp TEXT NOT NULL DEFAULT current_time, break_reason TEXT,breakpoint_current_hit integer, debug_target TEXT, config_name TEXT);");
            ddls.Add("CREATE TABLE IF NOT EXISTS breakpoint ( id INTEGER PRIMARY KEY, tag TEXT, condition TEXT, condition_type TEXT, current_hits INT DEFAULT 0, file TEXT NOT NULL, file_column INT NOT NULL, file_line INT NOT NULL, function_name TEXT, location_type TEXT NOT NULL , enabled TEXT DEFAULT true NOT NULL);");
            ddls.Add("CREATE TABLE IF NOT EXISTS exception ( id INTEGER PRIMARY KEY, type TEXT, name TEXT, description TEXT, code INT, action TEXT NOT NULL);");
            ddls.Add("CREATE TABLE IF NOT EXISTS local_variable ( id INTEGER PRIMARY KEY, debug_id INTEGER NOT NULL, name TEXT NOT NULL, value TEXT NOT NULL);");
            ddls.Add("CREATE TABLE IF NOT EXISTS breakpoint_event ( id INTEGER PRIMARY KEY, modification TEXT NOT NULL, breakpoint_id INTEGER, old_breakpoint_id INTEGER);");
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
                    if (pair.Key.Equals("id") && !tableNamesWithIdentiedId.Contains(tableName))
                    {
                        continue;
                    }

                    if (!keys.Equals("")) 
                    {
                        keys += ", ";
                        vals += ", ";
                    }
                    keys += pair.Key;
                    vals += "@" + pair.Key;
                }
               
                SQLiteConnection conn = dbHelper.getConnection();
                string sql = String.Format("insert into {0} ({1}) values ({2})", tableName, keys, vals);
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);

                foreach (var pair in list)
                {
                    cmd.Parameters.Add(new SQLiteParameter("@" + pair.Key, pair.Value + ""));
                }
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                dbHelper.returnConnection();
            }
            return true;
        }

        private bool logDebugInfo(List<KeyValuePair<string, object>> list)
        {
            bool containsTime = false;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Key.Equals("timestamp"))
                {
                    containsTime = true;
                    break;
                }
            }
            if (!containsTime)
            {
                list.Add(new KeyValuePair<string, object>("timestamp", DateTime.Now.ToString()));
            }
            return defaultLogHandler("debug_info",list);
        }

        private bool updateBreakpoint(List<KeyValuePair<string, object>> list)
        {
            var dbHelper = DBHelper.getInstance();
            try
            {
                int id = -1;
                string keyVals = "";
                foreach (var pair in list)
                {
                    if (pair.Key.Equals("id"))
                    {
                        id = (int)pair.Value;
                        continue;
                    }

                    if (pair.Key.Equals("tag") || pair.Key.Equals("current_hits"))
                    {
                        continue;
                    }

                    if (!keyVals.Equals(""))
                    {
                        keyVals += ", ";
                    }
                    keyVals += String.Format("{0} = {1}", pair.Key, "@" + pair.Key);
                }

                SQLiteConnection conn = dbHelper.getConnection();
                if(id == -1)
                {
                    Debug.Write("update breakpoint failed, insert a new line");
                    return defaultLogHandler("breakpoint", list);
                }

                string sql = String.Format("update breakpoint set current_hits = current_hits + 1, {0} where id = {1}", keyVals, id);
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);

                foreach (var pair in list)
                {
                    cmd.Parameters.Add(new SQLiteParameter("@" + pair.Key, pair.Value + ""));
                }
                Debug.Write("execute update sql: " + cmd.CommandText);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                dbHelper.returnConnection();
            }
            return true;
        }

        private bool logException(List<KeyValuePair<string, object>> vals)
        {
            return defaultLogHandler("exception", vals);
        }

        private bool logBreakpoint(List<KeyValuePair<string, object>> vals)
        {
            foreach(KeyValuePair<string, object> val in vals)
            {
                if(val.Key.Equals("id"))
                {
                    if((int)val.Value == -1)
                    {
                        break;
                    }

                    return updateBreakpoint(vals);
                }
            }
            return defaultLogHandler("breakpoint", vals);
        }

        private bool logLocalVarialbles(List<KeyValuePair<string, object>> vals)
        {
            return defaultLogHandler("local_variable", vals);
        }

        private bool logBreakpointEvent(List<KeyValuePair<string, object>> vals)
        {
            return defaultLogHandler("breakpoint_event", vals);
        }

        

        private Dictionary<string, LogHandler> handlers;

        public delegate bool LogHandler(List<KeyValuePair<string, object>> vals);

        private List<string> tableNamesWithIdentiedId;
    }

    public class NoSuchBreakpointException : Exception 
    {
        public NoSuchBreakpointException() : base() { }
        public NoSuchBreakpointException(string message) : base(message) { }
    }
}
