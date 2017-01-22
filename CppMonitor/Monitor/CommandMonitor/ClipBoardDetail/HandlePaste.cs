﻿using EnvDTE;
using EnvDTE80;
using NanjingUniversity.CppMonitor.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;

namespace NanjingUniversity.CppMonitor.Monitor.CommandMonitor.ClipBoardDetail
{
    class HandlePaste : IHandleClipBoard
    {

        private DTE Dte;

        private Events DteEvents;

        private DocumentEvents DocEvents;

        private List<KeyValuePair<String, object>> list;

        private CommandUtil util;

        public HandlePaste()
        {
            Dte = (DTE)Microsoft.VisualStudio.Shell.Package.
                GetGlobalService(typeof(Microsoft.VisualStudio.Shell.Interop.SDTE));
            DteEvents = Dte.Events;
            DocEvents = DteEvents.DocumentEvents;
            list = new List<KeyValuePair<string, object>>();
            util = new CommandUtil();
        }
        public void handleText(ILoggerDao Logger)
        {
            string ctype = "Text";
            object obj = Clipboard.GetText();
            list.Add(new KeyValuePair<String, object>("Action", "Paste"));
            list.Add(new KeyValuePair<String, object>("PasteType", ctype));
            list.Add(new KeyValuePair<String, object>("PasteTo_Name", Dte.ActiveDocument.Name));
            list.Add(new KeyValuePair<String, object>("Path", Dte.ActiveDocument.Path));
            list.Add(new KeyValuePair<String, object>("Content", (string)obj));
            Logger.LogInfo(list);
        }

        public void handleFileDrop(ILoggerDao Logger)
        {
            MessageBox.Show("paste file");
            string ctype = "FileDrop";
            object obj = Clipboard.GetFileDropList();
            //IDataObject id = Clipboard.GetDataObject();
            StringCollection file_list = (StringCollection)obj;
            //string[] file_names = (string[])obj;
            string path_content = "";
            foreach (string file_name in file_list)
            {
                path_content += file_name + ";";
            }
            list.Add(new KeyValuePair<String, object>("Action", "Paste"));
            list.Add(new KeyValuePair<String, object>("PasteType", ctype));
            list.Add(new KeyValuePair<String, object>("fileFrom_Path", path_content));
            //get the path of paste_to 
            EnvDTE80.DTE2 _applicationObject = (DTE2)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(DTE));
            EnvDTE.Solution solution = _applicationObject.Solution;

            if (solution == null)
            {
            }
            else if (string.IsNullOrEmpty(solution.FullName))
            {
            }
            else
            {
                string path = Path.GetDirectoryName(solution.FullName);
                list.Add(new KeyValuePair<String, object>("Paste_to_Path", path));
            }
            EnvDTE.UIHierarchy solutionExplorer = _applicationObject.ToolWindows.SolutionExplorer;
            object[] items = solutionExplorer.SelectedItems as object[];
            if (items != null)
            {
                EnvDTE.UIHierarchyItem item = items[0] as EnvDTE.UIHierarchyItem;
                string a = item.Name;
                MessageBox.Show(a);
                //EnvDTE.ProjectItem projectItem = item.Object as EnvDTE.ProjectItem; 
                //string path = projectItem.Properties.Item("FullPath").Value.ToString();
                //list.Add(new KeyValuePair<String, object>("Paste_to_Path", path));
            }

            Logger.LogInfo(list);
        }

        public void handleImage(ILoggerDao Logger)
        {
            Bitmap clipboardImage = Clipboard.GetImage() as Bitmap;
            clipboardImage.MakeTransparent();
            string imagePath = Path.GetTempFileName();
            clipboardImage.Save(imagePath);
            MessageBox.Show("paste image!");
        }

        public void handleAudio(ILoggerDao Logger)
        {
            throw new NotImplementedException();
        }

        public void handleVSProjectItem(ILoggerDao Logger)
        {
            MessageBox.Show("VSProjectItem Paste");
            list.Add(new KeyValuePair<String, object>("Action", "Paste"));
            list.Add(new KeyValuePair<String, object>("PasteType", "file_in_VisualStudio"));

            //get the path of paste_to 
            EnvDTE80.DTE2 _applicationObject = (DTE2)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(DTE));
            string thepath = util.GetSelectedProjectPath(_applicationObject);
            MessageBox.Show(thepath);
            EnvDTE.Solution solution = _applicationObject.Solution;

            list.Add(new KeyValuePair<String, object>("Paste_to_Path", thepath));

            Logger.LogInfo(list);

        }
    }
}
