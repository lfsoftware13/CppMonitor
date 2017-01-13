using EnvDTE;
using Microsoft.VisualStudio.VCProjectEngine;
using System;
using System.Collections.Generic;
using System.IO;
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
        String middlePath;

        public FileListener(DTE dte)
        {
            this.dte = dte;
            vcp = dte.Events.GetObject("VCProjectEngineEventsObject") as VCProjectEngineEvents;
        }

        public void addListener(String middlePath)
        {
            //MessageBox.Show("add listener for file events");
            this.middlePath = middlePath;
            vcp.ItemAdded += vcp_ItemAdded;
            vcp.ItemRemoved += vcp_ItemRemoved;
        }

        public void removeListener()
        {
            //MessageBox.Show("remove listener for file events");
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
                String desDir = getDesDir(f.FullPath);
                CopyUtil.copyFile(f.FullPath, Path.Combine(desDir, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "-(-)" + f.Name));
            }
            else if (pitem.Kind.Equals("VCFilter"))
            {
                VCFilter f = Item as VCFilter;
                MessageBox.Show("remove filter: " + f.CanonicalName);
            }
        }

        void vcp_ItemAdded(object Item, object ItemParent)
        {
            VCProjectItem pitem = Item as VCProjectItem;
            if (pitem.Kind.Equals("VCFile"))
            {
                VCFile f = Item as VCFile;
                MessageBox.Show("add file: " + f.Name);
                MessageBox.Show(f.RelativePath);
                String desDir = getDesDir(f.FullPath);
                CopyUtil.copyFile(f.FullPath, Path.Combine(desDir, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "-(+)" + f.Name));
            }
            else if (pitem.Kind.Equals("VCFilter"))
            {
                VCFilter f = Item as VCFilter;
                MessageBox.Show("add filter: " + f.CanonicalName);
            }
        }

        private String getDesDir(String fullPath)
        {
            String thisDir = Path.GetDirectoryName(fullPath);
            if (thisDir.Contains(Path.GetDirectoryName(dte.Solution.FullName)))
            {
                return thisDir.Replace(Path.GetDirectoryName(dte.Solution.FullName), this.middlePath);
            }
            else
            {
                return Path.Combine(this.middlePath, "out files");
            }
        }
    }
}
