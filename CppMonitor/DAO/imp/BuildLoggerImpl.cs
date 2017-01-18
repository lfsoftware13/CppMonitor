using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace NanjingUniversity.CppMonitor.DAO.imp
{
    class BuildLoggerImpl:ILoggerDao
    {
        private string tableName;

        public BuildLoggerImpl()
        {
            tableName = "build_log";
        }

        public Boolean LogInfo(List<KeyValuePair<String, Object>> list)
        {
            
            return false;
        }


        public void ensureTableExist()
        {
            DBHelper dbHelper = DBHelper.getInstance();
            SQLiteConnection conn = dbHelper.getConnection();
            string sql = "create table if not exists build_log (content blob, time varchar(22))";
            SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            cmd.ExecuteNonQuery();
            dbHelper.returnConnection();
        }


        public void clearLog()
        {
            DBHelper dbHelper = DBHelper.getInstance();
            SQLiteConnection conn = dbHelper.getConnection();
            string sql = "delete from " + tableName + ";";
            string sql2 = "update sqlite_sequence SET seq = 0 where name ='" + tableName + "';";
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            command.ExecuteNonQuery();
            SQLiteCommand command2 = new SQLiteCommand(sql2, conn);
            command2.ExecuteNonQuery();
            dbHelper.returnConnection();
        }
    }
}
