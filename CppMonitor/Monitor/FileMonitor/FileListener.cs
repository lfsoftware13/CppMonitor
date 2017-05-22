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
            vcp.ItemRenamed += vcp_ItemRenamed;
        }

        public void removeListener()
        {
            //MessageBox.Show("remove listener for file events");
            vcp.ItemAdded -= vcp_ItemAdded;
            vcp.ItemRemoved -= vcp_ItemRemoved;
            vcp.ItemRenamed -= vcp_ItemRenamed;
        }

        #region rename function
        void vcp_ItemRenamed(object Item, object ItemParent, string OldName)
        {
          VCProjectItem pitem = Item as VCProjectItem;
          //avoid to get null object
           if(pitem != null){
               if (pitem.Kind.Equals("VCFile"))
               {
                   VCFile f = Item as VCFile;
                   FileLogUtil.logFileEvent(5, f.FullPath, (f.project).Name,OldName);//记录日志
                   String desDir = getDesDir(f.FullPath);
               }
               else if (pitem.Kind.Equals("VCFilter"))
               {
                   VCFilter f = Item as VCFilter;
                   FileLogUtil.logFileEvent(6, f.CanonicalName, f.project.Name,OldName);//记录日志
               }
          }
        }
        #endregion

        void vcp_ItemRemoved(object Item, object ItemParent)
        {
            VCProjectItem pitem = Item as VCProjectItem;
            if (pitem.Kind.Equals("VCFile"))
            {
                VCFile f = Item as VCFile;
                //MessageBox.Show("remove file: " + f.FullPath);
                FileLogUtil.logFileEvent(2,f.FullPath,(f.project).Name);//记录日志
                String desDir = getDesDir(f.FullPath);
                //CopyUtil.copyFile(f.FullPath, Path.Combine(desDir, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "-(-)" + f.Name));
            }
            else if (pitem.Kind.Equals("VCFilter"))
            {
                VCFilter f = Item as VCFilter;
                //MessageBox.Show("remove filter: " + f.CanonicalName);
                FileLogUtil.logFileEvent(4,f.CanonicalName,f.project.Name);//记录日志
            }
        }

        void vcp_ItemAdded(object Item, object ItemParent)
        {
            VCProjectItem pitem = Item as VCProjectItem;
            if (pitem.Kind.Equals("VCFile"))
            {
                VCFile f = Item as VCFile;
                //MessageBox.Show("add file: " + f.Name);
                //MessageBox.Show(f.RelativePath);
                String desDir = getDesDir(f.FullPath);
                string desFile = Path.Combine(desDir, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "-(+)" + f.Name);
                CopyUtil.copyFile(f.FullPath, desFile);
                FileLogUtil.logFileEvent(1, f.FullPath, (f.project).Name,desFile);//记录日志
                
            }
            else if (pitem.Kind.Equals("VCFilter"))
            {
                VCFilter f = Item as VCFilter;
                //MessageBox.Show("add filter: " + f.CanonicalName);
                FileLogUtil.logFileEvent(3, f.CanonicalName, (f.project).Name);//记录日志
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
