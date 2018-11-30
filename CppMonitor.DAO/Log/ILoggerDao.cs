using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Data.SQLite;

namespace NanjingUniversity.CppMonitor.DAO
{
    public abstract class ILoggerDao
    {
        protected string[] tableNameList;

        public abstract Boolean LogInfo(string Target,List<KeyValuePair<String, Object>> List);

        public int returnKeyAfterLogInfo(string Target,List<KeyValuePair<String,Object>> List)
        {
            LogInfo(Target, List);
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
            foreach (string tableName in tableNameList)
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
        //确保日志文件是否存在
        public abstract void ensureTableExist();
    }
}
