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
        }

        public void addListener()
        {
            MessageBox.Show("add listener for file events");
            vcp = dte.Events.GetObject("VCProjectEngineEventsObject") as VCProjectEngineEvents;
            vcp.ItemAdded += vcp_ItemAdded;
            vcp.ItemRemoved += vcp_ItemRemoved;
        }

        void vcp_ItemRemoved(object Item, object ItemParent)
        {
            VCProjectItem pitem = Item as VCProjectItem;
            MessageBox.Show(pitem.ItemName + " " + pitem.Kind);
        }

        void vcp_ItemAdded(object Item, object ItemParent)
        {
            VCProjectItem pitem = Item as VCProjectItem;
            MessageBox.Show(pitem.ItemName + " " + pitem.Kind);
        }
    }
}
