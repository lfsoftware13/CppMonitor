using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanjingUniversity.CppMonitor.Monitor.FileMonitor
{
    class CopyUtil
    {
        public static String backupDirPath = @"D:\cr_vs2013\vs_plugin\CppMonitor\Monitor\FileMonitor\backup";

        public static void copyDir(string sourceDir, string targetDir)
        {
            if (!Directory.Exists(sourceDir))
            {
                Directory.CreateDirectory(sourceDir);
            }
            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }
            //拷贝文件
            string[] filenames = Directory.GetFiles(sourceDir);
            foreach (string item in filenames)
            {
                string item_target = targetDir + "\\" + item.Substring(sourceDir.Length + 1);
                copyFile(item, item_target);
            }
            //拷贝子目录
            string[] dirnames = Directory.GetDirectories(sourceDir);
            foreach (string item in dirnames)
            {
                string item_target = targetDir + "\\" + item.Substring(sourceDir.Length + 1);
                copyDir(item, item_target);
            }
        }

        public static void copyFile(string source, string target)
        {
            //若不存在，直接复制文件；若存在，覆盖复制
            if (File.Exists(target))
            {
                File.Copy(source, target, true);
            }
            else
            {
                File.Copy(source, target);
            }
        }
    }
}
