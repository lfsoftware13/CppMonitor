using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using NanjingUniversity.CppMonitor.DAO.imp;
namespace NanjingUniversity.CppMonitor.DAO
{
    class DaoTest
    {
        static void Main(string[] args)
        {
            DaoTest.testInsert();
        }

        public static void testInsert()
        {
            DBHelper helper = DBHelper.getInstance();
            helper.insertMulti();
        }

        public void testShow()
        {
            DBHelper helper = DBHelper.getInstance();
            helper.printData();
        }

        public void testMultiThread()
        {
            System.DateTime start = System.DateTime.Now;
            Thread t1 = new Thread(new ThreadStart(testInsert));
            DaoTest.testInsert();
            t1.Start();
            System.DateTime end = System.DateTime.Now;
            TimeSpan span = end - start;
            Console.WriteLine(span.TotalMilliseconds / 1000);
        }

        public static void testClear()
        {
            DBHelper helper = DBHelper.getInstance();
            helper.clearLog("build_log");
        }
    }
}
