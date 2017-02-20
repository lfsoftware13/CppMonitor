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
            windowEvents = dte.Events.WindowEvents;
        }

        public void RegisterEvent()
        {
            Dictionary<string, StringBuilder> debugOutputs = new Dictionary<string, StringBuilder>();
            debuggerEvents.OnEnterBreakMode += (dbgEventReason Reason, ref dbgExecutionAction ExecutionAction) =>
            {
                string breakReason = Reason + "";
                Breakpoint breakpoint = dte.Debugger.BreakpointLastHit;
                DebugLogUtil.LogDebugBreak(dte.Debugger.DebuggedProcesses.Item(1).Name, Reason + "", breakpoint, dte.Debugger.CurrentStackFrame.Locals);
                var arguments = dte.Debugger.CurrentStackFrame.Arguments;
                var locals = dte.Debugger.CurrentStackFrame.Locals;
                var args = new Dictionary<string, object>();
                var locs = new Dictionary<string, object>();
                
            };

            // 调试结束
            debuggerEvents.OnEnterDesignMode += (dbgEventReason Reason) =>
            {
                if (dte.Debugger.DebuggedProcesses.Count < 1)
                {
                    DebugLogUtil.LogDebugBreak(null, Reason + "", null, null);
                }
                else
                {
                    DebugLogUtil.LogDebugBreak(dte.Debugger.DebuggedProcesses.Item(1).Name, Reason + "", null, dte.Debugger.CurrentStackFrame.Locals);
                }
                isStarted = false;
            };

            // 调试开始 or 继续
            debuggerEvents.OnEnterRunMode += (dbgEventReason Reason) =>
            {
                if (!isStarted)
                {
                    //MessageBox.Show("[DebugEvent] 调试开始");
                    isStarted = true;
                    DebugLogUtil.LogDebugStart(dte.Debugger.DebuggedProcesses.Item(1).Name);
                }
                else
                {
                    //MessageBox.Show("[DebugEvent] 调试继续");
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

            watcher = new BreakpointWatcher(dte.Debugger);
            watcher.Start();
        }

        private DTE dte;
        private IVsDebugger debugger;
        private IVsDebugger2 debugger2;
        private IVsDebugger3 debugger3;
        private IVsDebugger4 debugger4;
        private DebuggerEvents debuggerEvents;
        private BuildEvents buildEvents;
        private WindowEvents windowEvents;
        private Breakpoint lastBreakpoint;
        private BreakpointWatcher watcher;
        private string lastDebugTarget;
        private bool isStarted = false;
    }

    public class BreakpointWatcher
    {
        public BreakpointWatcher(EnvDTE.Debugger debugger) 
        {
            this.debugger = debugger;
            cache = makeCache(debugger.Breakpoints);
            checkThread = new System.Threading.Thread(() =>
            {
                while (true)
                {
                    var New = makeCache(debugger.Breakpoints);
                    findDifference(New, cache);
                    cache = New;
                    System.Threading.Thread.Sleep(500);
                }
            });
        }

        public void Start()
        {
            checkThread.Start();
        }

        public void Stop()
        {
             checkThread.Interrupt();
        }

        private void findDifference(BreakpointCache New, BreakpointCache Old)
        {
            if (New == null || Old == null)
            {
                cache = makeCache(debugger.Breakpoints);
                return;
            }
            List<string> common = new List<string>();
            foreach (var key in New.Keys)
            {
                if (!Old.Keys.Contains(key))
                {
                    // TODO: 触发 断点新增 事件。
                    DebugLogUtil.LogBreakpointEvent("add", New[key]);
                }
                else
                {
                    common.Add(key);
                }
            }

            foreach (var key in Old.Keys)
            {
                if (!New.Keys.Contains(key))
                {
                    // TODO: 触发 断点删除 事件
                    DebugLogUtil.LogBreakpointEvent("delete", Old[key]);
                }
            }

            foreach (var key in common)
            {
                BreakpointVO _new = New[key];
                BreakpointVO _old = Old[key];
                bool ne = _new.Enabled;
                bool oe = _old.Enabled;
                if (_new.Enabled != _old.Enabled)
                {
                    // TODO: 触发 断点更改事件
                    if (_new.Enabled)
                    {
                        DebugLogUtil.LogBreakpointEvent("enable", New[key]);
                    }
                    else
                    {
                        DebugLogUtil.LogBreakpointEvent("disable", New[key]);
                    }
                }
                string _nc = _new.Condition;
                string _oc = _old.Condition;
                if (!(_new.Condition + "").Equals(_old.Condition + ""))
                {
                    // TODO: 触发 断点更改事件
                    DebugLogUtil.LogBreakpointEvent("changeCondition", New[key]);
                }
            }
        }

        private BreakpointCache makeCache(Breakpoints bps)
        {
            BreakpointCache result = new BreakpointCache();
            if (bps != null)
            {
                foreach (Breakpoint bp in bps)
                {
                    if (bp.Tag == null || bp.Tag.Equals(""))
                    {
                        bp.Tag = getNextTag();
                    }
                    result[bp.Tag] = new BreakpointVO(bp);
                }
            }
            else
            {
                return null;
            }
            return result;
        }

        private string getNextTag()
        {
            return string.Format("bp#{0}@{1}", ++nextIndex, DateTime.Now.ToLongTimeString());
        }

        private static int nextIndex = 0;
        private BreakpointCache cache;
        private System.Threading.Thread checkThread;
        private EnvDTE.Debugger debugger;
    }

    public class BreakpointVO
    {
        public BreakpointVO(Breakpoint bp)
        {
            Tag = bp.Tag;
            Enabled = bp.Enabled;
            Condition = bp.Condition;
            ConditionType = bp.ConditionType;
            CurrentHits = bp.CurrentHits;
            File = bp.File;
            FileColumn = bp.FileColumn;
            FileLine = bp.FileLine;
            FunctionName = bp.FunctionName;
            LocationType = bp.LocationType;
        }
        public string Tag;
        public bool Enabled;
        public string Condition;
        public dbgBreakpointConditionType ConditionType;
        public int CurrentHits;
        public string File;
        public int FileColumn;
        public int FileLine;
        public string FunctionName;
        public dbgBreakpointLocationType LocationType;
    }

    public class BreakpointCache : Dictionary<string, BreakpointVO> { }
}
