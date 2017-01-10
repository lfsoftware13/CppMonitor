using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE80;
using EnvDTE;
using System.IO;

using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using System.Globalization;
using NanjingUniversity.CppMonitor.DAO.imp;
using NanjingUniversity.CppMonitor.DAO;
using System.Windows.Forms;

namespace NanjingUniversity.CppMonitor.Monitor.CommandMonitor
{
    class CommandBindEvent : IBindEvent
    {

        private DTE Dte;

        private Events DteEvents;

        private DocumentEvents DocEvents;

        private TextEditorEvents TextChangedEvents;

        private CommandEvents CmdEvents;

        private String DocContent;

        private ILoggerDao Logger;


        private IDictionary<int, Delegate> BefEventTable;
        private IDictionary<int, Delegate> AftEventTable;

        private delegate void KeyEventHandler();

        public CommandBindEvent()
        {
            Dte = (DTE)Microsoft.VisualStudio.Shell.Package.
                GetGlobalService(typeof(Microsoft.VisualStudio.Shell.Interop.SDTE));
            DteEvents = Dte.Events;

            //Handle document saving
            DocEvents = DteEvents.DocumentEvents;


            //Handle command events
            CmdEvents = DteEvents.CommandEvents;

            //database handle
            Logger = new ILoggerDaoImpl_stub();


            // Initialize key event handlers table
            BefEventTable = new Dictionary<int, Delegate>();

            BefEventTable.Add(
                (int)VSConstants.VSStd97CmdID.Copy,
                new KeyEventHandler(HandleCopyEvent)
            );

            BefEventTable.Add(
                (int)VSConstants.VSStd97CmdID.Cut,
                new KeyEventHandler(HandleCutEvent)
            );

            BefEventTable.Add(
                (int)VSConstants.VSStd97CmdID.Paste,
                new KeyEventHandler(HandlePasteEvent)
            );

            BefEventTable.Add(
                (int)VSConstants.VSStd97CmdID.Undo,
                new KeyEventHandler(HandleUndoEvent)
            );

            BefEventTable.Add(
                (int)VSConstants.VSStd97CmdID.Redo,
                new KeyEventHandler(HandleRedoEvent)
            );

            AftEventTable = new Dictionary<int,Delegate>();
            AftEventTable.Add(
                (int)VSConstants.VSStd97CmdID.Undo,
                new KeyEventHandler(HandleUndoEventAft)
            );

            AftEventTable.Add(
                (int)VSConstants.VSStd97CmdID.Redo,
                new KeyEventHandler(HandleRedoEventAft)
            );
        }

        void IBindEvent.RegisterEvent()
        {
            // throw new NotImplementedException();
            //InitFile();
            //Handle document saving
            DocEvents.DocumentSaved += OnDocumentSave;
            //Handle command events
            CmdEvents.BeforeExecute += BefCmdExecute;
            CmdEvents.AfterExecute += AftCmdExecute;
            //init active doc content
            //InitDocContent();
        }

        /**
         * Save current document content
         */
        private void InitDocContent()
        {
            DocContent = GetCurrentDocContent();
        }

        /**
         * Get current document content
         */
        private String GetCurrentDocContent()
        {
            TextDocument Doc = (TextDocument)Dte.ActiveDocument.Object("TextDocument");
            EditPoint DocStart = Doc.StartPoint.CreateEditPoint();
            return DocStart.GetText(Doc.EndPoint);
        }

        /**
         * Document Save Event handler
         */
        private void OnDocumentSave(Document doc)
        {
            string content = GetCurrentDocContent();
            List<KeyValuePair<String, object>> list = new List<KeyValuePair<string, object>>();
            list.Add(new KeyValuePair<String, object>("Avtion", "Save"));
            list.Add(new KeyValuePair<String, object>("FileName", doc.Name));
            list.Add(new KeyValuePair<String, object>("Path", doc.Path));
            list.Add(new KeyValuePair<String, object>("Content", content));
            Logger.LogInfo(list);

        }

        /**
         * Command Events handler
         */
        private void BefCmdExecute(
            string Guid, int ID, object CustomIn,
            object CustomOut, ref bool CancelDefault
        )
        {
            //MessageBox.Show("Start command");
            if (BefEventTable.ContainsKey(ID))
            {

                BefEventTable[ID].DynamicInvoke();
            }
        }

        private void AftCmdExecute(string Guid, int ID, object CustomIn,
            object CustomOut)
        {
            //MessageBox.Show("Command end");
            if (AftEventTable.ContainsKey(ID))
            {

                AftEventTable[ID].DynamicInvoke();
            }
        }

