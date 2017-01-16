using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Debugger.Interop;
using Microsoft.VisualStudio.Shell;
using System.Diagnostics;
using System.Threading;
using static NanjingUniversity.CppMonitor.Util.ServiceCookie;
using Microsoft.VisualStudio.TextManager.Interop;

namespace NanjingUniversity.CppMonitor.Monitor.DebugMonitor
{
    class DebugBindEvent : IBindEvent
    {

        public DebugBindEvent()
        {
            dte = Package.GetGlobalService(typeof(DTE)) as DTE;
            debugger = Package.GetGlobalService(typeof(SVsShellDebugger)) as IVsDebugger;
            debugger2 = debugger as IVsDebugger2;
            debugger3 = debugger as IVsDebugger3;
            debugger4 = debugger as IVsDebugger4;
            debuggerEvents = dte.Events.DebuggerEvents;
            buildEvents = dte.Events.BuildEvents;
        }

        public void RegisterEvent()
        {
            debuggerEvents.OnEnterBreakMode += (dbgEventReason Reason, ref dbgExecutionAction ExecutionAction) =>
            {
                Debug.Print("[DebugEvent] Break");
                Debug.Print("[DebugEvent] EventReason: " + Reason + " ExecutionAction: " + ExecutionAction);
                var breakpoint = dte.Debugger.BreakpointLastHit;
                if (breakpoint != null)
                {
                    lastBreakpoint = breakpoint;
                    Debug.Print("[DebugEvent] Breakpoint: " + breakpoint.File + " (" + breakpoint.FileLine + ", " + breakpoint.FileColumn + ")");
                }
            };

            debuggerEvents.OnContextChanged += (EnvDTE.Process NewProcess, Program 
                NewProgram, EnvDTE.Thread NewThread, EnvDTE.StackFrame NewStackFrame) => { };

            // 调试结束
            debuggerEvents.OnEnterDesignMode += (dbgEventReason Reason) => 
            {
                string output;
                Debug.Print("[DebugEvent] 调试结束");
                EnvDTE.Process process;

                lastBreakpoint = null;
            };

            // 调试开始 or 继续
            debuggerEvents.OnEnterRunMode += (dbgEventReason Reason) =>
            {
                List<string> debugTarget = new List<string>();
                List<string> debugProjects = new List<string>();
                string debugType;

                if (lastBreakpoint == null)
                {
                    Debug.Print("[DebugEvent] 调试开始");
                    foreach (EnvDTE.Process process in dte.Debugger.DebuggedProcesses)
                    {
                        debugTarget.Add(process.Name);
                        Debug.Print("[DebugEvent] 当前调试的是 " + process.Name);
                        string processName = process.Name;
                        string[] frags = processName.Split(new char[] { '\\' });
                        if (frags[frags.Length - 2].Equals("Debug") || frags[frags.Length - 2].Equals("Release"))
                        {
                            Debug.Print("[DebugEvent] 当前调试的是 " + frags[frags.Length - 2] + " 版本");
                            debugType = frags[frags.Length - 2];
                        }
                    }
                }
                else
                {
                    Debug.Print("[DebugEvent] 调试继续");
                }
            };

            debuggerEvents.OnExceptionNotHandled += (string ExceptionType, string Name, int Code, string Description, ref dbgExceptionAction ExceptionAction) => 
            {
                Debug.Print("[DebugEvent] 有未处理的异常");
                Debug.Print("[DebugEvent] ExceptionType: " + ExceptionType + "ExceptionDescription: " + Description + " Name: " + Name + " ExceptionAction: " + ExceptionAction);
            };

            debuggerEvents.OnExceptionThrown += (string ExceptionType, string Name, int Code, string Description, ref dbgExceptionAction ExceptionAction) =>
            {
                Debug.Print("[DebugEvent] 抛出异常");
                Debug.Print("[DebugEvent] ExceptionType: " + ExceptionType + "ExceptionDescription: " + Description + " Name: " + Name + " ExceptionAction: " + ExceptionAction);
            };
        }

        private DTE dte;
        private IVsDebugger debugger;
        private IVsDebugger2 debugger2;
        private IVsDebugger3 debugger3;
        private IVsDebugger4 debugger4;
        private DebuggerEvents debuggerEvents;
        private BuildEvents buildEvents;
        private Breakpoint lastBreakpoint;
    }
}
