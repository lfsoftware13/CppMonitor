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
            Dictionary<string, StringBuilder> debugOutputs = new Dictionary<string, StringBuilder>();
            debuggerEvents.OnEnterBreakMode += (dbgEventReason Reason, ref dbgExecutionAction ExecutionAction) =>
            {
                string breakReason = Reason + "";
                Breakpoint breakpoint = dte.Debugger.BreakpointLastHit;
                DebugLogUtil.LogDebugBreak(dte.Debugger.DebuggedProcesses.Item(1).Name, Reason + "", breakpoint);
            };

            debuggerEvents.OnContextChanged += (EnvDTE.Process NewProcess, Program
                NewProgram, EnvDTE.Thread NewThread, EnvDTE.StackFrame NewStackFrame) =>
            {
                Debug.Print("[DebugEvent] Process: " + NewProcess.Name);
            };

            // 调试结束
            debuggerEvents.OnEnterDesignMode += (dbgEventReason Reason) =>
            {
                if (dte.Debugger.DebuggedProcesses.Count < 1) return;
                DebugLogUtil.LogDebugBreak(dte.Debugger.DebuggedProcesses.Item(1).Name, Reason + "", null);
            };

            // 调试开始 or 继续
            debuggerEvents.OnEnterRunMode += (dbgEventReason Reason) =>
            {
                if (!isStarted)
                {
                    Debug.Print("[DebugEvent] 调试开始");
                    isStarted = true;
                    DebugLogUtil.LogDebugStart(dte.Debugger.DebuggedProcesses.Item(1).Name);
                }
                else
                {
                    Debug.Print("[DebugEvent] 调试继续");
                    DebugLogUtil.LogDebugContinue(dte.Debugger.DebuggedProcesses.Item(1).Name, dte.Debugger.BreakpointLastHit);
                }
            };

            debuggerEvents.OnExceptionNotHandled += (string ExceptionType, string Name, int Code, string Description, ref dbgExceptionAction ExceptionAction) =>
            {
                string exceptionType = ExceptionType;
                string description = Description;

                DebugLogUtil.LogDebugExceptionNotHandled(dte.Debugger.DebuggedProcesses.Item(1).Name, ExceptionType, Name, Description, Code, ExceptionAction + "");
            };

            debuggerEvents.OnExceptionThrown += (string ExceptionType, string Name, int Code, string Description, ref dbgExceptionAction ExceptionAction) =>
            {
                string exceptionType = ExceptionType;
                string description = Description;

                DebugLogUtil.LogDebugExceptionThrown(dte.Debugger.DebuggedProcesses.Item(1).Name, ExceptionType, Name, Description, Code, ExceptionAction + "");
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
        private bool isStarted = false;
    }
}
