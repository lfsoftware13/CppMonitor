using EnvDTE;
using EnvDTE80;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NanjingUniversity.CppMonitor.Monitor.FileMonitor
{
    class SolutionListener
    {
        DTE dte;
        DTE2 dte2;
        SolutionEvents se;
        FileListener fl;

        public SolutionListener(DTE dte, DTE2 dte2)
        {
            this.dte = dte;
            this.dte2 = dte2;
            fl = new FileListener(dte);
            se = ((Events2)dte.Events).SolutionEvents;
        }

        public void addListener()
        {
            MessageBox.Show("add listener for solution events");
            se.Opened += se_Opened;
            se.BeforeClosing += se_BeforeClosing;
        }

        void se_Opened()
        {
            UIHierarchy uih = dte2.ToolWindows.SolutionExplorer;
            UIHierarchyItems arr = uih.UIHierarchyItems;
            String mes = getStructure(arr, 0);
            //保存信息
            MessageBox.Show(mes);
            //快照
            String solutionFullname = dte.Solution.FullName;
            int lindex = solutionFullname.LastIndexOf("\\");
            int diff = solutionFullname.LastIndexOf(".")-lindex;
            String name =  solutionFullname.Substring(lindex+1, diff-1);
            String solutionDir = solutionFullname.Substring(0, lindex);
            CopyUtil.copyDir(solutionDir, CopyUtil.backupDirPath+"\\"+DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "-"+name);
            //打开解决方案后增加文件的监听
            fl.addListener();
        }

        void se_BeforeClosing()
        {
            MessageBox.Show("project is going to closing!");
            fl.removeListener();
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
