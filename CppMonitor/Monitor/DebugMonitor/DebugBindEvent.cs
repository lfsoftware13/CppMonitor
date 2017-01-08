using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using System.Diagnostics;
using System.Threading;
using static NanjingUniversity.CppMonitor.Util.ServiceCookie;

namespace NanjingUniversity.CppMonitor.Monitor.DebugMonitor
{
    class DebugBindEvent : IBindEvent
    {

        public DebugBindEvent()
        {
            this.dte = Package.GetGlobalService(typeof(DTE)) as DTE;
            this.debugger = Package.GetGlobalService(typeof(SVsShellDebugger)) as IVsDebugger;
        }

        public void RegisterEvent()
        {
            dte.Events.WindowEvents.WindowCreated += (window) =>
            {
                Debug.WriteLine("[Info] Window opened: " + window.Caption);
                var watcher = BreakpointWatcher.Watch(window);
                watcher.BreakpointCreatedEvents += (bp) =>
                {
                    MessageBox.Show("添加了断点:" + bp.Tag);
                };
            };

            dte.Events.WindowEvents.WindowClosing += (window) =>
            {
                Debug.WriteLine("[Info] Window closing: " + window.Caption);
            };
        }
        private DTE dte;
        private IVsDebugger debugger;
    }

    class BreakpointEvents1 : IBreakpointHandler
    {
        public void OnBreakpointCreated(Breakpoint bp)
        {
            MessageBox.Show("添加了断点:" + bp.Tag);
        }

        public void OnBreakpointDisabled(Breakpoint bp) { }

        public void OnBreakpointEnabled(Breakpoint bp) { }

        public void OnBreakpointModified(Breakpoint bp) { }

        public void OnBreakpointRemoved(Breakpoint bp) { }
    }
}
