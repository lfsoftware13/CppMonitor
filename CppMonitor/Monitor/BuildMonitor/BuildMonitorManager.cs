using Microsoft.VisualStudio.VCProjectEngine;
using NanjingUniversity.CppMonitor.Monitor.BuildMonitor;
using NanjingUniversity.CppMonitor.Monitor.BuildMonitor.BO;
using NanjingUniversity.CppMonitor.Monitor.BuildMonitor.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanjingUniversity.CppMonitor.Monitor
{
    class BuildMonitorManager
    {

        public BuildInfo CurrentBuild
        {
            get
            {
                return CurrentBuild;
            }
            set
            {
                CurrentBuild = value;
            }
        }


        public void StartBuild()
        {
            CleanBuildInfo();
            CurrentBuild.BuildStartTime = DateTime.Now.ToString();
        }

        public void EndBuild()
        {
            CurrentBuild.BuildEndTime = DateTime.Now.ToString();
            string content = BuildMonitorUtil.GetOrderBuildOutput();
            CurrentBuild.Content = content;
            LogBuildInfo();
        }

        public void CleanBuildInfo()
        {
            CurrentBuild = new BuildInfo();
        }

        public Boolean LogBuildInfo()
        {
            return true;
        }


        public void StartBuildVCProject(VCConfiguration con)
        {
            if (con == null)
            {
                return ;
            }

            BuildProjectInfo info = new BuildProjectInfo();
            info.BuildProjectStartTime = DateTime.Now.ToString();
            if (con.BuildLogFile != null)
            {
                info.BuildLogFile = con.Evaluate(con.BuildLogFile);
            }

            if(con.DebugSettings!=null){
                VCDebugSettings deb=con.DebugSettings as VCDebugSettings;
                if(deb!=null){
                    info.CommandArguments=con.Evaluate(deb.CommandArguments);
                    info.RunCommand = con.Evaluate(deb.Command);
                }
            }
            
            info.ConfigurationName = con.Name;
            info.ConfigurationType = Enum.GetName(typeof(ConfigurationTypes), con.ConfigurationType);
            if (con.project != null)
            {
                VCProject pro=con.project as VCProject;
                if(pro!=null){
                    info.ProjectName = pro.Name;
                }
            }
        }

        public void EndBuildVCProject(VCConfiguration con)
        {
            if (con == null)
            {
                return ;
            }

            VCProject pro=con.project as VCProject;

            if(pro==null||pro.Name==null){
                return ;
            }

            foreach (BuildProjectInfo info in CurrentBuild.Projects)
            {
                if(info.ProjectName==null){
                    continue;
                }
                if (info.ProjectName.Equals(pro.Name))
                {
                    info.BuildProjectEndTime = DateTime.Now.ToString();
                    if (info.BuildLogFile != null)
                    {
                        info.BuildLogContent = BuildMonitorUtil.ReadFile(info.BuildLogFile);
                    }
                }
            }
        }

    }
}
