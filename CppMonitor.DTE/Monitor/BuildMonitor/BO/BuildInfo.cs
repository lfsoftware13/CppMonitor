using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanjingUniversity.CppMonitor.Monitor.BuildMonitor.BO
{
    class BuildInfo
    {
        private string _BuildStartTime;
        private string _BuildEndTime;
        private string _SolutionName;
        private string _Content;
        private List<BuildProjectInfo> _Projects;

        public string BuildStartTime
        {
            get
            {
                return _BuildStartTime;
            }
            set
            {
                _BuildStartTime=value;
            }
        }

        public string BuildEndTime
        {
            get
            {
                return _BuildEndTime;
            }
            set
            {
                _BuildEndTime = value;
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

        public string Content
        {
            get
            {
                return _Content;
            }
            set
            {
                _Content = value;
            }
        }

        public List<BuildProjectInfo> Projects
        {
            get
            {
                if (_Projects == null)
                {
                    _Projects = new List<BuildProjectInfo>();
                }
                return _Projects;
            }
            set
            {
                _Projects = value;
            }
        }

    }
}
