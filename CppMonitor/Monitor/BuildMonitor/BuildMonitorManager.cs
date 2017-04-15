using Microsoft.VisualStudio.VCProjectEngine;
using NanjingUniversity.CppMonitor.Monitor.BuildMonitor;
using NanjingUniversity.CppMonitor.Monitor.BuildMonitor.BO;
using NanjingUniversity.CppMonitor.Monitor.BuildMonitor.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanjingUniversity.CppMonitor.Monitor
{
    class BuildMonitorManager
    {

        private BuildInfo _CurrentBuild;
        public BuildInfo CurrentBuild
        {
            get
            {
                return _CurrentBuild;
            }
            set
            {
                _CurrentBuild = value;
            }
        }


        public void StartBuild()
        {
            CreateBuildInfo();
            if (_CurrentBuild != null)
            {
                EnvDTE.DTE dte = (EnvDTE.DTE)Microsoft.VisualStudio.Shell.ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE.DTE));
                if (dte != null && dte.Solution != null)
                {
                    EnvDTE.Property pro = dte.Solution.Properties.Item("Name");
                    if (pro != null)
                    {
                        _CurrentBuild.SolutionName = pro.Value as string;
                    }
                }
                _CurrentBuild.BuildStartTime = DateTime.Now.ToString();
            }
        }

        public void EndBuild()
        {
            if (_CurrentBuild != null)
            {
                _CurrentBuild.BuildEndTime = DateTime.Now.ToString();
                string content = BuildMonitorUtil.GetOrderBuildOutput();
                _CurrentBuild.Content = content;
                BuildLogUtil.LogBuildInfo(_CurrentBuild);
                CleanBuildInfo();
            }
        }

        public void CreateBuildInfo()
        {
            _CurrentBuild = new BuildInfo();
        }

        public void CleanBuildInfo()
        {
            _CurrentBuild = null ;
        }

        public void StartBuildVCProject(VCConfiguration con)
        {
            if (con == null || _CurrentBuild==null)
            {
                return ;
            }

            BuildProjectInfo info = new BuildProjectInfo();
            info.BuildProjectStartTime = DateTime.Now.ToString();
            _CurrentBuild.Projects.Add(info);
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

            EnvDTE.DTE dte = (EnvDTE.DTE)Microsoft.VisualStudio.Shell.ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE.DTE));
            if (dte != null && dte.Solution != null)
            {
                EnvDTE.Property pro = dte.Solution.Properties.Item("Name");
                if (pro != null)
                {
                    info.SolutionName = pro.Value as string;
                }
            }

        }

        public void EndBuildVCProject(VCConfiguration con)
        {
            if (con == null || _CurrentBuild==null)
            {
                return ;
            }

            VCProject pro=con.project as VCProject;

            if(pro==null||pro.Name==null){
                return ;
            }

            foreach (BuildProjectInfo info in _CurrentBuild.Projects)
            {
                if(info.ProjectName==null){
                    continue;
                }
                if (info.ProjectName.Equals(pro.Name))
                {
                    info.BuildProjectEndTime = DateTime.Now.ToString();
                    if (info.BuildLogFile == null)
                    {
                        if (con.BuildLogFile != null)
                        {
                            info.BuildLogFile = con.Evaluate(con.BuildLogFile);
                        }
                    }
                    if (info.BuildLogFile != null)
                    {
                        if (!File.Exists(info.BuildLogFile))
                        {
                            info.BuildLogFile = con.Evaluate("$(ProjectDir)")+info.BuildLogFile;
                        }
                        info.BuildLogContent = BuildMonitorUtil.ReadFile(info.BuildLogFile);

                        info.CompilerCommand = GetCLCommand(info.BuildLogContent);
                        info.LinkCommand = GetLinkCommand(info.BuildLogContent);

                    }
                }
            }
        }

        string GetCLCommand(string content)
        {
            string command="";
            if(content==null){
                return command;
            }
            string[] strs = content.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            foreach(string str in strs){
                string line = str.Trim();
                if (line.Length > 3 && "cl ".Equals(line.Substring(0, 3)) && (".cpp".Equals(line.Substring(line.Length - 4, 4)) || ".cxx".Equals(line.Substring(line.Length - 4, 4)) || ".c".Equals(line.Substring(line.Length - 2, 2)) || ".C".Equals(line.Substring(line.Length - 2, 2))))
                {
                    command = line;
                }
            }
            return command;
        }

        string GetLinkCommand(string content)
        {
            string command = "";
            if (content == null)
            {
                return command;
            }
            string[] strs = content.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            foreach (string str in strs)
            {
                string line = str.Trim();
                if (line.Length>6 && "\"/OUT:".Equals(line.Substring(0, 6)) && ".obj".Equals(line.Substring(line.Length-4, 4)))
                {
                    command = line;
                }
            }
            return command;
        }

        

    }
}
