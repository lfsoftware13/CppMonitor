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

        public HandlePaste()
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
                list.Add(new KeyValuePair<String, object>("Action", "Paste"));
                list.Add(new KeyValuePair<String, object>("Type", ctype));
                list.Add(new KeyValuePair<String, object>("Name", Dte.ActiveDocument.Name));
                list.Add(new KeyValuePair<String, object>("Path", Dte.ActiveDocument.Path));
                list.Add(new KeyValuePair<String, object>("Content", (string)obj));
                list.Add(new KeyValuePair<String, object>("Project", doc == null ? "" : doc.ProjectItem.ContainingProject.Name));
            }
            else
            {
                list.Add(new KeyValuePair<String, object>("Action", "Paste"));
                list.Add(new KeyValuePair<String, object>("Type", ctype));
                list.Add(new KeyValuePair<String, object>("Content", (string)obj));
            }
            ILoggerDaoImpl_stub.CommandLogger.LogText(list);
        }

        public void handleFileDrop()
        {
            //MessageBox.Show("paste file");
            string ctype = "File";
            object obj = Clipboard.GetFileDropList();
            StringCollection file_list = (StringCollection)obj;
            list.Add(new KeyValuePair<String, object>("Action", "Paste"));
            list.Add(new KeyValuePair<String, object>("Type", ctype));
            list.Add(new KeyValuePair<String, object>("PasteFileType", "out_visualstudio"));            

            string[] filePaths = new string[file_list.Count];
            file_list.CopyTo(filePaths,0);
            list.Add(new KeyValuePair<String, object>("FilePath", String.Join(",",filePaths)));

            //get the path of paste_to 
            EnvDTE80.DTE2 _applicationObject = (DTE2)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(DTE));
            string thepath = CommandUtil.GetSelectedProjectPath(_applicationObject);
            list.Add(new KeyValuePair<String, object>("PasteTo", thepath));
            list.Add(new KeyValuePair<String, object>("Project", CommandUtil.GetActiveProjects(Dte as DTE2)));
            ILoggerDaoImpl_stub.CommandLogger.LogFile(list);
        }

        public void handleImage()
        {
            //Bitmap clipboardImage = Clipboard.GetImage() as Bitmap;
            //clipboardImage.MakeTransparent();
            //string imagePath = Path.GetTempFileName();
            //clipboardImage.Save(imagePath);
            //MessageBox.Show("paste image!");
        }

        public void handleAudio()
        {
            //throw new NotImplementedException();
        }

        public void handleVSProjectItem()
        {
            //MessageBox.Show("VSProjectItem Paste");
            list.Add(new KeyValuePair<String, object>("Action", "Paste"));
            list.Add(new KeyValuePair<String, object>("Type", "File"));
            list.Add(new KeyValuePair<String, object>("PasteFileType", "in_visualstudio"));   

            //get the path of paste_to 
            EnvDTE80.DTE2 _applicationObject = (DTE2)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(DTE));
            string thepath = CommandUtil.GetSelectedProjectPath(_applicationObject);
            //MessageBox.Show(thepath);
            list.Add(new KeyValuePair<String, object>("PasteTo", thepath));
            list.Add(new KeyValuePair<String, object>("Project", CommandUtil.GetActiveProjects(Dte as DTE2)));
            ILoggerDaoImpl_stub.CommandLogger.LogFile(list);
        }
    }
}
