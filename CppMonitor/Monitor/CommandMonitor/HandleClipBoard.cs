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
