using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NanjingUniversity.CppMonitor.DAO;

namespace NanjingUniversity.CppMonitor.Monitor.CommandMonitor
{
    class ILoggerDaoImpl_stub
    {
        ILoggerDao logger;
        private ILoggerDaoImpl_stub()
        {
           if(logger == null){
               logger = LoggerFactory.loggerFactory.getLogger("Command");
           }
        }

        private static ILoggerDaoImpl_stub commandLogger;
        public static ILoggerDaoImpl_stub CommandLogger
        {
            get
            {
                if(commandLogger == null){
                    commandLogger = new ILoggerDaoImpl_stub();
                }
                return commandLogger;
            }
        }

        public Boolean LogText(List<KeyValuePair<String, Object>> list)
        {
            if(logger != null){
                List<KeyValuePair<String, Object>> logParams = new List<KeyValuePair<string, object>>();

                HashSet<string> keyNames = new HashSet<string>() { "Action", "Name", "Path", "Content" ,"Project"};

                foreach (KeyValuePair<string, object> item in list)
                {
                    if (keyNames.Contains(item.Key))
                    {
                        logParams.Add(item);
                        keyNames.Remove(item.Key);
                    }
                }
                foreach (string key in keyNames)
                {
                    logParams.Add(new KeyValuePair<string, object>(key, ""));
                }
                logParams.Add(new KeyValuePair<string,object>("Happentime",DateTime.Now.Ticks.ToString()));
                return logger.LogInfo("text", logParams);
            }
            return false;
        }
        public Boolean LogFile(List<KeyValuePair<String, Object>> list)
        {
            if (logger != null)
            {
                List<KeyValuePair<String, Object>> logParams = new List<KeyValuePair<string, object>>();

                HashSet<string> keyNames = new HashSet<string>() { "Action", "FilePath", "PasteFileType", "PasteTo", "Project" };

                foreach (KeyValuePair<string, object> item in list)
                {
                    if (keyNames.Contains(item.Key))
                    {
                        logParams.Add(item);
                        keyNames.Remove(item.Key);
                    }
                }
                foreach (string key in keyNames)
                {
                    logParams.Add(new KeyValuePair<string, object>(key, ""));
                }
                return logger.LogInfo("file", logParams);
            }
            return false;
        }
    }
}
