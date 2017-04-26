using NanjingUniversity.CppMonitor.Common;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace NanjingUniversity.CppMonitor.DAO.imp
{
    class CommandLoggerImpl:ILoggerDao
    {
        private string[] tableNameList;

        public CommandLoggerImpl()
        {
            tableNameList = new string[2];
            tableNameList[0] = "command_text";
            tableNameList[1] = "command_file";
        }

        public Boolean LogInfo(string target,List<KeyValuePair<String, Object>> list)
        {
            bool result= false;
            switch (target)
            {
                case "text":
                    result = logText(list);
                    break;
                case "file":
                    result = logFile(list);
                    break;
                default :
                    break;
            }
            return result;
        }

        private bool logText(List<KeyValuePair<String, Object>> list)
        {
            DBHelper dbHelper = DBHelper.getInstance();
            try
            {
                SQLiteConnection conn = dbHelper.getConnection();
                string sql = "insert into command_text (time,action,name,path,content,happentime) values(@time,@action,@name,@path,@content,@happentime)";
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                //加时间戳
                string current = DateTime.Now.ToString();
                cmd.Parameters.Add(new SQLiteParameter("@time", current));
                foreach (KeyValuePair<string, object> paramPair in list)
                {
                    switch (paramPair.Key)
                    {
                        case "Action":
                            cmd.Parameters.Add(new SQLiteParameter("@action", paramPair.Value.ToString()));
                            break;
                        case "Name":
                            cmd.Parameters.Add(new SQLiteParameter("@name", paramPair.Value.ToString()));
                            break;
                        case "Path":
                            cmd.Parameters.Add(new SQLiteParameter("@path", paramPair.Value));
                            break;
                        case "Content":
                            cmd.Parameters.Add(new SQLiteParameter("@content", paramPair.Value.ToString()));
                            break;
                        case "Happentime":
                            cmd.Parameters.Add(new SQLiteParameter("@happentime", paramPair.Value));
                            break;
                        default:
                            break;
                    }
                }
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                dbHelper.returnConnection();
            }
        }

        private bool logFile(List<KeyValuePair<String, Object>> list)
        {
            DBHelper dbHelper = DBHelper.getInstance();
            try
            {
                SQLiteConnection conn = dbHelper.getConnection();
                string sql = "insert into command_file (time,action,filepath,pastefilepath,pasteto) values(@time,@action,@filepath,@pastefilepath,@pasteto)";
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                //加时间戳
                string current = DateTime.Now.ToString();
                cmd.Parameters.Add(new SQLiteParameter("@time", current));
                foreach (KeyValuePair<string, object> paramPair in list)
                {
                    switch (paramPair.Key)
                    {
                        case "Action":
                            cmd.Parameters.Add(new SQLiteParameter("@action", paramPair.Value.ToString()));
                            break;
                        case "FilePath":
                            cmd.Parameters.Add(new SQLiteParameter("@filepath", paramPair.Value.ToString()));
                            break;
                        case "PasteFileType":
                            cmd.Parameters.Add(new SQLiteParameter("@pastefilepath", paramPair.Value.ToString()));
                            break;
                        case "PasteTo":
                            cmd.Parameters.Add(new SQLiteParameter("@pasteto", paramPair.Value.ToString()));
                            break;
                        default:
                            break;
                    }
                }
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                dbHelper.returnConnection();
            }
        }

        public void ensureTableExist()
        {
            SQLiteConnection conn = new SQLiteConnection("Data Source=" + AddressCommon.DBFilePath);
            conn.Open();
            //建立command_text
            string sql = "create table if not exists command_text (id INTEGER PRIMARY KEY AUTOINCREMENT, time char[22],action char[10],name TEXT,path TEXT,content TEXT,happentime int8)";
            SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            cmd.ExecuteNonQuery();
            //建立command_file
            sql = "create table if not exists command_file (id INTEGER PRIMARY KEY AUTOINCREMENT, time char[22],action char[5],filepath TEXT,pastefilepath TEXT,pasteto TEXT)";
            cmd = new SQLiteCommand(sql, conn);
            cmd.ExecuteNonQuery();

            conn.Close();
        }


        public void clearLog()
        {
            DBHelper dbHelper = DBHelper.getInstance();
            SQLiteConnection conn = dbHelper.getConnection();
            foreach(string tableName in tableNameList){
                string sql = "delete from " + tableName + ";";
                string sql2 = "update sqlite_sequence SET seq = 0 where name ='" + tableName + "';";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.ExecuteNonQuery();
                SQLiteCommand command2 = new SQLiteCommand(sql2, conn);
                command2.ExecuteNonQuery();
            }
            dbHelper.returnConnection();
        }


        public int returnKeyAfterLogInfo(string target, List<KeyValuePair<string, object>> list)
        {
            throw new NotImplementedException();
        }
    }
}
