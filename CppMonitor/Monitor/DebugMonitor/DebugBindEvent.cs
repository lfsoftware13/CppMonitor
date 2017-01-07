using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using static NanjingUniversity.CppMonitor.Util.ServiceCookie;
using NanjingUniversity.CppMonitor.Monitor.DebugMonitor.Events;

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
            uint cookie;
            debugger.AdviseDebuggerEvents(new CppDebuggerEvents(), out cookie);
            cookies["IVsDebuggerEvents.DebuggerEvents"] = cookie;
            dte.Events.SolutionEvents.Opened += SolutionEvents_Opened;
            dte.Events.SolutionEvents.ProjectAdded += SolutionEvents_ProjectAdded;
            dte.Events.CommandEvents.BeforeExecute += CommandEvents_BeforeExecute;
            dte.Events.DebuggerEvents.OnEnterBreakMode += DebuggerEvents_OnEnterBreakMode;
            dte.Events.DocumentEvents.DocumentOpening += DocumentEvents_DocumentOpening;
            
        }

        private void SolutionEvents_Opened()
        {
            MessageBox.Show("open project");
        }

        private void SolutionEvents_ProjectAdded(Project Project)
        {
            MessageBox.Show("add project: " + Project.Name);
        }

        private void DocumentEvents_DocumentOpening(string DocumentPath, bool ReadOnly)
        {
            MessageBox.Show("open: " + DocumentPath);
        }

        private void DebuggerEvents_OnEnterBreakMode(dbgEventReason Reason, ref dbgExecutionAction ExecutionAction)
        {
            MessageBox.Show("reason: " + Reason + "\naction: " + ExecutionAction);
        }

        private void CommandEvents_BeforeExecute(string Guid, int ID, object CustomIn, object CustomOut, ref bool CancelDefault)
        {
            MessageBox.Show("guid: " + Guid + "\nid: " + ID);
        }

        private DTE dte;
        private IVsDebugger debugger;
    }

    namespace Events
    {
        class CppDebuggerEvents : IVsDebuggerEvents
        {
            public int OnModeChange(DBGMODE dbgmodeNew)
            {
                MessageBox.Show(dbgmodeNew.ToString());
                return 0;
            }
        }
    }
    
}
