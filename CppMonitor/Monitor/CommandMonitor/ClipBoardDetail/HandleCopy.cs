using EnvDTE;
using EnvDTE80;
using NanjingUniversity.CppMonitor.DAO;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NanjingUniversity.CppMonitor.Monitor.CommandMonitor.ClipBoardDetail
{
    class HandleCopy : IHandleClipBoard
    {
        private DTE Dte;

        private Events DteEvents;

        private DocumentEvents DocEvents;

        private List<KeyValuePair<String, object>> list;

        public HandleCopy()
        {
            // TODO: Complete member initialization
            Dte = (DTE)Microsoft.VisualStudio.Shell.Package.
                GetGlobalService(typeof(Microsoft.VisualStudio.Shell.Interop.SDTE));
            DteEvents = Dte.Events;
            DocEvents = DteEvents.DocumentEvents;
            list = new List<KeyValuePair<string, object>>();
        }
        public void handleText(ILoggerDao Logger)
        {
            string ctype = "Text";
            object obj = Clipboard.GetText();
            list.Add(new KeyValuePair<String, object>("Action", "Copy"));
            list.Add(new KeyValuePair<String, object>("CopyType", ctype));
            list.Add(new KeyValuePair<String, object>("CopyFrom_Name", Dte.ActiveDocument.Name));
            list.Add(new KeyValuePair<String, object>("Path", Dte.ActiveDocument.Path));
            list.Add(new KeyValuePair<String, object>("Content", (string)obj));
            Logger.LogInfo(list);

        }

        public void handleFileDrop(ILoggerDao Logger)
        {
            string ctype = "FileDrop";
            object obj = Clipboard.GetFileDropList();
            StringCollection file_list = (StringCollection)obj;
            string path_content = "";
            foreach (string file_name in file_list)
            {
                path_content += file_name + "\n";
            }
            list.Add(new KeyValuePair<String, object>("Action", "Copy"));
            list.Add(new KeyValuePair<String, object>("CopyType", ctype));
            list.Add(new KeyValuePair<String, object>("file_Path", path_content));
            Logger.LogInfo(list);
        }

        public void handleImage(ILoggerDao Logger)
        {
            string ctype = "Image";
            object obj = Clipboard.GetImage();
        }

        public void handleAudio(ILoggerDao Logger)
        {
            string ctype = "Audio";
            object obj = Clipboard.GetAudioStream();
        }

        public void handleVSProjectItem(ILoggerDao Logger)
        {
            list.Add(new KeyValuePair<String, object>("Action", "Copy_in_VisualStudio"));
            list.Add(new KeyValuePair<String, object>("CopyType", "file"));
            EnvDTE80.DTE2 _applicationObject = (DTE2)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(DTE));
            EnvDTE.UIHierarchy solutionExplorer = _applicationObject.ToolWindows.SolutionExplorer;
            object[] items = solutionExplorer.SelectedItems as object[];

            if(items != null){
                int i = 1;
                foreach(object it in items){
                    EnvDTE.UIHierarchyItem item = it as EnvDTE.UIHierarchyItem;
                    EnvDTE.ProjectItem projectItem = item.Object as EnvDTE.ProjectItem;
                    string path = projectItem.Properties.Item("FullPath").Value.ToString();
                    list.Add(new KeyValuePair<String, object>("CopyPath"+i, path));
                    i++;
                }
            }
            Logger.LogInfo(list);
        }
    }
}
