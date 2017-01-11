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
            Insert, Delete
        }

        private enum RecordKey
        {
            Operation, FileName, Content, Offset
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

            // TODO
            //Logger = LoggerFactory.loggerFactory.getLogger("Content");
            Logger = new LoggerDAOImpl_Stub();

            Context = new ContextState(-1, new StringBuilder(), null, null);

            EditState = new StartState(this);
        }
         
        void IBindEvent.RegisterEvent()
        {
            TextEvents.LineChanged += OnTextChange;

            DocEvents.DocumentOpened += OnDocOpened;
            DocEvents.DocumentClosing += OnDocClosing;
        }

        public void OnTextChange(TextPoint StartPoint, TextPoint EndPoint, int Hint)
        {
            ReLog(StartPoint, EndPoint);

            Context.LastDocContent = GetDocContent();
            Context.LastStartOffset = StartPoint.AbsoluteCharOffset;
        }

        private void OnDocOpened(Document Doc)
        {
            if (Context.ActiveDoc != null)
            {
                EditState.LogInfo(null, null, null);
            }

            Context.ActiveDoc = Doc;
            Context.LastStartOffset = -1;
            Context.LastDocContent = GetDocContent();
        }

        private void OnDocClosing(Document Doc)
        {
            EditState.LogInfo(null, null, null);

            Context.ActiveDoc = null;
            Context.LastStartOffset = -1;
            Context.LastDocContent = null;
        }

        private String GetDocContent()
        {
            if (Context.ActiveDoc == null)
            {
                Context.ActiveDoc = Dte2.ActiveDocument;
            }

            TextDocument Doc = (TextDocument)Context.ActiveDoc.Object("TextDocument");
            EditPoint DocStart = Doc.StartPoint.CreateEditPoint();
            return DocStart.GetText(Doc.EndPoint);
        }

        public void FlushBuffer(Operation Op)
        {
            List<KeyValuePair<String, Object>> list = new List<KeyValuePair<string, object>>();

            list.Add(new KeyValuePair<string, Object>(
                RecordKey.Operation.ToString(),
                Op.ToString()
            ));

            String Content = Context.Buffer.ToString();
            list.Add(new KeyValuePair<string, object>(
                RecordKey.Content.ToString(), Content
            ));
            Context.Buffer.Clear();

            list.Add(new KeyValuePair<string, object>(
                RecordKey.FileName.ToString(), Context.ActiveDoc.Name
            ));

            list.Add(new KeyValuePair<string, object>(
                RecordKey.Offset.ToString(), Context.LastStartOffset
            ));

            Logger.LogInfo(list);
        }

        private bool isCppFile(String name)
        {
            try
            {
                String[] temp = name.Split(new char[] { '.' });
                return temp[1].Equals("h") || temp[1].Equals("cpp") || temp[1].Equals("cs");
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void ReLog(TextPoint StartPoint, TextPoint EndPoint)
        {
            if (!isCppFile(Dte2.ActiveWindow.Document.Name)) return;

            EditState.LogInfo(StartPoint, EndPoint, GetDocContent());
        }

        public int GetContentDelta(String CurrentContent)
        {
            return CurrentContent.Length - Context.LastDocContent.Length;
        }

        public void SetState(IEditState State)
        {
            EditState = State;
        }

        public int LastStartOffset
        {
            get { return Context.LastStartOffset; }
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
        }

        
    }
}
