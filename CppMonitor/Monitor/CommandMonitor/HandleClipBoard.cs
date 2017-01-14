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
            List<KeyValuePair<String, object>> list = new List<KeyValuePair<string, object>>();
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
                else
                {
                    //obj = Clipboard.GetFileDropList();
                    //string[] file_names = (string[])obj;
                    IDataObject id = Clipboard.GetDataObject();
                    string[] li = id.GetFormats();
                    object ob = id.GetData("CF_VSREFPROJECTITEMS");
                    
                    MemoryStream memory = (MemoryStream)ob;
                    byte[] buffer = memory.ToArray();
                    //string result = ToString(buffer);
                    string result = System.Text.Encoding.UTF8.GetString(buffer);
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    string constructedString = encoding.GetString(buffer);
                    //UnicodeEncoding encoding = new UnicodeEncoding();
                    //string constructedString = encoding.GetString(buffer);  
                    list.Add(new KeyValuePair<String, object>("Action", constructedString));
                    Logger.LogInfo(list);
                    //int a=0;
                    //if(memory.CanRead){
                    //    a = memory.ReadByte();
                    //}
                    //string[] file_names = (string[])id.GetData(DataFormats.FileDrop);
                    Type n = id.GetType();
                    MessageBox.Show(ob.ToString()+"!!!"+constructedString);
                }
            }
            catch (Exception e)
            {
                //obj = null;
            }
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
                else
                {
                    MessageBox.Show("Command Wrong!");
                }
            }
            catch (Exception e)
            {
                //obj = null;
            }
        }


        private string ToString(byte[] bytes)
        {
            string response = string.Empty;

            foreach (byte b in bytes)
                response += (Char)b;

            return response;
        }


    }
}
