using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanjingUniversity.CppMonitor.Monitor.FileMonitor
{
    class CopyUtil
    {
        private static String directoryPath = @"D:\cr_vs2013\vs_plugin\CppMonitor\Monitor\FileMonitor\backup\";

        public static void CopyFile(String source, String name)
        {
            if (!System.IO.Directory.Exists(directoryPath))
            {
                //目录不存在，建立目录
                System.IO.Directory.CreateDirectory(directoryPath);
            }
            bool isrewrite = true; //覆盖同名文件
            System.IO.File.Copy(source, directoryPath + name, isrewrite);
        }
    }
}
