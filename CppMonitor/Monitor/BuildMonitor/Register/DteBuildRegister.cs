using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.VCProjectEngine;
using NanjingUniversity.CppMonitor.Monitor.BuildMonitor.Util;
using NanjingUniversity.CppMonitor.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            String projectFilePath = Path.Combine(CopyUtil.backupBuildDirPath, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
            CopyUtil.backupSolutionFile(projectFilePath);

            BuildMonitorManager manager = BuildBindEvent.Manager;

            DTE dte = (DTE)ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE.DTE));
            if (dte.Solution != null && dte.Solution.Projects!=null)
            {
                foreach (Project project in dte.Solution.Projects)
                {
                    VCProject pro = project.Object as VCProject;
                    if (pro != null)
                    {
                        foreach (VCConfiguration con in pro.Configurations)
                        {
                            SetBuildLogSwitch(con);
                            CheckLogFile(con);
                        }
                    }
                    
                }
            }


            if (manager != null)
            {
                manager.StartBuild();
            }
        }

        BackgroundWorker worker;
        public void OnBuildDone(vsBuildScope Scope, vsBuildAction Action)
        {
            worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(EndBuildAsync);
            worker.RunWorkerAsync();
        }

        void EndBuildAsync(object sender, DoWorkEventArgs e)
        {
            System.Threading.Thread.Sleep(2000);
            BuildMonitorManager manager = BuildBindEvent.Manager;
            if (manager != null)
            {
                manager.EndBuild();
            }
        }

        void SetBuildLogSwitch(VCConfiguration con)
        {
            if (con != null)
            {
                IVCRulePropertyStorage cl = con.Rules.Item("CL");
                cl.SetPropertyValue("SuppressStartupBanner", "false");
                string clAddOption = cl.GetEvaluatedPropertyValue("AdditionalOptions");
                string[] clstr = clAddOption.Split(' ');
                for (int i = 0; i < clstr.Count(); i++)
                {
                    if ("/nologo".Equals(clstr[i]))
                    {
                        clstr[i] = "";
                    }
                }
                string clnew = string.Join(" ", clstr);
                cl.SetPropertyValue("AdditionalOptions", clnew);

                IVCRulePropertyStorage link = con.Rules.Item("Link");
                link.SetPropertyValue("SuppressStartupBanner", "false");
                string linkAddOption = link.GetEvaluatedPropertyValue("AdditionalOptions");
                string[] linkstr = linkAddOption.Split(' ');
                for (int i = 0; i < linkstr.Count(); i++)
                {
                    if ("/nologo".Equals(linkstr[i]))
                    {
                        linkstr[i] = "";
                    }
                }
                string linknew = string.Join(" ", linkstr);
                link.SetPropertyValue("AdditionalOptions", linknew);
            }
        }

        const string ProjectPath = "$(ProjectDir)";
        const string DefaultLogPath = "$(IntDir)$(MSBuildProjectName).log";

        void CheckLogFile(VCConfiguration con)
        {

            if (con == null)
            {
                return ;
            }

            if (con.BuildLogFile == null)
            {
                con.BuildLogFile = DefaultLogPath;
            }
            string path = con.Evaluate(con.BuildLogFile);

            FileStream fs = null;
            try
            {
                fs = new FileStream(path, FileMode.OpenOrCreate);
            }
            catch
            {
                
            }

            if (fs == null)
            {
                try
                {
                    path = con.Evaluate(ProjectPath) + path;
                    fs = new FileStream(path, FileMode.OpenOrCreate);
                }
                catch
                {

                }
                if (fs == null)
                {
                    con.BuildLogFile = DefaultLogPath;
                }

            }

            if (fs != null)
            {
                fs.Close();
            }

        }


    }
}
