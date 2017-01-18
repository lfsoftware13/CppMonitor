using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanjingUniversity.CppMonitor.Monitor.BuildMonitor.Util
{
    class BuildMonitorUtil
    {

        public static string GetOrderBuildOutput()
        {
            DTE2 dte = (DTE2)ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE.DTE));
            Windows windows = dte.Windows;
            OutputWindowPanes panes = dte.ToolWindows.OutputWindow.OutputWindowPanes;
            OutputWindowPane buildPane = null;
            try
            {
                if (buildPane == null)
                {
                    buildPane = panes.Item("Build Order");
                }
            }
            catch
            {

            }

            try
            {
                if (buildPane == null)
                {
                    buildPane = panes.Item("生成顺序");
                }
            }
            catch
            {

            }
            string content = "";
            if (buildPane != null)
            {
                TextDocument doc = buildPane.TextDocument;
                EditPoint2 strtPt = (EditPoint2)doc.StartPoint.CreateEditPoint();
                content = strtPt.GetText(doc.EndPoint);

                if ("".Equals(content))
                {
                    System.Threading.Thread.Sleep(3000);
                    content = strtPt.GetText(doc.EndPoint);
                }

            }
            else
            {
                content = "ERROR: Can't find the Build Order Output Pane. ";
            }
            return content;
        }

        public static string GetBuildOutput()
        {
            DTE2 dte = (DTE2)ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE.DTE));
            Windows windows = dte.Windows;
            OutputWindowPanes panes = dte.ToolWindows.OutputWindow.OutputWindowPanes;
            OutputWindowPane buildPane = null;
            try
            {
                if (buildPane == null)
                {
                    buildPane = panes.Item("Build");
                }
            }
            catch
            {

            }

            try
            {
                if (buildPane == null)
                {
                    buildPane = panes.Item("生成");
                }
            }
            catch
            {

            }
            string content = "";
            if (buildPane != null)
            {
                TextDocument doc = buildPane.TextDocument;
                EditPoint2 strtPt = (EditPoint2)doc.StartPoint.CreateEditPoint();
                content = strtPt.GetText(doc.EndPoint);
            }
            else
            {
                content = "ERROR: Can't find the Build Output Pane. ";
            }
            return content;
        }

        public static string ReadFile(string path)
        {
            FileStream fs=null;
            string content="";
            try
            {
                fs = new FileStream(path, FileMode.Open);
            }
            catch(FileNotFoundException e)
            {
                return "Can't Find the Log File in Path : "+path;
            }
            catch
            {
                return "Open File Error";
            }
            if (fs != null)
            {
                StreamReader reader = new StreamReader(fs);
                content=reader.ReadToEnd();
                reader.Close();
                fs.Close();
            }

            return content;
        }


    }
}
