using EnvDTE;
using EnvDTE80;
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

        private CommandUtil util;
        public HandleCut()
        {
            Dte = (DTE)Microsoft.VisualStudio.Shell.Package.
                GetGlobalService(typeof(Microsoft.VisualStudio.Shell.Interop.SDTE));
            DteEvents = Dte.Events;
            DocEvents = DteEvents.DocumentEvents;
            list = new List<KeyValuePair<string, object>>();
            util = new CommandUtil();
        }

        public void handleText(DAO.ILoggerDao Logger)
        {
            string ctype = "Text";
            object obj = Clipboard.GetText();
            list.Add(new KeyValuePair<String, object>("Action", "Cut"));
            list.Add(new KeyValuePair<String, object>("CutType", ctype));
            list.Add(new KeyValuePair<String, object>("CutFrom_Name", Dte.ActiveDocument.Name));
            list.Add(new KeyValuePair<String, object>("Path", Dte.ActiveDocument.Path));
            list.Add(new KeyValuePair<String, object>("Content", (string)obj));
            Logger.LogInfo(list);

        }

        public void handleFileDrop(DAO.ILoggerDao Logger)
        {
            string ctype = "FileDrop";
            object obj = Clipboard.GetFileDropList();
            StringCollection file_list = (StringCollection)obj;
            string path_content = "";
            foreach (string file_name in file_list)
            {
                path_content += file_name + "\n";
            }
            list.Add(new KeyValuePair<String, object>("Action", "Cut"));
            list.Add(new KeyValuePair<String, object>("CopyType", ctype));
            list.Add(new KeyValuePair<String, object>("file_Path", path_content));
            Logger.LogInfo(list);
        }

        public void handleImage(DAO.ILoggerDao Logger)
        {
            throw new NotImplementedException();
        }

        public void handleAudio(DAO.ILoggerDao Logger)
        {
            throw new NotImplementedException();
        }

        public void handleVSProjectItem(ILoggerDao Logger)
        {
            list.Add(new KeyValuePair<String, object>("Action", "Cut_in_VisualStudio"));
            list.Add(new KeyValuePair<String, object>("CutType", "file"));
            EnvDTE80.DTE2 _applicationObject = (DTE2)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(DTE));
            List<string> Ie = util.GetSelectedFilePaths(_applicationObject);
            if (Ie != null)
            {
                int i = 1;
                foreach (string path in Ie)
                {
                    string text = System.IO.File.ReadAllText(@path);
                    list.Add(new KeyValuePair<String, object>("CutPath" + i, path));
                    list.Add(new KeyValuePair<String, object>("Cut_Content" + i, text));
                    i++;
                }
            }
            //if (Ie != null)
            //{
            //    IEnumerator<string> Enum = Ie.GetEnumerator();
            //    MessageBox.Show("???");
            //    int i = 1;
            //    while (Enum.MoveNext())
            //    {
            //        string path = Enum.Current;
            //        string text = System.IO.File.ReadAllText(@path);
            //        list.Add(new KeyValuePair<String, object>("CutPath" + i, path));
            //        list.Add(new KeyValuePair<String, object>("Cut_Content" + i, text));
            //        i++;
            //    }
            //}
            Logger.LogInfo(list);
        }
    }
}
