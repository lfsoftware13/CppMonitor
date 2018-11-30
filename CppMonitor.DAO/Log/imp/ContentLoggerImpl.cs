using NanjingUniversity.CppMonitor.Util.Common;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace NanjingUniversity.CppMonitor.DAO.imp
{
    class ContentLoggerImpl:ILoggerDao
    {

        public ContentLoggerImpl()
        {
            tableNameList = new string[1];
            tableNameList[0] = "content_info";
        }

        public override Boolean LogInfo(string target,List<KeyValuePair<String, Object>> list)
        {
            bool result= false;
            result = logContentInfo(list);
            return result;
        }

        private bool logContentInfo(List<KeyValuePair<String, Object>> list)
        {

            DBHelper dbHelper = DBHelper.getInstance();
            try
            {
                SQLiteConnection conn = dbHelper.getConnection();
                string sql = "insert into content_info (time,operation,fullpath,textfrom,textto,line,lineoffset,happentime,project) values(@time,@operation,@fullpath,@textfrom,@textto, @lie, @lineoffset,@happentime,@project)";
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                //加时间戳
                string current = DateTime.Now.ToString();
                cmd.Parameters.Add(new SQLiteParameter("@time", current));
                foreach (KeyValuePair<string, object> paramPair in list)
                {
                    switch (paramPair.Key)
                    {
                        case "Operation":
                            cmd.Parameters.Add(new SQLiteParameter("@operation", paramPair.Value.ToString()));
                            break;
                        case "FilePath":
                            cmd.Parameters.Add(new SQLiteParameter("@fullpath", paramPair.Value.ToString()));
                            break;
                        case "From":
                            cmd.Parameters.Add(new SQLiteParameter("@textfrom", paramPair.Value.ToString()));
                            break;
                        case "To":
                            cmd.Parameters.Add(new SQLiteParameter("@textto", paramPair.Value.ToString()));
                            break;
                        case "Line":
                            cmd.Parameters.Add(new SQLiteParameter("@lie", paramPair.Value));
                            break;
                        case "LineOffset":
                            cmd.Parameters.Add(new SQLiteParameter("@lineoffset", paramPair.Value));
                            break;
                        case "HappenTime":
                            cmd.Parameters.Add(new SQLiteParameter("@happentime", paramPair.Value));
                            break;
                        case "Project":
                            cmd.Parameters.Add(new SQLiteParameter("@project", paramPair.Value.ToString()));
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
            //建立content_info
            string sql = "create table if not exists content_info (id INTEGER PRIMARY KEY autoincrement, time char[22],operation char[7],fullpath TEXT,textfrom blob,textto blob,line int,lineoffset int,happentime int8,project Text)";
            SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }

    }
}
