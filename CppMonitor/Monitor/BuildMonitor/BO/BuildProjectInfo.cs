using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanjingUniversity.CppMonitor.Monitor.BuildMonitor.BO
{
    class BuildProjectInfo
    {
        public string BuildProjectStartTime
        {
            get
            {
                return BuildProjectStartTime;
            }
            set
            {
                BuildProjectStartTime = value;
            }
        }

        public string BuildProjectEndTime
        {
            get
            {
                return BuildProjectEndTime;
            }
            set
            {
                BuildProjectEndTime = value;
            }
        }

        public string ConfigurationName
        {
            get
            {
                return ConfigurationName;
            }
            set
            {
                ConfigurationName = value;
            }
        }

        public string ConfigurationType
        {
            get
            {
                return ConfigurationType;
            }
            set
            {
                ConfigurationType = value;
            }
        }

        public string ProjectName
        {
            get
            {
                return ProjectName;
            }
            set
            {
                ProjectName = value;
            }
        }

        public string CommandArguments
        {
            get
            {
                return CommandArguments;
            }
            set
            {
                CommandArguments = value;
            }

        }

        public string RunCommand
        {
            get
            {
                return RunCommand;
            }
            set
            {
                RunCommand = value;
            }
        }

        public string BuildLogFile
        {
            get
            {
                return BuildLogFile;
            }
            set
            {
                BuildLogFile = value;
            }
        }

        public string BuildLogContent
        {
            get
            {
                return BuildLogContent;
            }
            set
            {
                BuildLogContent = value;
            }
        }

        public string CompilerCommand
        {
            get
            {
                return CompilerCommand;
            }
            set
            {
                CompilerCommand = value;
            }
        }

        public string LinkCommand
        {
            get
            {
                return LinkCommand;
            }
            set
            {
                LinkCommand = value;
            }
        }


    }
}
