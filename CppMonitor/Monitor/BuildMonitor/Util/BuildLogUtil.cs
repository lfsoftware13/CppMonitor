using NanjingUniversity.CppMonitor.DAO;
using NanjingUniversity.CppMonitor.Monitor.BuildMonitor.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NanjingUniversity.CppMonitor.Monitor.BuildMonitor.Util
{
    class BuildLogUtil
    {
        static ILoggerDao logger;

        static BuildLogUtil()
        {
            if(logger == null){
                logger = LoggerFactory.loggerFactory.getLogger("Build");
            }
        }

        public static Boolean LogBuildInfo(BuildInfo info)
        {
            if(logger != null && info != null){
                List<KeyValuePair<String, Object>> buildInfoParams = new List<KeyValuePair<string, object>>();
                buildInfoParams.Add(new KeyValuePair<string,object>("buildstarttime",info.BuildStartTime));
                buildInfoParams.Add(new KeyValuePair<string, object>("buildendtime", info.BuildEndTime));
                buildInfoParams.Add(new KeyValuePair<string, object>("content", info.Content));
                logger.LogInfo("build_info", buildInfoParams);

                foreach(BuildProjectInfo bpi in info.Projects){
                    List<KeyValuePair<String, Object>> buildProjectInfoParams = new List<KeyValuePair<string, object>>();
                    buildProjectInfoParams.Add(new KeyValuePair<string, object>("buildid", info.BuildStartTime));
                    buildProjectInfoParams.Add(new KeyValuePair<string, object>("buildstarttime", bpi.BuildProjectStartTime));
                    buildProjectInfoParams.Add(new KeyValuePair<string, object>("buildendtime", bpi.BuildProjectEndTime));
                    buildProjectInfoParams.Add(new KeyValuePair<string, object>("projectname", bpi.ProjectName));
                    buildProjectInfoParams.Add(new KeyValuePair<string, object>("configurationname", bpi.ConfigurationName));
                    buildProjectInfoParams.Add(new KeyValuePair<string, object>("configurationtype", bpi.ConfigurationType));
                    buildProjectInfoParams.Add(new KeyValuePair<string, object>("runcommand", bpi.RunCommand));
                    buildProjectInfoParams.Add(new KeyValuePair<string, object>("commandarguments", bpi.CommandArguments));
                    buildProjectInfoParams.Add(new KeyValuePair<string, object>("buildlogcontent", bpi.BuildLogContent));
                    buildProjectInfoParams.Add(new KeyValuePair<string, object>("buildlogfile", bpi.BuildLogFile));
                    buildProjectInfoParams.Add(new KeyValuePair<string, object>("compilercommand", bpi.CompilerCommand));
                    buildProjectInfoParams.Add(new KeyValuePair<string, object>("linkcommand", bpi.LinkCommand));
                    logger.LogInfo("build_project_info", buildProjectInfoParams);
                }
                return true;
            }
            return false;
        }

        public static Boolean LogMessage(string Message)
        {
            return true;
        }


    }
}
