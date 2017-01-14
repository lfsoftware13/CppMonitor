using EnvDTE;
using NanjingUniversity.CppMonitor.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Specialized;

namespace NanjingUniversity.CppMonitor.Monitor.CommandMonitor.ClipBoardDetail
{
    class HandlePaste : IHandleClipBoard
    {

        private DTE Dte;

        private Events DteEvents;

        private DocumentEvents DocEvents;

        private List<KeyValuePair<String, object>> list;

        public HandlePaste()
        {
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
            IDataObject id = Clipboard.GetDataObject();
            string[] li = id.GetFormats();
            MessageBox.Show(li[0]+"!@!@!"+li[1]);
            StringCollection file_list = (StringCollection)obj;
            //string[] file_names = (string[])obj;
            string path_content = "";
            foreach (string file_name in file_list)
            {
                path_content += file_name + "\n";
            }
            list.Add(new KeyValuePair<String, object>("Action", "Paste"));
            list.Add(new KeyValuePair<String, object>("PasteType", ctype));
            list.Add(new KeyValuePair<String, object>("fileFrom_Path", path_content));
            Logger.LogInfo(list);
        }

        public void handleImage(ILoggerDao Logger)
        {
            throw new NotImplementedException();
        }

        public void handleAudio(ILoggerDao Logger)
        {
            throw new NotImplementedException();
        }
    }
}
