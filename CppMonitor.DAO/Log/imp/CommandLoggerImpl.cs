using NanjingUniversity.CppMonitor.Util.Common;
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
        public CommandLoggerImpl()
        {
            tableNameList = new string[2];
            tableNameList[0] = "command_text";
            tableNameList[1] = "command_file";
        }

        public override Boolean LogInfo(string target,List<KeyValuePair<String, Object>> list)
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
                string sql = "insert into command_text (id,time,action,name,path,content,happentime,project) values(@id,@time,@action,@name,@path,@content,@happentime,@project)";
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                //加时间戳
                string current = DateTime.Now.ToString();
                cmd.Parameters.Add(new SQLiteParameter("@time", current));
                foreach (KeyValuePair<string, object> paramPair in list)
                {
                    switch (paramPair.Key)
                    {
                        case "id":
                            cmd.Parameters.Add(new SQLiteParameter("@id", paramPair.Value.ToString()));
                            break;
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
                        case "Project":
                            cmd.Parameters.Add(new SQLiteParameter("@project", paramPair.Value));
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
                string sql = "insert into command_file (id,time,action,filepath,pastefilepath,pasteto,project) values(@id,@time,@action,@filepath,@pastefilepath,@pasteto,@project)";
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                //加时间戳
                string current = DateTime.Now.ToString();
                cmd.Parameters.Add(new SQLiteParameter("@time", current));
                foreach (KeyValuePair<string, object> paramPair in list)
                {
                    switch (paramPair.Key)
                    {
                        case "id":
                            cmd.Parameters.Add(new SQLiteParameter("@id", paramPair.Value.ToString()));
                            break;
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
                        case "Project":
                            cmd.Parameters.Add(new SQLiteParameter("@project", paramPair.Value));
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

        public override void ensureTableExist()
        {
            SQLiteConnection conn = new SQLiteConnection("Data Source=" + AddressCommon.DBFilePath);
            conn.Open();
            //建立command_text
            string sql = "create table if not exists command_text (id INTEGER, time char[22],action char[10],name TEXT,path TEXT,content TEXT,happentime int8,project TEXT)";
            SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            cmd.ExecuteNonQuery();
            //建立command_file
            sql = "create table if not exists command_file (id INTEGER, time char[22],action char[5],filepath TEXT,pastefilepath TEXT,pasteto TEXT,project TEXT)";
            cmd = new SQLiteCommand(sql, conn);
            cmd.ExecuteNonQuery();

            conn.Close();
        }

    }
}
