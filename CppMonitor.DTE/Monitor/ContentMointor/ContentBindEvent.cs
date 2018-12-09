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
using NanjingUniversity.CppMonitor.Util.Util;
using NanjingUniversity.CppMonitor.Util.Common;

namespace NanjingUniversity.CppMonitor.Monitor.ContentMointor
{
    class ContentBindEvent : IBindEvent
    {
        private enum RecordKey
        {
            Operation, FilePath, From, To, Line, LineOffset,HappenTime,Project
        }
    
        private DTE2 Dte2;

        private Events DteEvents;

        private TextEditorEvents TextEvents;

        private DocumentEvents DocEvents;

        private SelectionEvents SelectEvents;

        private WindowEvents WindowEvents;

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
            this.WindowEvents = DteEvents.WindowEvents; 

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

            this.WindowEvents.WindowActivated += onWindowActivited;
        }

        /**
         * 监听代码窗口活跃状态，用来管理上下文中的文档对象
         */
        private void onWindowActivited(Window GotFocus, Window LostFocus)
        {
            if (IsWindowNeedToBeMonitored(LostFocus) && IsDocValid(LostFocus.Document))
            {
                Document oldDocument = LostFocus.Document;
                LogDocumentAction(DocumentAction.documentDeactive,oldDocument);
                clearContext();
            }
            if (IsWindowNeedToBeMonitored(GotFocus)&&IsDocValid(GotFocus.Document))
            {
                Document newDocument = GotFocus.Document;

                LogDocumentAction(DocumentAction.documentActive, newDocument);

                if (newDocument != Context.ActiveDoc)
                {
                    Context.ActiveDoc = newDocument;

                    string content = GetDocumentContent(Context.ActiveDoc);

                    Context.LastDocContent = content;
                    Context.LastEndOffset = -1;
                }
            }
        }

        /**
         * 文本事件变化监听器
         */
        private void OnTextChange(TextPoint StartPoint, TextPoint EndPoint, int Hint)
        {
            //对象获取过程 TextPoint -> TextDocument -> Document
            Document sourceDocument = StartPoint.Parent.Parent;
            if (!IsDocValid(sourceDocument) && sourceDocument != Context.ActiveDoc)
            {
                return;
            }
            sem.WaitOne();
            String Content = GetDocumentContent(Context.ActiveDoc);
            Tuple<String, String> ReplaceText = ContentUtil.GetReplaceText(
                StartPoint, EndPoint, Context.LastDocContent, Content
            );
            Debug.WriteLine("ReplacingText:{0} :ReplacedText:{1}",ReplaceText.Item1,ReplaceText.Item2);
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

        private void clearContext()
        {
            Context.ActiveDoc = null;
            Context.LastEndOffset = -1;
            Context.LastDocContent = null;
            TransferToStartState();
        }

        private static bool IsWindowNeedToBeMonitored(Window targetWindow)
        {
            return targetWindow != null && targetWindow.Type == vsWindowType.vsWindowTypeDocument;
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

            LogDocumentAction(DocumentAction.documentOpen, Doc);

        }

        private void OnDocClosing(Document Doc)
        {
            if (!IsDocValid(Doc))
            {
                return;
            }

            LogDocumentAction(DocumentAction.documentClose,Doc);
        }

        private void OnDocSaved(Document Doc)
        {
            if (!IsDocValid(Doc))
            {
                return;
            }

            LogDocumentAction(DocumentAction.documentSave,Doc);
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

        public String GetDocumentContent(Document targetDocument)
        {
            if (targetDocument == null)
            {
                //在假设里这一行不会被执行到，只是单纯做异常处理
                return null;
            }

            TextDocument Doc = (TextDocument)targetDocument.Object("TextDocument");
            EditPoint DocStart = Doc.StartPoint.CreateEditPoint();
            return DocStart.GetText(Doc.EndPoint);
        }

        private void LogDocumentAction(DocumentAction actionType,Document validDocument)
        {
            string fileFullPath = validDocument.FullName;
            string projectName = ConstantCommon.UNKNOWN_PROJECTNAME;
            //当事件类型是DocmentClose的时候 ProjectItem会触发异常，也就是此时document不再有Project属性
            if (actionType != DocumentAction.documentClose 
                    &&  validDocument.ProjectItem != null 
                    &&  validDocument.ProjectItem.ContainingProject != null)
            {
                projectName = validDocument.ProjectItem.ContainingProject.Name;
            }

            Debug.WriteLine("DocumentAction:{0}:file:{1}",actionType.ToString(),fileFullPath);

            List<KeyValuePair<String, Object>> list = new List<KeyValuePair<string, object>>();
            list.Add(new KeyValuePair<string, object>("Operation", actionType.ToString()));
            list.Add(new KeyValuePair<string, object>("FilePath", fileFullPath));
            list.Add(new KeyValuePair<string, object>("Project", projectName));

            Logger.LogInfo("document",list);
           
        }

        public void FlushBuffer(ContentAction Op, String From, String To)
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
                ContentUtil.ToUTF8(ProjectUtil.getProjectNameFromDoc(Context.ActiveDoc,""))
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

            Logger.LogInfo("content",list);
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
