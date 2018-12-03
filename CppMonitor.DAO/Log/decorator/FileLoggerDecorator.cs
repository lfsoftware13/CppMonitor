using NanjingUniversity.CppMonitor.Util.Common;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace NanjingUniversity.CppMonitor.DAO.decorator
{
    class FileLoggerDecorator : LoggerDecoratorBase
    {
        public FileLoggerDecorator(ILoggerDao subLogger) : base(subLogger)
        {
        }

        public override Boolean LogInfo(string target, List<KeyValuePair<String, Object>> list)
        {
            bool result = false;
            switch (target)
            {
                case "solution_open_event":
                    result = logSolutionOpenEvent(list);
                    break;
                case "file_event":
                    result = logFileEvent(list);
                    break;
                default:
                    break;
            }
            return result && subLogger.LogInfo(target, list);
        }

        private bool logSolutionOpenEvent(List<KeyValuePair<String, Object>> list)
        {
            string action = ConstantCommon.UNKNOWN_SOLUTION_ACTION;
            string solutionName = ConstantCommon.UNKNOWN_SOLUTIONNAME;

            int type = 0;
            foreach (KeyValuePair<String, Object> keyValuePair in list)
            {
                switch (keyValuePair.Key)
                {
                    case "type":
                        type = (int)keyValuePair.Value;
                        break;
                    case "solutionName":
                        solutionName = keyValuePair.Value.ToString();
                        break;
                    default:
                        break;
                }
            }

            action = ((SolutionAction)type).ToString();            

            List<KeyValuePair<String, Object>> summaryParamsList = new List<KeyValuePair<string, object>>();
            summaryParamsList.Add(new KeyValuePair<string, object>("action", action));
            summaryParamsList.Add(new KeyValuePair<string, object>("solutionName", solutionName));

            int id = 0;

            id = summaryLogger.returnKeyAfterLogInfo("summary_info", summaryParamsList);
            list.Add(new KeyValuePair<string, object>("id", id));

            return id > 0;
        }

        private bool logFileEvent(List<KeyValuePair<String, Object>> list)
        {
            int type = 0;
            string projectName = ConstantCommon.UNKNOWN_PROJECTNAME;
            foreach (KeyValuePair<String, Object> keyValuePair in list)
            {
                switch (keyValuePair.Key)
                {
                    case "type":
                        type = (int)keyValuePair.Value;
                        break;
                    case "projectName":
                        projectName = keyValuePair.Value.ToString();
                        break;
                    default:
                        break;
                }
            }

            string action = ConstantCommon.UNKNOWN_FILE_ACTION;

            switch (type)
            {
                case 1:
                    action = "fileAdd";
                    break;
                case 2:
                    action = "fileDel";
                    break;
                case 3:
                    action = "filterAdd";
                    break;
                case 4:
                    action = "filterDel";
                    break;
                default:
                    break;
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
