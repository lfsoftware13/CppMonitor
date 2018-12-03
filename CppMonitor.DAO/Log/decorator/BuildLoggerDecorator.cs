using NanjingUniversity.CppMonitor.Util.Common;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace NanjingUniversity.CppMonitor.DAO.decorator
{
    class BuildLoggerDecorator : LoggerDecoratorBase
    {
        public BuildLoggerDecorator(ILoggerDao subLogger) : base(subLogger)
        {
        }

        public override Boolean LogInfo(string target, List<KeyValuePair<String, Object>> list)
        {
            bool result = false;
            switch (target)
            {
                case "build_info":
                    result = logBuildInfo(list);
                    break;
                case "build_project_info":
                    result = logBuildProjectInfo(list);
                    break;
                default:
                    break;
            }
            return result && subLogger.LogInfo(target, list);
        }

        private bool logBuildInfo(List<KeyValuePair<String, Object>> list)
        {
            string action = ConstantCommon.UNKNOWN_SOLUTION_ACTION;
            string solutionName = ConstantCommon.UNKNOWN_SOLUTIONNAME;

            action = "buildSolution";

            foreach (KeyValuePair<String, Object> keyValuePair in list)
            {
                if (keyValuePair.Key.Equals("solutionname"))
                {
                    solutionName = keyValuePair.Value.ToString();
                }
            }

            List<KeyValuePair<String, Object>> summaryParamsList = new List<KeyValuePair<string, object>>();
            summaryParamsList.Add(new KeyValuePair<string, object>("action", action));
            summaryParamsList.Add(new KeyValuePair<string, object>("solutionName", solutionName));

            int id = 0;

            id = summaryLogger.returnKeyAfterLogInfo("summary_info", summaryParamsList);
            list.Add(new KeyValuePair<string, object>("id", id));

            return id > 0;
        }

        private bool logBuildProjectInfo(List<KeyValuePair<String, Object>> list)
        {
            string action = ConstantCommon.UNKNOWN_FILE_ACTION;
            string projectName = ConstantCommon.UNKNOWN_PROJECTNAME;

            action = "buildProject";
            foreach (KeyValuePair<String, Object> keyValuePair in list)
            {
                if (keyValuePair.Key.Equals("projectname"))
                {
                    projectName = keyValuePair.Value.ToString();
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

    }
}
