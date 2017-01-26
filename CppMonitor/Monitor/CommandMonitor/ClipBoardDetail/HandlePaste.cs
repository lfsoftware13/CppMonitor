using EnvDTE;
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
            Document doc = Dte.ActiveDocument;
            if (doc != null)
            {
                list.Add(new KeyValuePair<String, object>("Action", "Paste"));
                list.Add(new KeyValuePair<String, object>("Type", ctype));
                list.Add(new KeyValuePair<String, object>("Name", Dte.ActiveDocument.Name));
                list.Add(new KeyValuePair<String, object>("Path", Dte.ActiveDocument.Path));
                list.Add(new KeyValuePair<String, object>("Content", (string)obj));
            }
            else
            {
                list.Add(new KeyValuePair<String, object>("Action", "Paste"));
                list.Add(new KeyValuePair<String, object>("Type", ctype));
                list.Add(new KeyValuePair<String, object>("Content", (string)obj));
            }
            
            Logger.LogInfo(list);
        }

        public void handleFileDrop(ILoggerDao Logger)
        {
            //MessageBox.Show("paste file");
            string ctype = "File";
            object obj = Clipboard.GetFileDropList();
            StringCollection file_list = (StringCollection)obj;
            list.Add(new KeyValuePair<String, object>("Action", "Paste"));
            list.Add(new KeyValuePair<String, object>("Type", ctype));
            list.Add(new KeyValuePair<String, object>("PasteFileType", "out_visualstudio"));            
            
            //int i = 1;
            string allpath = "";
            foreach (string file_path in file_list)
            {
                allpath += file_path + ";";
                //i++;
            }
            list.Add(new KeyValuePair<String, object>("FilePath", allpath));
            //list.Add(new KeyValuePair<String, object>("Path_number", i - 1));
            //get the path of paste_to 
            EnvDTE80.DTE2 _applicationObject = (DTE2)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(DTE));
            string thepath = util.GetSelectedProjectPath(_applicationObject);
            list.Add(new KeyValuePair<String, object>("PasteTo", thepath));

            //EnvDTE.UIHierarchy solutionExplorer = _applicationObject.ToolWindows.SolutionExplorer;
            //object[] items = solutionExplorer.SelectedItems as object[];
            //if (items != null)
            //{
            //    EnvDTE.UIHierarchyItem item = items[0] as EnvDTE.UIHierarchyItem;
            //    string a = item.Name;
            //    MessageBox.Show(a);     //filter name
            //}

            Logger.LogInfo(list);
        }

        public void handleImage(ILoggerDao Logger)
        {
            Bitmap clipboardImage = Clipboard.GetImage() as Bitmap;
            clipboardImage.MakeTransparent();
            string imagePath = Path.GetTempFileName();
            clipboardImage.Save(imagePath);
            //MessageBox.Show("paste image!");
        }

        public void handleAudio(ILoggerDao Logger)
        {
            throw new NotImplementedException();
        }

        public void handleVSProjectItem(ILoggerDao Logger)
        {
            //MessageBox.Show("VSProjectItem Paste");
            list.Add(new KeyValuePair<String, object>("Action", "Paste"));
            list.Add(new KeyValuePair<String, object>("Type", "File"));
            list.Add(new KeyValuePair<String, object>("PasteFileType", "in_visualstudio"));   

            //get the path of paste_to 
            EnvDTE80.DTE2 _applicationObject = (DTE2)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(DTE));
            string thepath = util.GetSelectedProjectPath(_applicationObject);
            //MessageBox.Show(thepath);
            list.Add(new KeyValuePair<String, object>("Paste_to_Path", thepath));

            Logger.LogInfo(list);

        }
    }
}
