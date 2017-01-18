using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanjingUniversity.CppMonitor.Common
{
    class AddressCommon
    {
        private static string dbFilePath;

        public static string DBFilePath
        {
            get
            {
                return dbFilePath;
            }
        }

        static AddressCommon()
        {
            String appDataPath = getAppDataPath();
            dbFilePath = Path.Combine(appDataPath,"Dao/log.db");
            
        }

        public static String getAppDataPath()
        {
            String path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            path = Path.Combine(path, "CppMonitor");
            return path;
        }
    }
}
