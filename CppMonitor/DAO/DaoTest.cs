using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
namespace NanjingUniversity.CppMonitor.DAO
{
    class DaoTest
    {
        static void Main(string[] args)
        {
            DaoTest test = new DaoTest();
            test.testConnect();
        }

        public void testConnect()
        {
            SQLiteConnection.CreateFile("pluginLog.sqlite");
            SQLiteConnection m_dbConnection = new SQLiteConnection("Data Source=pluginLog.sqlite;Version=3;");
            m_dbConnection.Open();
            string sql = "create table buildLog (time varchar(20), score int)";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();
        }
    }
}
