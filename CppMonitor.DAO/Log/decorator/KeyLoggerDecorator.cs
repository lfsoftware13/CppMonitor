using NanjingUniversity.CppMonitor.Util.Common;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace NanjingUniversity.CppMonitor.DAO.decorator
{
    class KeyLoggerDecorator : LoggerDecoratorBase
    {
        public KeyLoggerDecorator(ILoggerDao subLogger):base(subLogger)
        {
        }

        public override Boolean LogInfo(string target, List<KeyValuePair<String, Object>> list)
        {
            bool result = false;
            switch (target)
            {
                case "key_info":
                    result = logKeyInfo(list);
                    break;
                default:
                    break;
            }
            return result && subLogger.LogInfo(target,list);
        }

        private bool logKeyInfo(List<KeyValuePair<String, Object>> list)
        {
            string actionSource = ConstantCommon.UNKNOWN_ACTION;
            string projectName = ConstantCommon.UNKNOWN_PROJECTNAME;

            foreach (KeyValuePair<String,object> keyValuePair in list)
            {
                switch (keyValuePair.Key)
                {
                    case "source":
                        actionSource = (string)keyValuePair.Value;
                        break;
                    case "projectName":
                        projectName = (string)keyValuePair.Value;
                        break;
                    default:
                        break;
                }
            }

            List<KeyValuePair<String, Object>> summaryParamlist = new List<KeyValuePair<string, object>>();
            summaryParamlist.Add(new KeyValuePair<string, object>("action",actionSource));
            summaryParamlist.Add(new KeyValuePair<string, object>("projectName", projectName));

            int id = summaryLogger.returnKeyAfterLogInfo("summary_info", summaryParamlist);

            list.Add(new KeyValuePair<string, object>("id",id));

            return id > 0;
        }

    }
}
