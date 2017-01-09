using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanjingUniversity.CppMonitor.Monitor.BuildMonitor.BO
{
    class BuildInfo
    {
        public string BuildStartTime
        {
            get
            {
                return BuildStartTime;
            }
            set
            {
                BuildStartTime=value;
            }
        }

        public string BuildEndTime
        {
            get
            {
                return BuildEndTime;
            }
            set
            {
                BuildEndTime = value;
            }
        }

        public string Content
        {
            get
            {
                return Content;
            }
            set
            {
                Content = value;
            }
        }

        public List<BuildProjectInfo> Projects
        {
            get
            {
                return Projects;
            }
            set
            {
                Projects = value;
            }
        }

    }
}
