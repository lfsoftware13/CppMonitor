using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell.Interop;
using NanjingUniversity.CppMonitor.DAO;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NanjingUniversity.CppMonitor.Monitor.CommandMonitor.ClipBoardDetail
{
    class HandleCut:IHandleClipBoard
    {
        private DTE Dte;

        private Events DteEvents;

        private DocumentEvents DocEvents;

        private List<KeyValuePair<String, object>> list;

        public HandleCut()
        {
            Dte = (DTE)Microsoft.VisualStudio.Shell.Package.
                GetGlobalService(typeof(Microsoft.VisualStudio.Shell.Interop.SDTE));
            DteEvents = Dte.Events;
            DocEvents = DteEvents.DocumentEvents;
            list = new List<KeyValuePair<string, object>>();
        }

        public void handleText()
        {
            string ctype = "Text";
            object obj = Clipboard.GetText();
            Document doc = Dte.ActiveDocument;
            if (doc != null)
            {
                list.Add(new KeyValuePair<String, object>("Action", "Cut"));
                list.Add(new KeyValuePair<String, object>("Type", ctype));
                list.Add(new KeyValuePair<String, object>("Name", doc.Name));
                list.Add(new KeyValuePair<String, object>("Path", doc.Path));
                list.Add(new KeyValuePair<String, object>("Content", (string)obj));
                list.Add(new KeyValuePair<String, object>("Project", doc == null ? "" : doc.ProjectItem.ContainingProject.Name));
            }
            else
            {
                list.Add(new KeyValuePair<String, object>("Action", "Cut"));
                list.Add(new KeyValuePair<String, object>("Type", ctype));
                list.Add(new KeyValuePair<String, object>("Content", (string)obj));
            }
            ILoggerDaoImpl_stub.CommandLogger.LogText(list);
        }

        public void handleFileDrop()
        {
            string ctype = "File";
            object obj = Clipboard.GetFileDropList();
            StringCollection file_list = (StringCollection)obj;
            string path_content = "";
            foreach (string file_name in file_list)
            {
                path_content += file_name + "\n";
            }
            list.Add(new KeyValuePair<String, object>("Action", "Cut"));
            list.Add(new KeyValuePair<String, object>("Type", ctype));
            list.Add(new KeyValuePair<String, object>("FilePath", path_content));
            ILoggerDaoImpl_stub.CommandLogger.LogFile(list);
        }

        public void handleImage()
        {
            //throw new NotImplementedException();
        }

        public void handleAudio()
        {
            //throw new NotImplementedException();
        }

        public void handleVSProjectItem()
        {
            list.Add(new KeyValuePair<String, object>("Action", "Cut"));
            list.Add(new KeyValuePair<String, object>("Type", "File"));
            EnvDTE80.DTE2 _applicationObject = (DTE2)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(DTE));
            List<string> Ie = CommandUtil.GetSelectedFilePaths(_applicationObject);
            if (Ie != null)
            {
                list.Add(new KeyValuePair<String, object>("FilePath", string.Join(",",Ie)));

            }
            list.Add(new KeyValuePair<String, object>("Project", CommandUtil.GetActiveProjects(Dte as DTE2)));
            ILoggerDaoImpl_stub.CommandLogger.LogFile(list);
        }
    }
}
