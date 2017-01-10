using EnvDTE;
using Microsoft.VisualStudio.VCProjectEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NanjingUniversity.CppMonitor.Monitor.FileMonitor
{
    class FileListener
    {
        DTE dte;
        VCProjectEngineEvents vcp;

        public FileListener(DTE dte)
        {
            this.dte = dte;
            vcp = dte.Events.GetObject("VCProjectEngineEventsObject") as VCProjectEngineEvents;
        }

        public void addListener()
        {
            MessageBox.Show("add listener for file events");
            vcp.ItemAdded += vcp_ItemAdded;
            vcp.ItemRemoved += vcp_ItemRemoved;
        }

        public void removeListener()
        {
            MessageBox.Show("remove listener for file events");
            vcp.ItemAdded -= vcp_ItemAdded;
            vcp.ItemRemoved -= vcp_ItemRemoved;
        }

        void vcp_ItemRemoved(object Item, object ItemParent)
        {
            VCProjectItem pitem = Item as VCProjectItem;
            if (pitem.Kind.Equals("VCFile"))
            {
                VCFile f = Item as VCFile;
                MessageBox.Show("remove file: " + f.FullPath);
                CopyUtil.copyFile(f.FullPath, CopyUtil.backupDirPath + "\\" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "-" + f.Name);
            }
            else if (pitem.Kind.Equals("VCFilter"))
            {
                VCFilter f = Item as VCFilter;
                MessageBox.Show("remove dir: " + f.CanonicalName);
                //CopyUtil.copyFile(f.FullPath, CopyUtil.backupDirPath + "\\" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "-" + f.Name);
            }
        }

        void vcp_ItemAdded(object Item, object ItemParent)
        {
            VCProjectItem pitem = Item as VCProjectItem;
            if (pitem.Kind.Equals("VCFile"))
            {
                VCFile f = Item as VCFile;
                MessageBox.Show("add file: " + f.Name);
                CopyUtil.copyFile(f.FullPath, CopyUtil.backupDirPath + "\\" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "-" + f.Name);
            }
            else if (pitem.Kind.Equals("VCFilter"))
            {
                VCFilter f = Item as VCFilter;
                MessageBox.Show("add filter: " + f.CanonicalName);
                //CopyUtil.copyFile(f.FullPath, CopyUtil.backupDirPath + "\\" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "-" + f.Name);
            }
        }
    }
}
