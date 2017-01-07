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
                
                new System.Threading.Thread(() => 
                {
                    while (true)
                    {
                        var breakPoints = window.DTE.Debugger.Breakpoints;
                        foreach (Breakpoint breakPoint in breakPoints)
                        {
                            Debug.WriteLine("[BreakPoint] " + breakPoint.File + " " + breakPoint.FileLine);
                        }
                        System.Threading.Thread.Sleep(1000);
                    }
                }).Start();  
            };

            dte.Events.WindowEvents.WindowClosing += (window) =>
            {
                Debug.WriteLine("[Info] Window closing: " + window.Caption);
            };
        }
        private DTE dte;
        private IVsDebugger debugger;
    }
}
