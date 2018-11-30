using NanjingUniversity.CppMonitor.Util.Common;
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
        public FileLoggerImpl()
        {
            tableNameList = new string[2];
            tableNameList[0] = "solution_open_event";
            tableNameList[1] = "file_event";
        }

        public override Boolean LogInfo(string target,List<KeyValuePair<String, Object>> list)
        {
            bool result= false;
            switch (target)
            {
                case "solution_open_event":
                    result = logSolutionOpenEvent(list);
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
                string sql = "insert into solution_open_event (time,solutionname,type,info,targetfolder) values(@time,@solutionname,@type,@info,@targetfolder)";
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
                        case "type":
                            cmd.Parameters.Add(new SQLiteParameter("@type", paramPair.Value));
                            break;
                        case "targetFolder":
                            cmd.Parameters.Add(new SQLiteParameter("@targetfolder", paramPair.Value.ToString()));
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
                string sql = "insert into file_event (time,filename,projectname,type,targetFile) values(@time,@filename,@projectname,@type,@targetFile)";
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                //加时间戳
                string current = DateTime.Now.ToString();
                cmd.Parameters.Add(new SQLiteParameter("@time", current));
                foreach (KeyValuePair<string, object> paramPair in list)
                {
                    switch (paramPair.Key)
                    {
                        case "fileName":
                            cmd.Parameters.Add(new SQLiteParameter("@filename", paramPair.Value.ToString()));
                            break;
                        case "projectName":
                            cmd.Parameters.Add(new SQLiteParameter("@projectname", paramPair.Value.ToString()));
                            break;
                        case "type":
                            cmd.Parameters.Add(new SQLiteParameter("@type", paramPair.Value));
                            break;
                        case "targetFile":
                            cmd.Parameters.Add(new SQLiteParameter("@targetFile", paramPair.Value.ToString()));
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
            //建立solution_open_event
            string sql = "create table if not exists solution_open_event (id INTEGER PRIMARY KEY autoincrement, time char[22],solutionname TEXT,type tinyint,info TEXT,targetfolder TEXT)";
            SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            cmd.ExecuteNonQuery();
            //建立file_event
            sql = "create table if not exists file_event (id INTEGER PRIMARY KEY autoincrement, time char[22],filename TEXT,projectname TEXT,type tinyint,targetFile TEXT)";
            cmd = new SQLiteCommand(sql, conn);
            cmd.ExecuteNonQuery();

            conn.Close();
        }

    }
}
