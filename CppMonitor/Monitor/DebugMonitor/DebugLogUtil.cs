using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NanjingUniversity.CppMonitor.DAO;
using NanjingUniversity.CppMonitor.DAO.imp;
using EnvDTE;
using EnvDTE100;

namespace NanjingUniversity.CppMonitor.Monitor.DebugMonitor
{
    class DebugLogUtil
    {
        static DebugLogUtil()
        {
            logger = LoggerFactory.loggerFactory.getLogger("Debug");
        }

        public static int LogDebugStart(string debugTarget)
        {
            var param = new List<KeyValuePair<string, object>>();
            param.Add(new KeyValuePair<string, object>("run_type", "start"));
            param.Add(new KeyValuePair<string, object>("debug_target", debugTarget));
            return logger.returnKeyAfterLogInfo("debug_run", param);
        }

        public static int LogDebugContinue(string debugTarget, Breakpoint bp)
        {
            var debugParam = new List<KeyValuePair<string, object>>();
            debugParam.Add(new KeyValuePair<string, object>("debug_target", debugTarget));
            debugParam.Add(new KeyValuePair<string, object>("run_type", "continue"));

            if (bp != null) {
                int bid = LogBreakpoint(new BreakpointVO(bp));
                debugParam.Add(new KeyValuePair<string, object>("breakpoint_last_hit", bid));
            }

            return logger.returnKeyAfterLogInfo("debug_run", debugParam);
        }

        public static int LogDebugBreak(string debugTarget, string breakReason, Breakpoint bp, Expressions vars)
        {
            var debugParam = new List<KeyValuePair<string, object>>();
            debugParam.Add(new KeyValuePair<string, object>("debug_target", debugTarget));
            debugParam.Add(new KeyValuePair<string, object>("break_reason", breakReason));

            if (bp != null) 
            {
                int bid = LogBreakpoint(new BreakpointVO(bp));
                debugParam.Add(new KeyValuePair<string, object>("breakpoint_last_hit", bid));
            }

            int debugId = logger.returnKeyAfterLogInfo("debug_break", debugParam);

            // 记录本地变量值
            if (vars != null)
            {
                foreach (Expression expression in vars)
                {
                    var varParam = new List<KeyValuePair<string, object>>();
                    varParam.Add(new KeyValuePair<string, object>("debug_id", debugId));
                    varParam.Add(new KeyValuePair<string, object>("name", expression.Name));
                    varParam.Add(new KeyValuePair<string, object>("value", expression.Value));
                    logger.LogInfo("local_variable", varParam);
                }
            }

            return debugId;
        }

        public static int LogDebugExceptionThrown(string debugTarget, string type, string name, string description, int code, string action)
        {
            var debugParam = new List<KeyValuePair<string, object>>();
            debugParam.Add(new KeyValuePair<string, object>("debug_target", debugTarget));

            int eid = LogException(type, name, description, code, action);
            debugParam.Add(new KeyValuePair<string, object>("exception_id", eid));

            return logger.returnKeyAfterLogInfo("debug_exception_thrown", debugParam);
        }

        public static int LogDebugExceptionNotHandled(string debugTarget, string type, string name, string description, int code, string action)
        {
            var debugParam = new List<KeyValuePair<string, object>>();
            debugParam.Add(new KeyValuePair<string, object>("debug_target", debugTarget));

            int eid = LogException(type, name, description, code, action);
            debugParam.Add(new KeyValuePair<string, object>("exception_id", eid));

            return logger.returnKeyAfterLogInfo("debug_exception_not_handled", debugParam);
        }

        public static void LogLocalVarialbles(int debugId, Expressions vars)
        {
            foreach (Expression expression in vars)
            {
                var debugParam = new List<KeyValuePair<string, object>>();
                debugParam.Add(new KeyValuePair<string, object>("debug_id", debugId));
                debugParam.Add(new KeyValuePair<string, object>("name", expression.Name));
                debugParam.Add(new KeyValuePair<string, object>("value", expression.Value));
            }
        }

        public static int LogBreakpoint(BreakpointVO bp)
        {
            var bpParam = new List<KeyValuePair<string, object>>();
            bpParam.Add(new KeyValuePair<string, object>("tag", bp.Tag));
            bpParam.Add(new KeyValuePair<string, object>("condition", bp.Condition));
            bpParam.Add(new KeyValuePair<string, object>("condition_type", bp.ConditionType));
            bpParam.Add(new KeyValuePair<string, object>("current_hits", bp.CurrentHits));
            bpParam.Add(new KeyValuePair<string, object>("file", bp.File));
            bpParam.Add(new KeyValuePair<string, object>("file_column", bp.FileColumn));
            bpParam.Add(new KeyValuePair<string, object>("file_line", bp.FileLine));
            bpParam.Add(new KeyValuePair<string, object>("function_name", bp.FunctionName));
            bpParam.Add(new KeyValuePair<string, object>("location_type", bp.LocationType));
            bpParam.Add(new KeyValuePair<string, object>("enabled", bp.Enabled));
            return logger.returnKeyAfterLogInfo("breakpoint", bpParam);
        }

        public static int LogException(string type, string name, string description, int code, string action)
        {
            var bpParam = new List<KeyValuePair<string, object>>();
            bpParam.Add(new KeyValuePair<string, object>("type", type));
            bpParam.Add(new KeyValuePair<string, object>("name", name));
            bpParam.Add(new KeyValuePair<string, object>("description", description));
            bpParam.Add(new KeyValuePair<string, object>("code", code));
            bpParam.Add(new KeyValuePair<string, object>("action", action));
            return logger.returnKeyAfterLogInfo("exception", bpParam);
        }

        public static int LogBreakpointEvent(string modification, BreakpointVO bp)
        {
            int breakpointId = LogBreakpoint(bp);
            var param = new List<KeyValuePair<string, object>>();
            param.Add(new KeyValuePair<string, object>("modification", modification));
            param.Add(new KeyValuePair<string, object>("breakpoint_id", breakpointId));
            return logger.returnKeyAfterLogInfo("breakpoint_event", param);
        }

        private static ILoggerDao logger;
    }
}
