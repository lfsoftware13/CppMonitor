using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CpppMonitor.DTE.Monitor.BuildMonitor.Model;
using Newtonsoft.Json;
using NanjingUniversity.CppMonitor.Util.Common;

namespace NanjingUniversity.CppMonitor.Monitor.BuildMonitor.Util
{
    class BuildMonitorUtil
    {
        public static string GetCurrentErrorListContent()
        {
            List<ErrorListItem> errorListItems = new List<ErrorListItem>();

            DTE2 dte2 = PersistentObjectManager.dte2;

            ErrorItems errorItems = dte2.ToolWindows.ErrorList.ErrorItems;
            for (int i = 1; i <= errorItems.Count; i++ )
            {
                ErrorListItem errorListItem = new ErrorListItem();

                ErrorItem errorItem = errorItems.Item(i);
                //根据OBSIDE中的注释，开发者表示部分情况下 下面的转换过程会出现引用无效的问题
                try
                {
                    errorListItem.Column = errorItem.Column;
                    errorListItem.Line = errorItem.Line;
                    errorListItem.FileName = errorItem.FileName;
                    errorListItem.Project = errorItem.Project;
                    errorListItem.Description = errorItem.Description;
                    errorListItem.ErrorLevel = (int)errorItem.ErrorLevel;
                }
                catch (Exception)
                {
                    errorListItem = null;
                }

                if (errorListItem != null)
                {
                    errorListItems.Add(errorListItem);
                }

            }

            return JsonConvert.SerializeObject(errorListItems);
        }

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
            catch(FileNotFoundException)
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