        /**
         * Handle copy command event
         */
        private void HandleCopyEvent()
        {
            Document Doc = Dte.ActiveDocument;
            TextSelection Selection = (TextSelection)Doc.Selection;
            List<KeyValuePair<String, object>> list = new List<KeyValuePair<string, object>>();

            if (Selection.Text != null)
            {
                list.Add(new KeyValuePair<String, object>("Avtion", "Copy"));
                list.Add(new KeyValuePair<String, object>("CopyFrom_Name", Doc.Name));
                list.Add(new KeyValuePair<String, object>("Path", Doc.Path));
                list.Add(new KeyValuePair<String, object>("Content", Selection.Text));
                Logger.LogInfo(list);
            }

        }

        /**
         * Handle paste command event
         */
        private void HandlePasteEvent()
        {
            Document Doc = Dte.ActiveDocument;
            String PasteContent;
            try
            {
                PasteContent = Clipboard.GetText(TextDataFormat.Text);
            }
            catch (Exception)
            {
                PasteContent = "Fail to get clipboard content";
            }

            List<KeyValuePair<String, object>> list = new List<KeyValuePair<string, object>>();
            if (PasteContent != null)
            {
                list.Add(new KeyValuePair<String, object>("Avtion", "Paste"));
                list.Add(new KeyValuePair<String, object>("PasteTo_Name", Doc.Name));
                list.Add(new KeyValuePair<String, object>("Path", Doc.Path));
                list.Add(new KeyValuePair<String, object>("Content", PasteContent));
                Logger.LogInfo(list);
            }

        }

        /**
         * Handle cut command event
         */
        private void HandleCutEvent()
        {
            Document Doc = Dte.ActiveDocument;
            String CutContent;
            try
            {
                CutContent = Clipboard.GetText(TextDataFormat.Text);
            }
            catch (Exception)
            {
                CutContent = "Fail to get clipboard content";
            }

            List<KeyValuePair<String, object>> list = new List<KeyValuePair<string, object>>();
            if (CutContent != null)
            {
                list.Add(new KeyValuePair<String, object>("Avtion", "Cut"));
                list.Add(new KeyValuePair<String, object>("CutFrom_Name", Doc.Name));
                list.Add(new KeyValuePair<String, object>("Path", Doc.Path));
                list.Add(new KeyValuePair<String, object>("Text", CutContent));
                Logger.LogInfo(list);
            }

        }

        /**
         * Handle undo command event
         */
        private void HandleUndoEvent()
        {
            Document Doc = Dte.ActiveDocument;
            String content = GetCurrentDocContent();
            List<KeyValuePair<String, object>> list = new List<KeyValuePair<string, object>>();
            list.Add(new KeyValuePair<String, object>("Avtion", "StartUndo"));
            list.Add(new KeyValuePair<String, object>("Name", Doc.Name));
            list.Add(new KeyValuePair<String, object>("Path", Doc.Path));
            list.Add(new KeyValuePair<String, object>("Content", content));
            Logger.LogInfo(list);

        }

        /**
         * Handle redo command event
         */
        private void HandleRedoEvent()
        {
            Document Doc = Dte.ActiveDocument;
            String content = GetCurrentDocContent();
            List<KeyValuePair<String, object>> list = new List<KeyValuePair<string, object>>();
            list.Add(new KeyValuePair<String, object>("Avtion", "StartRedo"));
            list.Add(new KeyValuePair<String, object>("Name", Doc.Name));
            list.Add(new KeyValuePair<String, object>("Path", Doc.Path));
            list.Add(new KeyValuePair<String, object>("Content", content));
            Logger.LogInfo(list);
        }

        private void HandleUndoEventAft()
        {
            Document Doc = Dte.ActiveDocument;
            String content = GetCurrentDocContent();
            List<KeyValuePair<String, object>> list = new List<KeyValuePair<string, object>>();
            list.Add(new KeyValuePair<String, object>("Avtion", "UndoEnd"));
            list.Add(new KeyValuePair<String, object>("Name", Doc.Name));
            list.Add(new KeyValuePair<String, object>("Content", content));
            Logger.LogInfo(list);
        }

        private void HandleRedoEventAft()
        {
            Document Doc = Dte.ActiveDocument;
            String content = GetCurrentDocContent();
            List<KeyValuePair<String, object>> list = new List<KeyValuePair<string, object>>();
            list.Add(new KeyValuePair<String, object>("Avtion", "RedoEnd"));
            list.Add(new KeyValuePair<String, object>("Name", Doc.Name));
            list.Add(new KeyValuePair<String, object>("Content", content));
            Logger.LogInfo(list);
        }



    }
}
