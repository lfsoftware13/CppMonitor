using NanjingUniversity.CppMonitor.Util.Common;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace NanjingUniversity.CppMonitor.DAO.decorator
{
    class ContentLoggerDecorator : LoggerDecoratorBase
    {
        public ContentLoggerDecorator(ILoggerDao subLogger) : base(subLogger)
        {
        }

        public override Boolean LogInfo(string target, List<KeyValuePair<String, Object>> list)
        {
            bool result = false;
            result = logContentInfo(list);
            return result && subLogger.LogInfo(target, list);
        }

        private bool logContentInfo(List<KeyValuePair<String, Object>> list)
        {
            string action = ConstantCommon.UNKNOWN_ACTION;
            string projectName = ConstantCommon.UNKNOWN_PROJECTNAME;

            foreach (KeyValuePair<String, Object> keyValuePair in list)
            {
                switch (keyValuePair.Key)
                {
                    case "Operation":
                        action = "content" + keyValuePair.Value;
                        break;
                    case "Project":
                        projectName = keyValuePair.Value.ToString();
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

    }
}
