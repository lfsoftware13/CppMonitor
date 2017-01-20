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
        private static string fileModuleRootPath;

        public static string DBFilePath
        {
            get
            {
                return dbFilePath;
            }
        }

        public static string FileModuleRootPath
        {
            get
            {
                if(!Directory.Exists(fileModuleRootPath)){
                    Directory.CreateDirectory(fileModuleRootPath);
                }
                return fileModuleRootPath;
            }
        }

        static AddressCommon()
        {
            String appDataPath = getAppDataPath();
            dbFilePath = Path.Combine(appDataPath,"Dao\\log.db");

            fileModuleRootPath = Path.Combine(appDataPath,"File");
        }

        public static String getAppDataPath()
        {
            String path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            path = Path.Combine(path, "CppMonitor");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);    
            }
            return path;
        }
    }
}
