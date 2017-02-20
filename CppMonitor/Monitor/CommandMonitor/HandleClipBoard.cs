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

        public void handleCopy()
        {
            handle = new HandleCopy();
            handleCPC();
        }

        public void handlePaste()
        {
            handle = new HandlePaste();
            handleCPC();
        }

        public void handleCut()
        {
            handle = new HandleCut();
            handleCPC();

        }
        
        private void handleCPC()
        {
            IDataObject iData = null;
            try
            {   
                while(iData == null){
                    try
                    {
                        iData = Clipboard.GetDataObject();
                        //if(iData == null){
                        //    System.Threading.Thread.Sleep(100);
                        //}
                    }catch(Exception e){
                        Application.DoEvents();
                        iData = Clipboard.GetDataObject();
                    }
                }
                if (Clipboard.ContainsText())
                {
                    handle.handleText();
                }
                else if (Clipboard.ContainsFileDropList())
                {
                    handle.handleFileDrop();
                }
                else if (Clipboard.ContainsImage())
                {
                    handle.handleImage();
                }
                else if (Clipboard.ContainsAudio())
                {
                    handle.handleAudio();
                }
                else
                {
              
                    if (iData.GetDataPresent("CF_VSREFPROJECTITEMS"))
                    {
                        handle.handleVSProjectItem();
                    }
                }
            }
            catch (Exception)
            {
                //obj = null;
            }
        }


    }
}
