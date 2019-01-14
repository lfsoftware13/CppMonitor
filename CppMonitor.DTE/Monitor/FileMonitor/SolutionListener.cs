using EnvDTE;
using EnvDTE80;
using NanjingUniversity.CppMonitor.Util;
using NanjingUniversity.CppMonitor.Util.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NanjingUniversity.CppMonitor.Util.Common;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace NanjingUniversity.CppMonitor.Monitor.FileMonitor
{
    class SolutionListener
    {
        DTE dte;
        DTE2 dte2;
        SolutionEvents se;
        FileListener fl;
        string middleFilePath;

        public SolutionListener(DTE dte, DTE2 dte2)
        {
            this.dte = dte;
            this.dte2 = dte2;
            fl = new FileListener(dte);
            se = ((Events2)dte.Events).SolutionEvents;

            middleFilePath = null;
        }

        public void addListener()
        {
            se.Opened += se_Opened;
            se.BeforeClosing += se_BeforeClosing;

            se.ProjectAdded += se_AddProject;
            se.ProjectRemoved += se_RemoveProject;
            se.ProjectRenamed += se_RenameProject;

            se.Renamed += se_RenameSolution;

        }

        void se_Opened()
        {
            //记录SolutionExplorer上的结构信息
            UIHierarchy uih = dte2.ToolWindows.SolutionExplorer;
            UIHierarchyItems arr = uih.UIHierarchyItems;
            String mes = getStructure(arr, 0);
            //快照
            String projectFilePath = Path.Combine(CopyUtil.backupStartDirPath, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
            CopyUtil.backupSolutionFile(projectFilePath);
            //保存信息
            FileLogUtil.logSolutionEvent(dte2.Solution.FullName, (int)SolutionAction.solutionOpen, mes, projectFilePath);

            //创建中间文件夹
            String middlePath = Path.Combine(CopyUtil.backupMiddleDirPath, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
            if (!Directory.Exists(middlePath))
            {
                Directory.CreateDirectory(middlePath);
            }
            middleFilePath = middlePath;
            //打开解决方案后增加文件的监听
            fl.addListener(middlePath);
        }

        void se_BeforeClosing()
        {
            FileLogUtil.logSolutionEvent(dte2.Solution.FullName, (int)SolutionAction.solutionClose);
            fl.removeListener();
        }

        void se_AddProject(Project Project)
        {
            //备份项目
            string targetFolder = null;
            if (middleFilePath != null)
            {
                targetFolder = Path.Combine(middleFilePath,DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")+"-(+)" + Project.Name);
                Directory.CreateDirectory(targetFolder);
                CopyUtil.copyProjectFilesToTmp(Project,targetFolder);
            }
            else
            {
                //Debug.WriteLine("solutionListener：中间文件夹不存在");
                targetFolder = "lose info";
            }
            //记录日志
            FileLogUtil.logSolutionEvent(dte2.Solution.FullName,(int)SolutionAction.solAddProject,Project.FileName,targetFolder);
        }

        void se_RemoveProject(Project Project)
        {
            //记录日志
            FileLogUtil.logSolutionEvent(dte2.Solution.FullName, (int)SolutionAction.solDelProject, Project.Name);
        }

        void se_RenameProject(Project Project, string OldName) {
            //记录日志
            JObject jObject = new JObject();
            jObject["OldName"] = OldName;
            jObject["NewName"] = Project.Name;
            FileLogUtil.logSolutionEvent(dte2.Solution.FullName, (int)SolutionAction.solRenameProject, jObject.ToString());
        }

        void se_RenameSolution(string OldName){
            //记录rename日志
            FileLogUtil.logSolutionEvent(dte2.Solution.FullName, (int)SolutionAction.solutionRename,OldName);
        }

        private String getStructure(UIHierarchyItems items, int depths)
        {
            String ret = "";
            foreach (UIHierarchyItem item in items)
            {
                ret += "\n";
                for (int i = 0; i < depths; i++) ret += "\t";
                ret += item.Name;
                if (item.UIHierarchyItems != null)
                {
                    ret += getStructure(item.UIHierarchyItems, depths + 1);
                }
            }
            return ret;
        }
    }
}
