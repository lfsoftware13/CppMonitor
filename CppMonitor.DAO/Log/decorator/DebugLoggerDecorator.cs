using NanjingUniversity.CppMonitor.Util.Common;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace NanjingUniversity.CppMonitor.DAO.decorator
{
    class DebugLoggerDecorator : LoggerDecoratorBase
    {

        public DebugLoggerDecorator(ILoggerDao subLogger) : base(subLogger)
        {

        }

        public override Boolean LogInfo(string target, List<KeyValuePair<String, Object>> list)
        {
            bool result = true;
            switch (target)
            {
                case "debug_info":
                    result = logDebugInfo(list);
                    break;
                case "breakpoint_event":
                    result = logBreakpointEvent(list);
                    break;
                default:
                    break;
            }
            return result && subLogger.LogInfo(target, list);
        }

        private string parseProjectNameFromDebugTarget(string debug_target)
        {
            int indexOfLastDot = debug_target.LastIndexOf(".");
            int indexOfLastBackslash = debug_target.LastIndexOf("\\");

            if (indexOfLastDot > indexOfLastBackslash && indexOfLastBackslash >= 0)
            {
                return debug_target.Substring(indexOfLastBackslash + 1, indexOfLastDot - indexOfLastBackslash - 1);
            }
            return ConstantCommon.UNKNOWN_PROJECTNAME;
        }

        private bool logDebugInfo(List<KeyValuePair<String, Object>> list)
        {
            string action = ConstantCommon.UNKNOWN_DEBUG_ACTION;
            string projectName = ConstantCommon.UNKNOWN_PROJECTNAME;

            foreach (KeyValuePair<String, Object> keyValuePair in list)
            {
                switch (keyValuePair.Key)
                {
                    case "type":
                        action = keyValuePair.Value.ToString();
                        break;
                    case "debug_target":
                        projectName = parseProjectNameFromDebugTarget(keyValuePair.Value.ToString());
                        break;
                    default:
                        break;
                }
            }
            List<KeyValuePair<String, Object>> summaryParamsList = new List<KeyValuePair<string, object>>();
            summaryParamsList.Add(new KeyValuePair<string, object>("action", action));
            summaryParamsList.Add(new KeyValuePair<string, object>("projectName", projectName));

            int id = 0;
            id = summaryLogger.returnKeyAfterLogInfo("summary_info", summaryParamsList);
            list.Add(new KeyValuePair<string, object>("id", id));
            return id > 0;
        }

        private bool logBreakpointEvent(List<KeyValuePair<String, Object>> list)
        {
            string action = ConstantCommon.UNKNOWN_DEBUG_ACTION;
            foreach (KeyValuePair<String, Object> keyValuePair in list)
            {
                switch (keyValuePair.Key)
                {
                    case "modification":
                        action = keyValuePair.Value.ToString();
                        break;
                    default:
                        break;
                }
            }

            List<KeyValuePair<String, Object>> summaryParamsList = new List<KeyValuePair<string, object>>();
            summaryParamsList.Add(new KeyValuePair<string, object>("action", action));

            int id = 0;
            id = summaryLogger.returnKeyAfterLogInfo("summary_info", summaryParamsList);
            list.Add(new KeyValuePair<string, object>("id", id));
            return id > 0;
        }
    }
}
