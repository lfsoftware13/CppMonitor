using NanjingUniversity.CppMonitor.DAO.imp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanjingUniversity.CppMonitor.DAO.decorator
{
    public abstract class LoggerDecoratorBase : ILoggerDao
    {
        protected ILoggerDao subLogger;
        protected ILoggerDao summaryLogger;
        public LoggerDecoratorBase(ILoggerDao subLogger)
        {
            this.subLogger = subLogger;
            summaryLogger = new SummaryLoggerImpl();
        }

        public override void ensureTableExist()
        {
            subLogger.ensureTableExist();
        }

        public new int returnKeyAfterLogInfo(string Target, List<KeyValuePair<String, Object>> List)
        {
            LogInfo(Target, List);

            int id = -1;
            foreach (KeyValuePair<string, object> paramPair in List)
            {
                if(paramPair.Key == "id")
                {
                    id = (int)paramPair.Value;
                    break;
                }
            }
            return id;
        }

        public new void clearLog()
        {
            subLogger.clearLog();
        }
    }
}
