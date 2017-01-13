using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NanjingUniversity.CppMonitor.Monitor.FileMonitor
{
    class FileBindEvent : IBindEvent
    {
        public void RegisterEvent()
        {
            MessageBox.Show("file monitor init!");
            DTE dte = (DTE)ServiceProvider.GlobalProvider.GetService(typeof(DTE));
            DTE2 dte2 = ServiceProvider.GlobalProvider.GetService(typeof(SDTE)) as DTE2;

            SolutionListener sl = new SolutionListener(dte, dte2);
            sl.addListener();
        }
    }
}
