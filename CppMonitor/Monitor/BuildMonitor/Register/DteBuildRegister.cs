using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanjingUniversity.CppMonitor.Monitor.BuildMonitor.Register
{
    class DteBuildRegister
    {
        private BuildEvents buildEvents;

        private Boolean hasRegister=false;

        #region 单例 dteBuildRegister
        private static DteBuildRegister _Self;

        public static DteBuildRegister dteBuildRegister
        {
            get
            {
                if (_Self == null)
                {
                    _Self=new DteBuildRegister();
                }
                return _Self;
            }
        }
        #endregion

        private DteBuildRegister()
        {
            DTE dte = (DTE)ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE.DTE));
            if (dte != null)
            {
                buildEvents = dte.Events.BuildEvents;
            }
        }

        public Boolean Register()
        {
            if (buildEvents != null && !hasRegister)
            {
                buildEvents.OnBuildBegin += new _dispBuildEvents_OnBuildBeginEventHandler(OnBuildBegin);
                buildEvents.OnBuildDone += new _dispBuildEvents_OnBuildDoneEventHandler(OnBuildDone);
                hasRegister = true;
                return true;
            }
            return false;
        }

        public Boolean Cancle()
        {
            if (buildEvents != null && hasRegister)
            {
                buildEvents.OnBuildBegin -= new _dispBuildEvents_OnBuildBeginEventHandler(OnBuildBegin);
                buildEvents.OnBuildDone -= new _dispBuildEvents_OnBuildDoneEventHandler(OnBuildDone);
                hasRegister = false;
                return true;
            }
            return false;
        }

        public void OnBuildBegin(vsBuildScope Scope, vsBuildAction Action)
        {
            
        }

        public void OnBuildDone(vsBuildScope Scope, vsBuildAction Action)
        {
            //MessageBox.Show("OnBuildDone(Solution):");
        }

    }
}
