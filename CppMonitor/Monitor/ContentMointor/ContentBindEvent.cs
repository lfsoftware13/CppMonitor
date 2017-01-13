using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using EnvDTE80;
using EnvDTE;
using NanjingUniversity.CppMonitor.DAO;
using NanjingUniversity.CppMonitor.Monitor.ContentMointor.State;

namespace NanjingUniversity.CppMonitor.Monitor.ContentMointor
{
    class ContentBindEvent : IBindEvent
    {
        public enum Operation
        {
            Insert, Delete, Replace, AutoComplete, Save
        }

        private enum RecordKey
        {
            Operation, FileName, From, To, Line, LineOffset
        }
    
        private DTE2 Dte2;

        private Events DteEvents;

        private TextEditorEvents TextEvents;

        private DocumentEvents DocEvents;

        private SelectionEvents SelectEvents;

        private ILoggerDao Logger;

        //当前编辑的上下文信息
        private ContextState Context;

        //当前的编辑状态
        private IEditState EditState;
        

        public ContentBindEvent()
        {
            Dte2 = (DTE2)Microsoft.VisualStudio.Shell.Package.
                GetGlobalService(typeof(Microsoft.VisualStudio.Shell.Interop.SDTE));
            DteEvents = Dte2.Events;
            TextEvents = DteEvents.TextEditorEvents;
            DocEvents = DteEvents.DocumentEvents;
            SelectEvents = DteEvents.SelectionEvents;

            //Logger = LoggerFactory.loggerFactory.getLogger("Content");
            Logger = new LoggerDAOImpl_Stub();

            Context = new ContextState(
                -1, -1, -1, new StringBuilder(), null, null
            );

            EditState = new StartState(this);
        }
         
        void IBindEvent.RegisterEvent()
        {
            TextEvents.LineChanged += OnTextChange;

            DocEvents.DocumentOpened += OnDocOpened;
            DocEvents.DocumentClosing += OnDocClosing;
            DocEvents.DocumentSaved += OnDocSaved;
        }

        public void OnTextChange(TextPoint StartPoint, TextPoint EndPoint, int Hint)
        {
            ReLog(StartPoint, EndPoint, GetDocContent());

            Context.LastDocContent = GetDocContent();
            Context.LastEndOffset = EndPoint.AbsoluteCharOffset;
        }

        /*====================== Document Event Method Start ==================================*/

        private void OnDocOpened(Document Doc)
        {
            if (!ContentUtil.isCppFile(Doc.Name))
            {
                return;
            }

            if (Context.ActiveDoc != null)
            {
                EditState.FlushBuffer();
            }

            Context.ActiveDoc = Doc;
            Context.LastEndOffset = -1;
            Context.LastDocContent = GetDocContent();
            TransferToStartState();
        }

        private void OnDocClosing(Document Doc)
        {
            EditState.FlushBuffer();

            Context.ActiveDoc = null;
            Context.LastEndOffset = -1;
            Context.LastDocContent = GetDocContent();
            TransferToStartState();
        }

        private void OnDocSaved(Document Doc)
        {
            EditState.FlushBuffer();

            FlushBuffer(Operation.Save, String.Empty, String.Empty);
        }

        /*====================== Document Event Method End ==================================*/

        /*====================== Edit State Method Start ==================================*/

        public void TransferToDeleteState(TextPoint StartPoint,
            TextPoint EndPoint, String DocContent)
        {
            EditState.FlushBuffer();
            SetState(new DeleteState(this));
            ReLog(StartPoint, EndPoint, DocContent);
        }

        public void TransferToReplaceState(TextPoint StartPoint,
            String ReplacingText, String ReplacedText)
        {
            EditState.FlushBuffer();
            ReplaceState State = new ReplaceState(this);
            State.JustReplace(StartPoint, ReplacingText, ReplacedText);
            SetState(State);
        }

        public void TransferToInsertState(TextPoint StartPoint,
            TextPoint EndPoint, String DocContent)
        {
            EditState.FlushBuffer();
            SetState(new InsertState(this));
            ReLog(StartPoint, EndPoint, DocContent);
        }

        private void TransferToStartState()
        {
            SetState(new StartState(this));
        }

        //public void TransferToInsertAfterEnterState(
        //    String InsertedText)
        //{
        //    Context.Buffer.Append(InsertedText);
        //    SetState(new InsertAfterEnterState(this));
        //}

        /*====================== Edit State Method End ==================================*/

        public String GetDocContent()
        {
            if (Context.ActiveDoc == null)
            {
                Context.ActiveDoc = Dte2.ActiveWindow.Document;
            }
            TextDocument Doc = (TextDocument)Context.ActiveDoc.Object("TextDocument");
            EditPoint DocStart = Doc.StartPoint.CreateEditPoint();
            return DocStart.GetText(Doc.EndPoint);
        }

        public void FlushBuffer(Operation Op, String From, String To)
        {
            List<KeyValuePair<String, Object>> list = new List<KeyValuePair<string, object>>();

            list.Add(new KeyValuePair<string, Object>(
                RecordKey.Operation.ToString(),
                Op.ToString()
            ));

            list.Add(new KeyValuePair<string, object>(
                RecordKey.From.ToString(), From
            ));

            list.Add(new KeyValuePair<string, object>(
                RecordKey.To.ToString(), To
            ));

            Context.Buffer.Clear();

            list.Add(new KeyValuePair<string, object>(
                RecordKey.FileName.ToString(), Context.ActiveDoc.Name
            ));

            list.Add(new KeyValuePair<string, object>(
                RecordKey.Line.ToString(), Context.LineBeforeFlush
            ));

            list.Add(new KeyValuePair<string, object>(
                RecordKey.LineOffset.ToString(), Context.LineOffsetBeforeFlush
            ));

            Logger.LogInfo(list);
        }

        public void ReLog(TextPoint StartPoint,
            TextPoint EndPoint, String DocContent)
        {
            if (!ContentUtil.isCppFile(Dte2.ActiveWindow.Document.Name)) return;

            EditState.LogInfo(StartPoint, EndPoint, DocContent);
        }

        public int GetContentDelta(String CurrentContent)
        {
            return CurrentContent.Length - Context.LastDocContent.Length;
        }

        public void SetState(IEditState State)
        {
            EditState = State;
        }

        /*====================== Get Property Method Start ==================================*/

        public int LastEndOffset
        {
            get { return Context.LastEndOffset; }
        }

        public int LineOffsetBeforeFlush
        {
            get { return Context.LineOffsetBeforeFlush; }
            set { Context.LineOffsetBeforeFlush = value; }
        }

        public int LineBeforeFlush
        {
            get { return Context.LineBeforeFlush; }
            set { Context.LineBeforeFlush = value; }
        }

        public StringBuilder Buffer
        {
            get { return Context.Buffer; }
        }

        public Document ActiveDoc
        {
            get { return Context.ActiveDoc; }
        }

        public String LastDocContent
        {
            get { return Context.LastDocContent; }
            set { Context.LastDocContent = value; }
        }

        /*====================== Get Property Method End ==================================*/
    }
}
