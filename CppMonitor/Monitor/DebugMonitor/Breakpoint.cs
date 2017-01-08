using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using NanjingUniversity.CppMonitor.Util;
using System.Windows.Forms;

namespace NanjingUniversity.CppMonitor.Monitor.DebugMonitor
{
    public interface IBreakpointHandler
    {
        void OnBreakpointCreated(Breakpoint bp);
        void OnBreakpointRemoved(Breakpoint bp);
        void OnBreakpointEnabled(Breakpoint bp);
        void OnBreakpointDisabled(Breakpoint bp);
        void OnBreakpointModified(Breakpoint bp);
    }

    public class BreakpointWatcher
    { 
        public delegate void BreakpointEvents(Breakpoint bp);

        protected event BreakpointEvents BreakpointCreatedEvents = null;
        protected event BreakpointEvents BreakpointRemovedEvents = null;
        protected event BreakpointEvents BreakpointEnabledEvents = null;
        protected event BreakpointEvents BreakpointDisabledEvents = null;
        protected event BreakpointEvents BreakpointModifiedEvents = null;

        public static BreakpointWatcher Watch(Window window)
        {
            string fileName = window.Document.FullName;
            if (fileName.EndsWith(".cpp") || fileName.EndsWith(".c") || fileName.EndsWith(".h"))
            {
                refreshWindow();
                if (!windowCache.ContainsKey(window.Document.FullName))
                {
                    windowCache[window.Document.FullName] = new BreakpointWatcher(window);
                }
                return windowCache[window.Document.FullName];
            }
            MessageBox.Show(windowCache.Values.Count + "");
            return null;
        }

        public static void Unwatch(Window window)
        {
            refreshWindow();
            if (windowCache.ContainsKey(window.Document.FullName))
            {
                windowCache[window.Document.FullName].StopWatch();
                windowCache[window.Document.FullName] = null;
            }
        }

        /// <summary>
        /// 将每个BreakpoingWatcher保存在正确的位置。防止文件被重命名后watcher
        /// 不在正确的地方，同一个文件可能会被两个watcher监测。
        /// </summary>
        private static void refreshWindow()
        {
            foreach (string key in windowCache.Keys)
            {
                BreakpointWatcher watcher = windowCache[key];
                if (!key.Equals(watcher.document.FullName))
                {
                    windowCache[key] = null;
                    windowCache[watcher.document.FullName] = watcher;
                }
            }
        }

        private static Dictionary<string, BreakpointWatcher> windowCache = new Dictionary<string, BreakpointWatcher>();

        public BreakpointWatcher(Window window)
        {
            this.document = window.Document;
            this.debugger = window.DTE.Debugger;
            fresh(debugger.Breakpoints);
            cacheBps(debugger.Breakpoints);
            checkThread = new System.Threading.Thread(() =>
            {
                while (true)
                {
                    fresh(debugger.Breakpoints);
                    checkNewBps(debugger.Breakpoints);
                    cacheBps(debugger.Breakpoints);
                    System.Threading.Thread.Sleep(500);
                }
            });
            checkThread.Start();
        }

        public void AdviceBreakpointEvents(IBreakpointHandler handler)
        {
            if (this.handler != null)
            {
                UnadviceBreakpointEvents();
            }
            this.handler = handler;
            BreakpointCreatedEvents += handler.OnBreakpointCreated;
            BreakpointRemovedEvents += handler.OnBreakpointRemoved;
            BreakpointEnabledEvents += handler.OnBreakpointEnabled;
            BreakpointDisabledEvents += handler.OnBreakpointDisabled;
            BreakpointModifiedEvents += handler.OnBreakpointModified;
        }

        public void UnadviceBreakpointEvents()
        {
            if (handler != null)
            {
                BreakpointCreatedEvents -= handler.OnBreakpointCreated;
                BreakpointRemovedEvents -= handler.OnBreakpointRemoved;
                BreakpointEnabledEvents -= handler.OnBreakpointEnabled;
                BreakpointDisabledEvents -= handler.OnBreakpointDisabled;
                BreakpointModifiedEvents -= handler.OnBreakpointModified;
                handler = null;
            }
        }
        

        public void StopWatch()
        {
            this.checkThread.Interrupt();
        }

        /// <summary>
        /// 给每一个断点设置一个唯一的Tag以便区分
        /// </summary>
        /// <param name="bps"></param>
        private void fresh(Breakpoints bps)
        {
            foreach (Breakpoint bp in bps)
            {
                // give each breakpoint a unique name to identify
                if (bp.Tag == null || bp.Tag.Equals(""))
                {
                    bp.Tag = StringUtil.GenerateTimeString(8);
                }
            }
        }

        /// <summary>
        /// 检测新断点，并触发相应事件
        /// </summary>
        /// <param name="bps"></param>
        private void checkNewBps(Breakpoints bps)
        {
            foreach (Breakpoint bp in bps)
            {
                if (!cache.ContainsKey(bp.Tag))
                {
                    BreakpointCreatedEvents?.Invoke(bp);
                }
            }
        }

        /// <summary>
        /// 将当前的断点情况保存为快照
        /// </summary>
        /// <param name="bps"></param>
        private void cacheBps(Breakpoints bps)
        {
            cache.Clear();
            foreach (Breakpoint bp in bps)
            {
                cache[bp.Tag] = bp;
            }
        }

        System.Threading.Thread checkThread = null;


        private EnvDTE.Debugger debugger;

        private Dictionary<string, Breakpoint> cache = new Dictionary<string, Breakpoint>();

        private DTE dte;

        private Document document;

        private IBreakpointHandler handler;
    }
    
}
