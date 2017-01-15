using EnvDTE;
using NanjingUniversity.CppMonitor.DAO;
using NanjingUniversity.CppMonitor.DAO.imp;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NanjingUniversity.CppMonitor.Monitor.CommandMonitor.ClipBoardDetail;
using System.Collections.Specialized;
using EnvDTE80;

namespace NanjingUniversity.CppMonitor.Monitor.CommandMonitor
{
    class HandleClipBoard
    {
        private IHandleClipBoard handle;

        public HandleClipBoard()
        {

        }

        public void handleCopy(ILoggerDao Logger)
        {
            handle = new HandleCopy();
            handleCPC(Logger);
            //List<KeyValuePair<String, object>> list = new List<KeyValuePair<string, object>>();
            //try
            //{
            //    if (Clipboard.ContainsText())
            //    {
            //        handle.handleText(Logger);
            //    }
            //    else if (Clipboard.ContainsFileDropList())
            //    {
            //        handle.handleFileDrop(Logger);
            //    }
            //    else if (Clipboard.ContainsImage())
            //    {
            //        handle.handleImage(Logger);
            //    }
            //    else if (Clipboard.ContainsAudio())
            //    {
            //        handle.handleAudio(Logger);
            //    }
            //    else
            //    {
            //        DTE Dte = (DTE)Microsoft.VisualStudio.Shell.Package.
            //   GetGlobalService(typeof(Microsoft.VisualStudio.Shell.Interop.SDTE));
            //        //SelectedItems select = Dte.SelectedItems;
            //        //SelectedItem SeI;
            //        //if(select.Count>0){
            //        //    SeI = select.Item(0);
            //        //    MessageBox.Show(SeI.Name);
            //        //}
            //        EnvDTE80.DTE2 _applicationObject = (DTE2)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(DTE));
            //        EnvDTE.UIHierarchy solutionExplorer = _applicationObject.ToolWindows.SolutionExplorer;
            //        object[] items = solutionExplorer.SelectedItems as object[];
            //        EnvDTE.UIHierarchyItem item = items[0] as EnvDTE.UIHierarchyItem;
            //        EnvDTE.ProjectItem projectItem = item.Object as EnvDTE.ProjectItem;
            //        string path = projectItem.Properties.Item("FullPath").Value.ToString();
            //        MessageBox.Show(path);

            //        //obj = Clipboard.GetFileDropList();
            //        //string[] file_names = (string[])obj;
            //        IDataObject id = Clipboard.GetDataObject();
            //        string[] li = id.GetFormats();
            //        object ob = id.GetData("CF_VSREFPROJECTITEMS");
                    
            //        MemoryStream memory = (MemoryStream)ob;
            //        byte[] buffer = memory.ToArray();
            //        //string result = ToString(buffer);
            //        string result = System.Text.Encoding.UTF8.GetString(buffer);
            //        ASCIIEncoding encoding = new ASCIIEncoding();
            //        string constructedString = encoding.GetString(buffer);
            //        //UnicodeEncoding encoding = new UnicodeEncoding();
            //        //string constructedString = encoding.GetString(buffer);  
            //        list.Add(new KeyValuePair<String, object>("Action", constructedString));
            //        Logger.LogInfo(list);
            //        //int a=0;
            //        //if(memory.CanRead){
            //        //    a = memory.ReadByte();
            //        //}
            //        //string[] file_names = (string[])id.GetData(DataFormats.FileDrop);
            //        Type n = id.GetType();
            //        MessageBox.Show(ob.ToString()+"!!!"+constructedString);
            //    }
            //}
            //catch (Exception e)
            //{
            //    //obj = null;
            //}
        }

        public void handlePaste(ILoggerDao Logger)
        {
            handle = new HandlePaste();
            handleCPC(Logger);
        }

        public void handleCut(ILoggerDao Logger)
        {
            handle = new HandleCut();
            handleCPC(Logger);

        }

        private void handleCPC(ILoggerDao Logger)
        {
            IDataObject iData = Clipboard.GetDataObject();
            try
            {
                if (Clipboard.ContainsText())
                {
                    handle.handleText(Logger);
                }
                else if (Clipboard.ContainsFileDropList())
                {
                    handle.handleFileDrop(Logger);
                }
                else if (Clipboard.ContainsImage())
                {
                    handle.handleImage(Logger);
                }
                else if (Clipboard.ContainsAudio())
                {
                    handle.handleAudio(Logger);
                }
                else if (iData.GetDataPresent("CF_VSREFPROJECTITEMS"))
                {
                    handle.handleVSProjectItem(Logger);
                }
                else
                {
                    MessageBox.Show("Command_ClipBoard Wrong!");
                }
            }
            catch (Exception e)
            {
                //obj = null;
            }
        }


    }
}
