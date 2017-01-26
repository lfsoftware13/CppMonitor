using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Threading;
using NanjingUniversity.CppMonitor.Common;
using System.IO;
namespace NanjingUniversity.CppMonitor.DAO
{
    class DBHelper
    {
     
        private static string dbParams = "";
        private static SQLiteConnection conn;
        private static Semaphore sem;

        private static String[] daoModulelist = { "Build", "Command", "Content", "Debug", "File" };

        
        private DBHelper()
        {
            string dbFilePath = AddressCommon.DBFilePath;
            ensureDBFileExist(dbFilePath);
            dbParams = "Data Source=" + dbFilePath + ";Version=3;Synchronous=OFF;Journal Mode=WAL;";
            sem = new Semaphore(1, 1);
            conn = null;
        }

        #region 检查db文件是否存在

        private void ensureDBFileExist(string dbFilePath)
        {
            if (!File.Exists(dbFilePath))
            {
                Directory.GetParent(dbFilePath).Create();
                //创建数据库
                SQLiteConnection.CreateFile(dbFilePath);
            }
            ensureTableExist();
        }

        private void ensureTableExist()
        {
            foreach (String key in daoModulelist)
            {
                ILoggerDao logger = LoggerFactory.loggerFactory.getLogger(key);
                if (logger != null)
                {
                    logger.ensureTableExist();
                }
            }
        }

        #endregion

        #region 单例
        private static DBHelper instance;

        public static DBHelper getInstance()
        {
            if (instance == null)
            {
                instance = new DBHelper();
            }
            return instance;
        }
        #endregion


        #region 数据库连接相关
        //每次用完都需要归还
        public SQLiteConnection getConnection()
        {
            sem.WaitOne();
            if (conn == null)
            {
                conn = new SQLiteConnection(dbParams);
                conn.Open();
            }
            return conn;
        }
        public void returnConnection()
        {
            sem.Release();
        }

        //在使用完毕之后 关闭连接
        public void closeConnection()
        {
            sem.WaitOne();
            if (conn != null)
            {
                conn.Close();
            }
            conn = null;
            sem.Release();
        }
        #endregion


        public void clearLog(string tableName)
        {
            foreach (String key in daoModulelist)
            {
                ILoggerDao logger = LoggerFactory.loggerFactory.getLogger(key);
                if (logger != null)
                {
                    logger.clearLog();
                }
            }
        }
    }
}
