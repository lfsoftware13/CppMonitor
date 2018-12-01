using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NanjingUniversity.CppMonitor.DAO;
using NanjingUniversity.CppMonitor.DAO.imp;
using EnvDTE;
using Microsoft.VisualStudio.VCProjectEngine;
using Microsoft.VisualStudio.Shell;

namespace NanjingUniversity.CppMonitor.Monitor.DebugMonitor
{
    class DebugLogUtil
    {
        static DebugLogUtil()
        {
            logger = LoggerFactory.loggerFactory.getLogger("Debug");
        }

        public static SolutionConfiguration GetCurrentProjectConfiguration()
        {
            DTE dte = (DTE)ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE.DTE));
            Solution sol = dte.Solution;
            if (sol != null)
            {
                if (sol.SolutionBuild != null)
                {
                    return sol.SolutionBuild.ActiveConfiguration;
                }
                
            }
            return null;
        }

        private static int LogDebug(string debugTarget, Breakpoint bp, string type)
        {
            var debugParam = new List<KeyValuePair<string, object>>();
            debugParam.Add(new KeyValuePair<string, object>("debug_target", debugTarget));
            debugParam.Add(new KeyValuePair<string, object>("type", type));

            if (bp != null)
            {
                int bid = LogBreakpoint(new BreakpointVO(bp));
                debugParam.Add(new KeyValuePair<string, object>("breakpoint_current_hit", bid));
            }

            SolutionConfiguration con = GetCurrentProjectConfiguration();
            string configName = "";
            if (con != null)
            {
                configName = con.Name;
            }
            debugParam.Add(new KeyValuePair<string, object>("config_name", configName));

            return logger.returnKeyAfterLogInfo("debug_info", debugParam);
        }

        public static int LogDebugStart(string debugTarget)
        {
            return LogDebug(debugTarget, null, "start");
        }

        public static int LogDebugContinue(string debugTarget, Breakpoint bp)
        {
            return LogDebug(debugTarget, bp, "continue");
        }

        public static int LogDebugExit(string debugTarget, string breakReason, Breakpoint bp, Expressions vars)
        {
            return LogDebugBreak(debugTarget, breakReason, bp, vars, true);
        }

        public static int LogDebugBreak(string debugTarget, string breakReason, Breakpoint bp, Expressions vars)
        {
            return LogDebugBreak(debugTarget, breakReason, bp, vars, false);
        }

            private static int LogDebugBreak(string debugTarget, string breakReason, Breakpoint bp, Expressions vars, bool isExit)
        {
            var debugParam = new List<KeyValuePair<string, object>>();
            debugParam.Add(new KeyValuePair<string, object>("debug_target", debugTarget));
            debugParam.Add(new KeyValuePair<string, object>("break_reason", breakReason));
            debugParam.Add(new KeyValuePair<string, object>("type", isExit ? "exit" : "break"));

            if (bp != null)
            {
                int bid = LogBreakpoint(new BreakpointVO(bp));
                debugParam.Add(new KeyValuePair<string, object>("breakpoint_current_hit", bid));
            }

            SolutionConfiguration con = GetCurrentProjectConfiguration();
            string configName = "";
            if (con != null)
            {
                configName = con.Name;
            }
            debugParam.Add(new KeyValuePair<string, object>("config_name", configName));

            int debugId = logger.returnKeyAfterLogInfo("debug_info", debugParam);

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
            debugParam.Add(new KeyValuePair<string, object>("type", "exception_thrown"));

            int eid = LogException(type, name, description, code, action);
            debugParam.Add(new KeyValuePair<string, object>("exception_id", eid));

            SolutionConfiguration con = GetCurrentProjectConfiguration();
            string configName = "";
            if (con != null)
            {
                configName = con.Name;
            }
            debugParam.Add(new KeyValuePair<string, object>("config_name", configName));

            return logger.returnKeyAfterLogInfo("debug_info", debugParam);
        }

        public static int LogDebugExceptionNotHandled(string debugTarget, string type, string name, string description, int code, string action)
        {
            var debugParam = new List<KeyValuePair<string, object>>();
            debugParam.Add(new KeyValuePair<string, object>("debug_target", debugTarget));
            debugParam.Add(new KeyValuePair<string, object>("type", "exception_not_handled"));

            int eid = LogException(type, name, description, code, action);
            debugParam.Add(new KeyValuePair<string, object>("exception_id", eid));

            SolutionConfiguration con = GetCurrentProjectConfiguration();
            string configName = "";
            if (con != null)
            {
                configName = con.Name;
            }
            debugParam.Add(new KeyValuePair<string, object>("config_name", configName));

            return logger.returnKeyAfterLogInfo("debug_info", debugParam);
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
