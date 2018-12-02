using CppMonitor.Model;
using NanjingUniversity.CppMonitor.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanjingUniversity.CppMonitor.MEFMonitor.Util
{
    class KeyEventLogUtil
    {
        private static ILoggerDao logger;

        static KeyEventLogUtil()
        {
            if (logger == null)
            {
                logger = LoggerFactory.loggerFactory.getLogger("Key");
            }
        }
        
        public static void logKeyEvent(string source, string filePath, string projectName, string key, KeyModifier keyModifier)
        {
            if (logger != null)
            {
                List<KeyValuePair<String, Object>> keyEventParams = new List<KeyValuePair<string, object>>();
                keyEventParams.Add(new KeyValuePair<String, Object>("key", key));
                keyEventParams.Add(new KeyValuePair<String, Object>("modifier", keyModifier.ToString()));
                keyEventParams.Add(new KeyValuePair<String, Object>("source", source));
                keyEventParams.Add(new KeyValuePair<String, Object>("projectName", projectName));
                keyEventParams.Add(new KeyValuePair<String, Object>("filePath", filePath));

                logger.LogInfo("key_info", keyEventParams);
            }
        }
    }
}
