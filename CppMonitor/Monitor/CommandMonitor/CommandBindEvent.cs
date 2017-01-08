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

        private IDictionary<int, Delegate> EventTable;

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
            
            // Initialize key event handlers table
            EventTable = new Dictionary<int, Delegate>();

            EventTable.Add(
                (int)VSConstants.VSStd97CmdID.Copy,
                new KeyEventHandler(HandleCopyEvent)
            );

            EventTable.Add(
                (int)VSConstants.VSStd97CmdID.Cut,
                new KeyEventHandler(HandleCutEvent)
            );

            EventTable.Add(
                (int)VSConstants.VSStd97CmdID.Paste,
                new KeyEventHandler(HandlePasteEvent)
            );

        }
        
        void IBindEvent.RegisterEvent()
        {
           // throw new NotImplementedException();
           //InitFile();
           //Handle document saving
           DocEvents.DocumentSaved += OnDocumentSave;
           //Handle command events
           CmdEvents.BeforeExecute += NextCmdExecute;
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
            
        }

        /**
         * Command Events handler
         */
        private void NextCmdExecute(
            string Guid, int ID, object CustomIn,
            object CustomOut, ref bool CancelDefault
        )
        {
            if (EventTable.ContainsKey(ID))
            {
                
                EventTable[ID].DynamicInvoke();
            }
        }

        /**
         * Handle copy command event
         */
        private void HandleCopyEvent()
        {
            Document Doc = Dte.ActiveDocument;
            TextSelection Selection = (TextSelection)Doc.Selection;
           
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture,"!!Copy is"+Selection.Text,this.ToString()));
        }

        /**
         * Handle paste command event
         */
        private void HandlePasteEvent()
        {
            String PasteContent;
            try
            {
                PasteContent = Clipboard.GetText(TextDataFormat.Text);
            }
            catch (Exception)
            {
                PasteContent = "Fail to get clipboard content";
            }

            
        }

        /**
         * Handle cut command event
         */
        private void HandleCutEvent()
        {
            String CutContent;
            try
            {
                CutContent = Clipboard.GetText(TextDataFormat.Text);
            }
            catch (Exception)
            {
                CutContent = "Fail to get clipboard content";
            }

            
        }
        

    }
}
