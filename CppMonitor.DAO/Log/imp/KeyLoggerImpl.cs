using NanjingUniversity.CppMonitor.Util.Common;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace NanjingUniversity.CppMonitor.DAO.imp
{
    class KeyLoggerImpl : ILoggerDao
    {
        public KeyLoggerImpl()
        {
            tableNameList = new string[1];
            tableNameList[0] = "key_info";
        }

        public override Boolean LogInfo(string target, List<KeyValuePair<String, Object>> list)
        {
            bool result = false;
            switch (target)
            {
                case "key_info":
                    result = logKeyInfo(list);
                    break;
                default:
                    break;
            }
            return result;
        }

        private bool logKeyInfo(List<KeyValuePair<String, Object>> list)
        {
            DBHelper dbHelper = DBHelper.getInstance();
            try
            {
                SQLiteConnection conn = dbHelper.getConnection();
                string sql = "insert into key_info (id,time,key,modifier,source,projectName,filePath,timeticks) values (@id,@time,@key,@modifier,@source,@projectName,@filePath,@timeticks)";
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
                        case "key":
                            cmd.Parameters.Add(new SQLiteParameter("@key", paramPair.Value.ToString()));
                            break;
                        case "modifier":
                            cmd.Parameters.Add(new SQLiteParameter("@modifier", paramPair.Value.ToString()));
                            break;
                        case "source":
                            cmd.Parameters.Add(new SQLiteParameter("@source", paramPair.Value.ToString()));
                            break;
                        case "projectName":
                            cmd.Parameters.Add(new SQLiteParameter("@projectName", paramPair.Value.ToString()));
                            break;
                        case "filePath":
                            cmd.Parameters.Add(new SQLiteParameter("@filePath", paramPair.Value.ToString()));
                            break;
                        case "timeticks":
                            cmd.Parameters.Add(new SQLiteParameter("@timeticks", paramPair.Value.ToString()));
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
            //建立key_info
            string sql = "create table if not exists key_info (id INTEGER, key char[22],modifier char[5],source char[8], projectName TEXT, filePath TEXT, time char[22],timeticks INTEGER)";
            SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }

    }
}
