using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanjingUniversity.CppMonitor.Monitor.BuildMonitor.BO
{
    class BuildProjectInfo
    {
        private string _BuildProjectStartTime;
        private string _BuildProjectEndTime;
        private string _ConfigurationName;
        private string _ConfigurationType;
        private string _SolutionName;
        private string _ProjectName;
        private string _CommandArguments;
        private string _RunCommand;
        private string _BuildLogFile;
        private string _BuildLogContent;
        private string _CompilerCommand;
        private string _LinkCommand;

        public string BuildProjectStartTime
        {
            get
            {
                return _BuildProjectStartTime;
            }
            set
            {
                _BuildProjectStartTime = value;
            }
        }

        public string BuildProjectEndTime
        {
            get
            {
                return _BuildProjectEndTime;
            }
            set
            {
                _BuildProjectEndTime = value;
            }
        }

        public string ConfigurationName
        {
            get
            {
                return _ConfigurationName;
            }
            set
            {
                _ConfigurationName = value;
            }
        }

        public string ConfigurationType
        {
            get
            {
                return _ConfigurationType;
            }
            set
            {
                _ConfigurationType = value;
            }
        }

        public string SolutionName
        {
            get
            {
                return _SolutionName;
            }
            set
            {
                _SolutionName = value;
            }
        }

        public string ProjectName
        {
            get
            {
                return _ProjectName;
            }
            set
            {
                _ProjectName = value;
            }
        }

        public string CommandArguments
        {
            get
            {
                return _CommandArguments;
            }
            set
            {
                _CommandArguments = value;
            }

        }

        public string RunCommand
        {
            get
            {
                return _RunCommand;
            }
            set
            {
                _RunCommand = value;
            }
        }

        public string BuildLogFile
        {
            get
            {
                return _BuildLogFile;
            }
            set
            {
                _BuildLogFile = value;
            }
        }

        public string BuildLogContent
        {
            get
            {
                return _BuildLogContent;
            }
            set
            {
                _BuildLogContent = value;
            }
        }

        public string CompilerCommand
        {
            get
            {
                return _CompilerCommand;
            }
            set
            {
                _CompilerCommand = value;
            }
        }

        public string LinkCommand
        {
            get
            {
                return _LinkCommand;
            }
            set
            {
                _LinkCommand = value;
            }
        }


    }
}
