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

        private LoggerDAOImpl_Stub Logger;

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

            Context = new ContextState(-1, -1, -1, new StringBuilder(), null, null);

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
            ReLog(StartPoint, EndPoint);

            Context.LastDocContent = GetDocContent();
            Context.LastStartOffset = StartPoint.AbsoluteCharOffset;
        }

        /*====================== Document Event Method Start ==================================*/

        private void OnDocOpened(Document Doc)
        {
            if (Context.ActiveDoc != null)
            {
                EditState.FlushBuffer();
            }

            Context.ActiveDoc = Doc;
            Context.LastStartOffset = -1;
            Context.LastDocContent = GetDocContent();
        }

        private void OnDocClosing(Document Doc)
        {
            EditState.FlushBuffer();

            Context.ActiveDoc = null;
            Context.LastStartOffset = -1;
            Context.LastDocContent = null;
        }

        private void OnDocSaved(Document Doc)
        {
            EditState.FlushBuffer();

            FlushBuffer(Operation.Save, String.Empty, String.Empty);
        }

        /*====================== Document Event Method End ==================================*/

        /*====================== Edit State Method Start ==================================*/

        public void TransferToDeleteState(TextPoint StartPoint, TextPoint EndPoint)
        {
            EditState.FlushBuffer();
            SetState(new DeleteState(this));
            ReLog(StartPoint, EndPoint);
        }

        public void TransferToReplaceState(TextPoint StartPoint,
            String ReplacedText, String ReplacingText)
        {
            EditState.FlushBuffer();
            ReplaceState State = new ReplaceState(this);
            State.JustReplace(StartPoint, ReplacingText, ReplacedText);
            SetState(State);
        }

        public void TransferToInsertState(TextPoint StartPoint, TextPoint EndPoint)
        {
            EditState.FlushBuffer();
            SetState(new InsertState(this));
            ReLog(StartPoint, EndPoint);
        }

        private void TransferToInsertAfterEnterState(
            String InsertedText)
        {
            Context.Buffer.Append(InsertedText);
            SetState(new InsertAfterEnterState(this));
        }

        public bool IsTypeEnter(String Text, int OffsetDiff)
        {
            return Text.Equals("\r\n") && OffsetDiff == 1;
        }

        public void HandleInsertText(TextPoint StartPoint,
            TextPoint EndPoint, String DocContent)
        {
            StringBuilder Buffer = Context.Buffer;
            String InsertedText = GetInsertedText(StartPoint, EndPoint);

            //第一次编辑文本或者刚从其他状态切换过来
            if (Context.LastStartOffset == -1 || Context.Buffer.Length == 0)
            {
                Debug.Assert(Buffer.Length == 0);

                Context.LineBeforeFlush = StartPoint.Line;
                Context.LineOffsetBeforeFlush = StartPoint.LineCharOffset;

                if (InsertedText.Equals("\r\n"))
                {
                    TransferToInsertAfterEnterState(InsertedText);
                    return;
                }

                Buffer.Append(InsertedText);
            }
            else
            {
                int InsertLength = InsertedText.Length;
                int NowOffset = StartPoint.AbsoluteCharOffset;
                int OffsetDiff = NowOffset - Context.LastStartOffset;

                // 被插入的字符是"\r\n"，而且前后字符偏移只差1，
                // 说明插入紧接着的是换行符，这是观察VS而得到的结论，
                // 这种情况下，切换到InsertAfterEnterState
                if (IsTypeEnter(InsertedText, OffsetDiff))
                {
                    TransferToInsertAfterEnterState(InsertedText);
                    return;
                }

                // 如果满足以下条件中的任意一个，则聚合所要插入的内容
                // 1、被插入字符长度 = 前后两次偏移之差
                if (NowOffset - Context.LastStartOffset == InsertLength)
                {
                    Buffer.Append(InsertedText);
                }
                else
                {
                    FlushBuffer(
                        ContentBindEvent.Operation.Insert,
                        String.Empty,
                        Buffer.ToString()
                    );
                    Buffer.Append(InsertedText);
                }
            }
        }

        /**
         * @returns Item1 Replacing Text
         *          Item2 Replaced Text
         */
        public Tuple<String, String> GetReplaceText(TextPoint StartPoint, String CurrentDoc)
        {
            String LastDoc = Context.LastDocContent;
            int Start = GetCharOffset(StartPoint);
            int OldLength = LastDoc.Length;
            int NewLength = CurrentDoc.Length;

            if (Start >= OldLength || Start >= NewLength)
            {
                return new Tuple<string, string>(
                    Start >= NewLength ? String.Empty : CurrentDoc.Substring(Start),
                    Start >= OldLength ? String.Empty : LastDoc.Substring(Start)
                );
            }

            // 截去两个文本后面相同的部分
            String OldDoc = LastDoc.Substring(Start);
            String NewDoc = CurrentDoc.Substring(Start);
            int OldIndex = OldDoc.Length - 1;
            int NewIndex = NewDoc.Length - 1;
            while (OldIndex >= 0 && NewIndex >= 0)
            {
                if (OldDoc[OldIndex] != NewDoc[NewIndex]) break;
                --OldIndex;
                --NewIndex;
            }

            return new Tuple<string, string>(
                NewDoc.Substring(0, NewIndex + 1),
                OldDoc.Substring(0, OldIndex + 1)
            );
        }

        public String GetInsertedText(TextPoint StartPoint, TextPoint EndPoint)
        {
            EditPoint StartEdit = StartPoint.CreateEditPoint();
            String InsertedText = StartEdit.GetText(EndPoint);
            return InsertedText;
        }

        private int GetCharOffset(TextPoint StartPoint)
        {
            // 观察VS得到的结论
            return StartPoint.AbsoluteCharOffset + StartPoint.Line - 2;
        }

        /*====================== Edit State Method End ==================================*/

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

        public void SetState(IEditState State1)
        {
            EditState = State1;
        }

        /*====================== Get Property Method Start ==================================*/

        public int LastStartOffset
        {
            get { return Context.LastStartOffset; }
        }

        public int StartOffsetBeforeFlush
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
        }

        /*====================== Get Property Method End ==================================*/
    }
}
