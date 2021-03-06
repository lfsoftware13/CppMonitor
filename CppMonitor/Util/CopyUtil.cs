﻿using EnvDTE;
using Microsoft.VisualStudio.Shell;
using NanjingUniversity.CppMonitor.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NanjingUniversity.CppMonitor.Util
{
    class CopyUtil
    {
        public static String backupStartDirPath = Path.Combine(AddressCommon.FileModuleRootPath,"start_files");
        public static String backupMiddleDirPath = Path.Combine(AddressCommon.FileModuleRootPath, "middle_files");
        public static String backupBuildDirPath = Path.Combine(AddressCommon.FileModuleRootPath, "build_files");

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

        public static bool copyFile(string source, string target)
        {
            //若不存在，直接复制文件；若存在，覆盖复制
            if (File.Exists(source))
            {
                String dirPath = Path.GetDirectoryName(target);
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                if (File.Exists(target))
                {
                    File.Copy(source, target, true);
                }
                else
                {
                    File.Copy(source, target);
                }
                return true;
            }
            return false;
        }

        public static bool copyProjectFilesToTmp(ProjectItems parentItems, String parentDir)
        {
            bool containsFile = false;//判断是否有文件
            foreach (ProjectItem item in parentItems)
            {
                string itemName = item.Name;
                string subParentPath = Path.Combine(parentDir, itemName);
                try
                {
                    item.Properties.Item("ItemType");//确保存在这个属性
                    //MessageBox.Show((String)item.Properties.Item("ItemType").Value);
                    if (item.Properties.Item("ItemType").Value.Equals("ClInclude") || item.Properties.Item("ItemType").Value.Equals("ClCompile"))
                    {
                        for (short i = 0; i < item.FileCount; i++)
                        {
                            String fileFullPath = item.get_FileNames(i);
                            if (File.Exists(fileFullPath))
                            {
                                containsFile = containsFile | CopyUtil.copyFile(fileFullPath, Path.Combine(parentDir, Path.GetFileName(fileFullPath)));
                            }
                        }
                    }
                    else
                    {
                        containsFile = containsFile | copyProjectFilesToTmp(item.ProjectItems, subParentPath);
                    }
                }
                catch (Exception)
                {
                    //MessageBox.Show("exception");
                    containsFile = containsFile | copyProjectFilesToTmp(item.ProjectItems, subParentPath);
                    continue;
                }
            }

            return containsFile;
        }

        //将解决方案下文件完整备份
        public static void backupSolutionFile(String targetPath){
            DTE dte = (DTE)ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE.DTE));
            Projects projects = dte.Solution.Projects;

            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }
            foreach (Project project in projects)
            {
                CopyUtil.copyProjectFilesToTmp(project.ProjectItems, Path.Combine(targetPath, project.Name));
            }
        }
    }
}
