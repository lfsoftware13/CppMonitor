using NanjingUniversity.CppMonitor.DAO;
using NanjingUniversity.CppMonitor.Util.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanjingUniversity.CppMonitor.Monitor.FileMonitor
{
    class FileLogUtil
    {
        private static ILoggerDao logger;

        static FileLogUtil()
        {
            if(logger == null){
                logger = LoggerFactory.loggerFactory.getLogger("File");
            }
        }
        /*
         * type: 1表示增加；2表示删除
         */
        public static void logFileEvent(int type,string fileName,string projectName,string targetFile =null){
            if(logger != null){
                List<KeyValuePair<String, Object>> fileEventParams = new List<KeyValuePair<string, object>>();
                fileEventParams.Add(new KeyValuePair<String,Object>("type",type));
                fileEventParams.Add(new KeyValuePair<String, Object>("fileName", fileName));
                fileEventParams.Add(new KeyValuePair<String, Object>("projectName", projectName));
                if(targetFile == null){
                    targetFile = "";//处理空值
                }
                fileEventParams.Add(new KeyValuePair<String, Object>("targetFile", targetFile));
                logger.LogInfo("file_event",fileEventParams);
            }
        }
        /*
         * type:  1表示打开  2表示关闭
         */ 
        public static void logSolutionOpenEvent(string solutionFullPath, int type, string info=null, string targetFolder = null)
        {
            if(logger != null){

                List<KeyValuePair<String, Object>> solutionEventParams = new List<KeyValuePair<string, object>>();
                solutionEventParams.Add(new KeyValuePair<String, Object>("type", type));
                solutionEventParams.Add(new KeyValuePair<String, Object>("fullPath", solutionFullPath));

                string solutionName = ConstantCommon.UNKNOWN_SOLUTIONNAME;

                int lindex = solutionFullPath.LastIndexOf("\\");
                int diff = solutionFullPath.LastIndexOf(".")-lindex;
                solutionName = solutionFullPath.Substring(lindex+1, diff-1);
                solutionEventParams.Add(new KeyValuePair<String, Object>("solutionName", solutionName));
                if (info == null)
                {
                    info = "";//处理空值
                }
                solutionEventParams.Add(new KeyValuePair<String, Object>("info", info));
                if (targetFolder == null)
                {
                    targetFolder = "";//处理空值
                }
                solutionEventParams.Add(new KeyValuePair<String, Object>("targetFolder", targetFolder));
                logger.LogInfo("solution_open_event", solutionEventParams);
            }
        }
    }
}
