using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
namespace NanjingUniversity.CppMonitor.DAO.imp
{
    class DBHelper
    {
        private static SQLiteConnection conn;
        private static string dataname = "Data Source=log.db;Version=3;";
        private static DBHelper instance;
        private DBHelper()
        {
        }

        public static DBHelper getInstance()
        {
            if (instance==null)
            {
                instance = new DBHelper();
                conn = new SQLiteConnection(dataname);
                conn.Open();
            }
            return instance;
        }

        public void insert(string sql)
        {
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            string current = DateTime.Now.ToString();
            command.Parameters.Add(new SQLiteParameter("@time", current));
            command.ExecuteNonQuery();
        }

        public void printData()
        {
            string sql = "select * from build_log";
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
                Console.WriteLine("content: " + reader["content"]);
            Console.ReadLine();
        }

        public void closeConnection()
        {
            if (conn != null)
            {
                conn.Close();
            }
            conn = null;
        }

        public void insertMulti()
        {
            string sql = "insert into build_log (time, content) values (@time, 'test');";
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            for (int i = 0; i < 5; i++)
            {
                string current = DateTime.Now.ToString();
                command.Parameters.Add(new SQLiteParameter("@time", current));
                command.ExecuteNonQuery();
            }
        }

        public void clearLog(string tableName)
        {
            string sql = "delete from " + tableName + ";";
            string sql2 = "update sqlite_sequence SET seq = 0 where name ='"+tableName+"';";
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            command.ExecuteNonQuery();
            SQLiteCommand command2 = new SQLiteCommand(sql2, conn);
            command2.ExecuteNonQuery();
        }
    }
}
