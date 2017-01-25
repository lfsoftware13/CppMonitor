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

        public static bool LogDebugStart(string debugTarget)
        {
            var param = new List<KeyValuePair<string, object>>();
            param.Add(new KeyValuePair<string, object>("run_type", "start"));
            return logger.LogInfo("debug_run", param);
        }

        public static bool LogDebugContinue(string debugTarget, Breakpoint bp)
        {
            var debugParam = new List<KeyValuePair<string, object>>();
            debugParam.Add(new KeyValuePair<string, object>("debug_target", debugTarget));
            debugParam.Add(new KeyValuePair<string, object>("run_type", "continue"));

            if (bp != null) {
                int bid = LogBreakpoint(bp);
                debugParam.Add(new KeyValuePair<string, object>("breakpoint_last_hit", bid));
            }

            return logger.LogInfo("debug_run", debugParam);
        }

        public static bool LogDebugBreak(string debugTarget, string breakReason, Breakpoint bp)
        {
            var debugParam = new List<KeyValuePair<string, object>>();
            debugParam.Add(new KeyValuePair<string, object>("debug_target", debugTarget));
            debugParam.Add(new KeyValuePair<string, object>("break_reason", breakReason));

            if (bp != null) 
            {
                int bid = LogBreakpoint(bp);
                debugParam.Add(new KeyValuePair<string, object>("breakpoint_last_hit", bid));
            }

            return logger.LogInfo("debug_break", debugParam);
        }

        public static bool LogDebugExceptionThrown(string debugTarget, string type, string name, string description, int code, string action)
        {
            var debugParam = new List<KeyValuePair<string, object>>();
            debugParam.Add(new KeyValuePair<string, object>("debug_target", debugTarget));

            int eid = LogException(type, name, description, code, action);
            debugParam.Add(new KeyValuePair<string, object>("exception_id", eid));

            return logger.LogInfo("debug_exception_thrown", debugParam);
        }

        public static bool LogDebugExceptionNotHandled(string debugTarget, string type, string name, string description, int code, string action)
        {
            var debugParam = new List<KeyValuePair<string, object>>();
            debugParam.Add(new KeyValuePair<string, object>("debug_target", debugTarget));

            int eid = LogException(type, name, description, code, action);
            debugParam.Add(new KeyValuePair<string, object>("exception_id", eid));

            return logger.LogInfo("debug_exception_not_handled", debugParam);
        }

        public static int LogBreakpoint(Breakpoint bp)
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
            return logger.returnKeyAfterLogInfo("breakpoint", bpParam);
        }

        private static ILoggerDao logger;
    }
}
