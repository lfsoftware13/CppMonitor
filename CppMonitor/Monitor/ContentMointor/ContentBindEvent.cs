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
using System.Threading;

namespace NanjingUniversity.CppMonitor.Monitor.ContentMointor
{
    class ContentBindEvent : IBindEvent
    {
        public enum Operation
        {
            Insert, Delete, Replace, Save
        }

        private enum RecordKey
        {
            Operation, FilePath, From, To, Line, LineOffset,HappenTime,Project
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

        #region 定时刷入
        ////用于管理异步请求是否取消的上下文
        //private CancellationTokenSource cts = null;
        //保证保证onTextChange顺序处理
        private Semaphore sem = null;
        #endregion

        public ContentBindEvent()
        {
            Dte2 = (DTE2)Microsoft.VisualStudio.Shell.Package.
                GetGlobalService(typeof(Microsoft.VisualStudio.Shell.Interop.SDTE));
            DteEvents = Dte2.Events;
            TextEvents = DteEvents.TextEditorEvents;
            DocEvents = DteEvents.DocumentEvents;
            SelectEvents = DteEvents.SelectionEvents;

            Logger = LoggerFactory.loggerFactory.getLogger("Content");

            Context = new ContextState(
                -1, -1, -1, new StringBuilder(), null, null
            );

            EditState = new StartState(this);

            sem = new Semaphore(1,1);
        }
         
        void IBindEvent.RegisterEvent()
        {
            TextEvents.LineChanged += OnTextChange;

            DocEvents.DocumentOpened += OnDocOpened;
            DocEvents.DocumentClosing += OnDocClosing;
            DocEvents.DocumentSaved += OnDocSaved;
        }

        /**
         * 文本事件变化监听器
         */
        private void OnTextChange(TextPoint StartPoint, TextPoint EndPoint, int Hint)
        {
           
            Context.ActiveDoc = StartPoint.Parent.Parent;
            if (!IsDocValid(Context.ActiveDoc))
            {
                return;
            }
            sem.WaitOne();
            String Content = GetDocContent();
            Tuple<String, String> ReplaceText = ContentUtil.GetReplaceText(
                StartPoint, Context.LastDocContent, Content
            );
            String ReplacingText = ReplaceText.Item1;
            String ReplacedText = ReplaceText.Item2;

            ReLog(StartPoint, EndPoint, ref ReplacingText, ref ReplacedText);

            Context.LastDocContent = Content;
            Context.LastEndOffset = EndPoint.AbsoluteCharOffset;
            sem.Release();
        }

        /**
         * 重新处理文本事件变化
         */
        public void ReLog(TextPoint StartPoint, TextPoint EndPoint,
            ref String ReplacingText, ref String ReplacedText)
        {
            //2018-11-25：每次事件直接记录，不做任何处理
            EditState.LogInfo(StartPoint, EndPoint,
                ref ReplacingText, ref ReplacedText);
            EditState.FlushBuffer();
            TransferToStartState();
        }

        private static bool IsDocValid(Document Doc)
        {
            return Doc != null && ContentUtil.IsCppFile(Doc.Name);
        }

        /*====================== Document Event Method Start ==================================*/

        private void OnDocOpened(Document Doc)
        {
            if (!IsDocValid(Doc))
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
            if (!IsDocValid(Doc))
            {
                return;
            }

            if (Context.ActiveDoc != null)
            {
                EditState.FlushBuffer();
            }

            Context.ActiveDoc = null;
            Context.LastEndOffset = -1;
            Context.LastDocContent = GetDocContent();
            TransferToStartState();
        }

        private void OnDocSaved(Document Doc)
        {
            if (!IsDocValid(Doc))
            {
                return;
            }

            Context.ActiveDoc = Doc;
            EditState.FlushBuffer();
            FlushBuffer(Operation.Save, String.Empty, String.Empty);
        }

        /*====================== Document Event Method End ==================================*/

        /*====================== Edit State Method Start ==================================*/

        public void TransferToDeleteState(TextPoint StartPoint, 
            TextPoint EndPoint, ref String ReplacingText,
            ref String ReplacedText)
        {
            EditState.FlushBuffer();
            SetState(new DeleteState(this));
            Context.HappenTime = DateTime.Now.Ticks;
            ReLog(StartPoint, EndPoint, ref ReplacingText, ref ReplacedText);
        }

