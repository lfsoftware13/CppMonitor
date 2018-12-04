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
using NanjingUniversity.CppMonitor.Util.Common;

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
            watcher = new BreakpointWatcher(dte.Debugger);
        }

        public void RegisterEvent()
        {
            Dictionary<string, StringBuilder> debugOutputs = new Dictionary<string, StringBuilder>();
            debuggerEvents.OnEnterBreakMode += (dbgEventReason Reason, ref dbgExecutionAction ExecutionAction) =>
            {
                watcher.watch();

                string breakReason = Reason + "";
                this.lastBreakpoint = dte.Debugger.BreakpointLastHit;
                if (!(Reason == dbgEventReason.dbgEventReasonEndProgram || Reason == dbgEventReason.dbgEventReasonStopDebugging)) lastDebugTarget = dte.Debugger.DebuggedProcesses.Item(1).Name;
                DebugLogUtil.LogDebugBreak(lastDebugTarget, Reason + "", lastBreakpoint, dte.Debugger.CurrentStackFrame.Locals);
            };

            // 调试结束
            debuggerEvents.OnEnterDesignMode += (dbgEventReason Reason) =>
            {
                watcher.watch();

                if (dte.Debugger.DebuggedProcesses.Count < 1)
                {
                    DebugLogUtil.LogDebugExit(lastDebugTarget, Reason + "", null, null);
                }
                else
                {
                    DebugLogUtil.LogDebugExit(this.lastDebugTarget = dte.Debugger.DebuggedProcesses.Item(1).Name, Reason + "", null, dte.Debugger.CurrentStackFrame.Locals);
                }
                isStarted = false;
            };

            // 调试开始 or 继续
            debuggerEvents.OnEnterRunMode += (dbgEventReason Reason) =>
            {
                watcher.watch();

                if (!isStarted)
                {
                    //MessageBox.Show("[DebugEvent] 调试开始");
                    isStarted = true;
                    String s = dte.Debugger.DebuggedProcesses.Item(1).Name;
                    DebugLogUtil.LogDebugStart(this.lastDebugTarget = dte.Debugger.DebuggedProcesses.Item(1).Name);
                }
                else
                {
                    //MessageBox.Show("[DebugEvent] 调试继续");
                    DebugLogUtil.LogDebugContinue(this.lastDebugTarget = dte.Debugger.DebuggedProcesses.Item(1).Name, lastBreakpoint);
                }
            };

            debuggerEvents.OnExceptionNotHandled += (string ExceptionType, string Name, int Code, string Description, ref dbgExceptionAction ExceptionAction) =>
            {
                string exceptionType = ExceptionType;
                string description = Description;
                watcher.watch();

                DebugLogUtil.LogDebugExceptionNotHandled(this.lastDebugTarget = dte.Debugger.DebuggedProcesses.Item(1).Name, ExceptionType, Name, Description, Code, ExceptionAction + "");
            };

            debuggerEvents.OnExceptionThrown += (string ExceptionType, string Name, int Code, string Description, ref dbgExceptionAction ExceptionAction) =>
            {
                watcher.watch();

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
        }

        /// <summary>
        /// 查看当前断点的状态，如果和已经记录的状态存在差异，那么就将对应的差异刷入数据库
        /// </summary>
        public void watch()
        {
             var New = makeCache(debugger.Breakpoints);
             findDifference(New, cache);
             cache = New;
        }

        private void findDifference(BreakpointCache New, BreakpointCache Old)
        {
            List<string> common = new List<string>();

            //确认新增的断点
            if (New != null)
            {
                foreach (var key in New.Keys)
                {
                    if (Old == null || !Old.Keys.Contains(key))
                    {
                        // TODO: 触发 断点新增 事件。
                        DebugLogUtil.LogBreakpointEvent(BreakpointAction.bpAdd.ToString(), New[key]);
                    }
                    else
                    {
                        common.Add(key);
                    }
                }
            }
            
            //确认删除的断点
            if (Old != null)
            {
                foreach (var key in Old.Keys)
                {
                    if (New == null || !New.Keys.Contains(key))
                    {
                        // TODO: 触发 断点删除 事件
                        DebugLogUtil.LogBreakpointEvent(BreakpointAction.bpDelete.ToString(), Old[key]);
                    }
                }
            }
            
            //确认修改的断点
            foreach (var key in common)
            {
                BreakpointVO _new = New[key];
                BreakpointVO _old = Old[key];
                if (_new.Enabled != _old.Enabled)
                {
                    // TODO: 触发 断点更改事件
                    if (_new.Enabled)
                    {
                        DebugLogUtil.LogBreakpointEvent(BreakpointAction.bpEnable.ToString(), New[key]);
                    }
                    else
                    {
                        DebugLogUtil.LogBreakpointEvent(BreakpointAction.bpDisable.ToString(), New[key]);
                    }
                }else if (!(_new.Condition + "").Equals(_old.Condition + ""))
                {
                    // TODO: 触发 断点更改事件
                    DebugLogUtil.LogBreakpointEvent(BreakpointAction.bpChangeCondition.ToString(), New[key]);
                }else if (! _new.Equals(_old))
                {
                    DebugLogUtil.LogBreakpointEvent(BreakpointAction.bpChangeAttri.ToString(), New[key]);
                }
            }
        }

        /// <summary>
        /// 记录并返回当前的断点，给每个断点赋予一个标签
        /// </summary>
        /// <param name="bps"></param>
        /// <returns></returns>
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
        private static BreakpointCache cache;
        private EnvDTE.Debugger debugger;
    }

    public class BreakpointVO
    {
        public BreakpointVO(Breakpoint bp)
        {
            id = -1;
            old_id = -1;

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

        //是否是同一个breakpoint
        public override bool Equals(Object obj)
        {
            if(obj == null || this == null)
            {
                return false;
            }

            BreakpointVO bp = obj as BreakpointVO;
            return bp.Condition == this.Condition && bp.ConditionType == this.ConditionType
                        && bp.File.Equals(this.File)&& bp.FileColumn == this.FileColumn
                        && bp.FileLine == this.FileLine && bp.LocationType == this.LocationType
                        && bp.Enabled == this.Enabled;
        }

        //是否由原breakpoint修改而来的breakpoint
        public bool hasOld(Object obj)
        {
            if (obj == null || this == null)
            {
                return false;
            }

            BreakpointVO bp = obj as BreakpointVO;
            return bp.File.Equals(this.File) && bp.FileColumn == this.FileColumn
                        && bp.FileLine == this.FileLine && bp.LocationType == this.LocationType;
        }

        public int id;
        public int old_id;
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
