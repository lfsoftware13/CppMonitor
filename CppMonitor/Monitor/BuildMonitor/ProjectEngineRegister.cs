using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.VCProjectEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanjingUniversity.CppMonitor.Monitor.BuildMonitor
{
    class ProjectEngineRegister
    {
        VCProjectEngineEvents vcProjectEvents;

        private Boolean hasRegister=false;

        #region 单例 projectEngineRegister
        private static ProjectEngineRegister _Self;

        public static ProjectEngineRegister projectEngineRegister
        {
            get
            {
                if (_Self == null)
                {
                    _Self = new ProjectEngineRegister();
                }
                return _Self;
            }
        }
        #endregion

        private ProjectEngineRegister()
        {
            DTE dte = (DTE)ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE.DTE));
            if (dte != null)
            {
                vcProjectEvents = dte.Events.GetObject("VCProjectEngineEventsObject") as VCProjectEngineEvents;
            }
        }

        public Boolean Register(){
            if (vcProjectEvents != null && !hasRegister)
            {
                vcProjectEvents.ProjectBuildFinished += new _dispVCProjectEngineEvents_ProjectBuildFinishedEventHandler(ProjectBuildFinished);
                vcProjectEvents.ProjectBuildStarted += new _dispVCProjectEngineEvents_ProjectBuildStartedEventHandler(ProjectBuildStarted);
                hasRegister = true;
                return true;
            }
            return false;
        }

        public Boolean Cancel()
        {
            if (vcProjectEvents != null && hasRegister)
            {
                vcProjectEvents.ProjectBuildFinished -= new _dispVCProjectEngineEvents_ProjectBuildFinishedEventHandler(ProjectBuildFinished);
                vcProjectEvents.ProjectBuildStarted -= new _dispVCProjectEngineEvents_ProjectBuildStartedEventHandler(ProjectBuildStarted);
                hasRegister = false;
                return true;
            }
            return false;
        }

        public void ProjectBuildFinished(object Cfg, int warnings, int errors, bool Cancelled)
        {
            VCConfiguration con = Cfg as VCConfiguration;
        }

        public void ProjectBuildStarted(object Cfg)
        {
            VCConfiguration con = Cfg as VCConfiguration;
        }


    }
}
