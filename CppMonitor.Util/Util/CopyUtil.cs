using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.VCProjectEngine;
using NanjingUniversity.CppMonitor.Util.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanjingUniversity.CppMonitor.Util.Util
{
    public class CopyUtil
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

        private static List<string> targetItemTypeList = new List<string>()
        {
            "ClInclude",
            "ClCompile",
        };

        public static bool copyProjectFilesToTmp(Project project, String targetDir)
        {
            //备份内容
            bool copyContentResult = copyProjectFilesToTmpInnter(project.ProjectItems, targetDir, true);
            //备份项目的proj文件；
            string proFullName = project.FullName;
            backUpFile(proFullName, targetDir, true);

            return copyContentResult;
        }

        private static bool copyProjectFilesToTmpInnter(ProjectItems parentItems, String parentDir, bool isRoot = false)
        {
            bool containsFile = false;//判断是否有文件
            foreach (ProjectItem item in parentItems)
            {
                string itemName = item.Name;
                string subParentPath = Path.Combine(parentDir, itemName);
                if (item.Object is VCFilter)
                {
                    containsFile = containsFile | copyProjectFilesToTmpInnter(item.ProjectItems, subParentPath);
                }
                else if (item.Object is VCFile)
                {
                    VCFile theVCFile = item.Object as VCFile;
                    string itemType = theVCFile.ItemType;

                    ///这里复制vc编译会用到的和vcxproj.filters文件
                    if (targetItemTypeList.Contains(itemType) || itemName.EndsWith("vcxproj.filters"))
                    {
                        for (short i = 0; i < item.FileCount; i++)
                        {
                            String fileFullPath = item.get_FileNames(i);
                            if (isRoot)
                            {
                                String fileFullDir = Path.GetDirectoryName(fileFullPath);
                                fileFullDir = fileFullDir.Substring(fileFullDir.LastIndexOf("\\"));
                                parentDir = parentDir.Substring(0, parentDir.LastIndexOf("\\")) + fileFullDir;
                            }
                            if (File.Exists(fileFullPath))
                            {
                                containsFile = containsFile | CopyUtil.copyFile(fileFullPath, Path.Combine(parentDir, Path.GetFileName(fileFullPath)));
                            }
                        }
                    }
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
                string projectTargetPath = Path.Combine(targetPath, project.Name);
                CopyUtil.copyProjectFilesToTmp(project, projectTargetPath);
            }

            //备份解决方案的sln文件
            Solution currentSolution = dte.Solution;
            backUpFile(currentSolution.FullName,targetPath);
        }

        private static void backUpFile(string fileFullPath, String targetDir, bool isProjectFile = false)
        {
            String fileName = fileFullPath.Substring(fileFullPath.LastIndexOf("\\")+1);
            if (isProjectFile)
            {
                String fileDir = Path.GetDirectoryName(fileFullPath);
                fileDir = fileDir.Substring(fileDir.LastIndexOf("\\"));
                targetDir = targetDir.Substring(0, targetDir.LastIndexOf("\\")) + fileDir;
            }
            String dirFilePath = Path.Combine(targetDir, fileName);
            //保证目标文件已经存在
            Directory.CreateDirectory(targetDir);
            File.Copy(fileFullPath,dirFilePath);
        }
    }
}