        public void TransferToReplaceState(TextPoint StartPoint,
            TextPoint EndPoint, ref String ReplacingText,
            ref String ReplacedText)
        {
            EditState.FlushBuffer();
            SetState(new ReplaceState(this));
            Context.HappenTime = DateTime.Now.Ticks;
            ReLog(StartPoint, EndPoint, ref ReplacingText, ref ReplacedText);
        }

        public void TransferToInsertState(TextPoint StartPoint,
            TextPoint EndPoint, ref String ReplacingText,
            ref String ReplacedText)
        {
            EditState.FlushBuffer();
            SetState(new InsertState(this));
            Context.HappenTime = DateTime.Now.Ticks;
            ReLog(StartPoint, EndPoint, ref ReplacingText, ref ReplacedText);
        }

        private void TransferToStartState()
        {
            SetState(new StartState(this));
        }

        /*====================== Edit State Method End ==================================*/

        public String GetDocContent()
        {
            if (Context.ActiveDoc == null)
            {
                if (Dte2.ActiveWindow.Document == null)
                {
                    return String.Empty;
                }
                Context.ActiveDoc = Dte2.ActiveWindow.Document;
            }

            TextDocument Doc = (TextDocument)Context.ActiveDoc.Object("TextDocument");
            EditPoint DocStart = Doc.StartPoint.CreateEditPoint();
            return DocStart.GetText(Doc.EndPoint);
        }

        public void FlushBuffer(Operation Op, String From, String To)
        {
            //sem.WaitOne();
            List<KeyValuePair<String, Object>> list = new List<KeyValuePair<string, object>>();

            list.Add(new KeyValuePair<string, Object>(
                ContentUtil.ToUTF8(RecordKey.Operation.ToString()),
                ContentUtil.ToUTF8(Op.ToString())
            ));

            list.Add(new KeyValuePair<string, object>(
                ContentUtil.ToUTF8(RecordKey.FilePath.ToString()), 
                ContentUtil.ToUTF8(Context.ActiveDoc.FullName)
            ));

            list.Add(new KeyValuePair<string, object>(
                ContentUtil.ToUTF8(RecordKey.Project.ToString()),
                ContentUtil.ToUTF8(Context.ActiveDoc.ProjectItem.ContainingProject.Name)
            ));

            list.Add(new KeyValuePair<string, object>(
                ContentUtil.ToUTF8(RecordKey.From.ToString()),
                ContentUtil.ToUTF8("`" + From + "`")
            ));

            list.Add(new KeyValuePair<string, object>(
                ContentUtil.ToUTF8(RecordKey.To.ToString()),
                ContentUtil.ToUTF8("`" + To + "`")
            ));

            Context.Buffer.Clear();
            Context.HappenTime = DateTime.Now.Ticks;//重置操作时间

            list.Add(new KeyValuePair<string, object>(
                ContentUtil.ToUTF8(RecordKey.Line.ToString()),
                ContentUtil.ToUTF8(Context.LineBeforeFlush.ToString())
            ));

            list.Add(new KeyValuePair<string, object>(
                ContentUtil.ToUTF8(RecordKey.LineOffset.ToString()),
                ContentUtil.ToUTF8((Context.LineOffsetBeforeFlush - 1).ToString())
            ));

            list.Add(new KeyValuePair<string, object>(
                ContentUtil.ToUTF8(RecordKey.HappenTime.ToString()),
                Context.HappenTime
            ));

            Logger.LogInfo("",list);
            //sem.Release();
        }

        public int GetContentDelta(String CurrentContent)
        {
            return CurrentContent.Length - Context.LastDocContent.Length;
        }

        public void SetState(IEditState State)
        {
            //sem.WaitOne();
            //fixbug6-26:当状态没有改变时不需要清空buffer
            if(EditState.GetType() != State.GetType()){
                EditState.FlushBuffer();
                EditState = State;
            }
            //sem.Release();
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

        public long HappenTime
        {
            get
            {
                return Context.HappenTime;
            }
            set
            {
                Context.HappenTime = value;
            }
        }
        /*====================== Get Property Method End ==================================*/
    }
}
