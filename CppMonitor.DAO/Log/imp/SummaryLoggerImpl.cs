using NanjingUniversity.CppMonitor.Util.Common;
using NanjingUniversity.CppMonitor.Util.Util;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace NanjingUniversity.CppMonitor.DAO.imp
{
    class SummaryLoggerImpl : ILoggerDao
    {
        public SummaryLoggerImpl()
        {
            tableNameList = new string[1];
            tableNameList[0] = "summary_info";
        }

        public override Boolean LogInfo(string target, List<KeyValuePair<String, Object>> list)
        {
            bool result = false;
            switch (target)
            {
                case "summary_info":
                    result = logSummaryInfo(list);
                    break;
                default:
                    break;
            }
            return result;
        }

        private bool logSummaryInfo(List<KeyValuePair<String, Object>> list)
        {
            DBHelper dbHelper = DBHelper.getInstance();
            try
            {
                SQLiteConnection conn = dbHelper.getConnection();
                string sql = "insert into summary_info (time,action,solutionName,projectName) values (@time,@action,@solutionName,@projectName)";
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);

                //加时间戳
                string current = DateTime.Now.ToString();
                cmd.Parameters.Add(new SQLiteParameter("@time", current));

                string projectName = null;
                string solutionName = null;
                foreach (KeyValuePair<string, object> paramPair in list)
                {
                    switch (paramPair.Key)
                    {
                        case "action":
                            cmd.Parameters.Add(new SQLiteParameter("@action", paramPair.Value.ToString()));
                            break;
                        case "projectName":
                            projectName = paramPair.Value.ToString();
                            break;
                        case "solutionName":
                            solutionName = paramPair.Value.ToString();
                            break;
                        default:
                            break;
                    }
                }

                if (solutionName == null)
                {
                    solutionName = SolutionUtil.getSolutionName();
                }

                cmd.Parameters.Add(new SQLiteParameter("@projectName", projectName));
                cmd.Parameters.Add(new SQLiteParameter("@solutionName", solutionName));
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
            string sql = "create table if not exists summary_info (id INTEGER PRIMARY KEY autoincrement, time char[22],action char[20],solutionName TEXT,projectName TEXT)";
            SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }

    }
}
