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
        private static string chromePath;

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

        public static string ChromePath
        {
            get
            {
                return chromePath;
            }
        }

        static AddressCommon()
        {
            String appDataPath = getAppDataPath();
            dbFilePath = Path.Combine(appDataPath,"Dao\\log.db");

            fileModuleRootPath = Path.Combine(appDataPath,"File");

            chromePath = Path.Combine(getCommonAppDataPath(), "Google", "Chrome", "User Data", "Default", "Local Storage", "chrome-extension_gnodhpdneljjpjdoiadhmigdcblneeoa_0.localstorage");
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

        public static String getCommonAppDataPath()
        {
            string path = Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData );
            return path;
        }
    }
}
