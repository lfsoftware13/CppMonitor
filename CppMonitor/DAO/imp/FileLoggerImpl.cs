using NanjingUniversity.CppMonitor.Common;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace NanjingUniversity.CppMonitor.DAO.imp
{
    class FileLoggerImpl:ILoggerDao
    {
        private string[] tableNameList;

        public FileLoggerImpl()
        {
            tableNameList = new string[3];
            tableNameList[0] = "solution_open_event";
            tableNameList[1] = "snap";
            tableNameList[2] = "file_event";
        }

        public Boolean LogInfo(string target,List<KeyValuePair<String, Object>> list)
        {
            bool result= false;
            switch (target)
            {
                case "solution_open_event":
                    result = logSolutionOpenEvent(list);
                    break;
                case "snap":
                    result = logSnap(list);
                    break;
                case "file_event":
                    result = logFileEvent(list);
                    break;
                default :
                    break;
            }
            return result;
        }

        private bool logSolutionOpenEvent(List<KeyValuePair<String, Object>> list)
        {
            DBHelper dbHelper = DBHelper.getInstance();
            try
            {
                SQLiteConnection conn = dbHelper.getConnection();
                string sql = "insert into solution_open_event (time,solutionname,info) values(@time,@solutionname,@info)";
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                //加时间戳
                string current = DateTime.Now.ToString();
                cmd.Parameters.Add(new SQLiteParameter("@time", current));
                foreach (KeyValuePair<string, object> paramPair in list)
                {
                    switch (paramPair.Key)
                    {
                        case "solutionName":
                            cmd.Parameters.Add(new SQLiteParameter("@solutionname", paramPair.Value.ToString()));
                            break;
                        case "info":
                            cmd.Parameters.Add(new SQLiteParameter("@info", paramPair.Value.ToString()));
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

        private bool logSnap(List<KeyValuePair<String, Object>> list)
        {
            DBHelper dbHelper = DBHelper.getInstance();
            try
            {
                SQLiteConnection conn = dbHelper.getConnection();
                string sql = "insert into snap (time,eventid,type,location) values(@time,@eventid,@type,@location)";
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                //加时间戳
                string current = DateTime.Now.ToString();
                cmd.Parameters.Add(new SQLiteParameter("@time", current));
                foreach (KeyValuePair<string, object> paramPair in list)
                {
                    switch (paramPair.Key)
                    {
                        case "eventId":
                            cmd.Parameters.Add(new SQLiteParameter("@eventid", (long)paramPair.Value));
                            break;
                        case "type":
                            cmd.Parameters.Add(new SQLiteParameter("@type", (int)paramPair.Value));
                            break;
                        case "location":
                            cmd.Parameters.Add(new SQLiteParameter("@location", paramPair.Value.ToString()));
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

        private bool logFileEvent(List<KeyValuePair<String, Object>> list)
        {
            DBHelper dbHelper = DBHelper.getInstance();
            try
            {
                SQLiteConnection conn = dbHelper.getConnection();
                string sql = "insert into file_event (time,filename,type) values(@time,@filename,@type)";
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                //加时间戳
                string current = DateTime.Now.ToString();
                cmd.Parameters.Add(new SQLiteParameter("@time", current));
                foreach (KeyValuePair<string, object> paramPair in list)
                {
                    switch (paramPair.Key)
                    {
                        case "filename":
                            cmd.Parameters.Add(new SQLiteParameter("@filename", paramPair.Value.ToString()));
                            break;
                        case "type":
                            cmd.Parameters.Add(new SQLiteParameter("@type", (int)paramPair.Value));
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
            //建立solution_open_event
            string sql = "create table if not exists solution_open_event (time char[22],solutionname TEXT,info TEXT)";
            SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            cmd.ExecuteNonQuery();
            //建立snap
            sql = "create table if not exists snap (time char[22],eventid INT8,type tinyint,location TEXT)";
            cmd = new SQLiteCommand(sql, conn);
            cmd.ExecuteNonQuery();
            //建立file_event
            sql = "create table if not exists file_event (time char[22],filename TEXT,type tinyint)";
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
    }
}
