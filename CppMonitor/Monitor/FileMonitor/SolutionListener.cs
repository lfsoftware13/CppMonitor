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
        }

        public void addListener()
        {
            MessageBox.Show("add listener for solution events");
            se = ((Events2)dte.Events).SolutionEvents;
            se.Opened += se_Opened;
            se.BeforeClosing += se_BeforeClosing;
        }

        void se_Opened()
        {
            UIHierarchy uih = dte2.ToolWindows.SolutionExplorer;
            UIHierarchyItems arr = uih.UIHierarchyItems;
            String mes = getStructure(arr, 0);
            MessageBox.Show(mes);
            fl.addListener();
        }

        void se_BeforeClosing()
        {
            //need to save?
            MessageBox.Show("project is going to closing!");
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
