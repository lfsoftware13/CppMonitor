using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanjingUniversity.CppMonitor.Util.Util
{
    public class ProjectUtil
    {
        public static string getProjectNameFromDoc(Document Doc , string defaultProjectName = null)
        {
            if (Doc == null)
            {
                return defaultProjectName;
            }
            if (Doc.ProjectItem == null)
            {
                return defaultProjectName;
            }
            ProjectItem projectItem = Doc.ProjectItem;
            if (projectItem.ContainingProject == null)
            {
                return defaultProjectName;
            }

            string projectName = projectItem.ContainingProject.Name;
            return projectName;
        }
    }
}
