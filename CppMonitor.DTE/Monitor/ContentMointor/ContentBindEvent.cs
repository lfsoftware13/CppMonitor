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

        //��ǰ�༭����������Ϣ
        private ContextState Context;

        #region ��ʱˢ��
        ////���ڹ����첽�����Ƿ�ȡ����������
        //private CancellationTokenSource cts = null;
        //��֤��֤onTextChange˳����
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
                null, null
            );

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
         * �������봰�ڻ�Ծ״̬�����������������е��ĵ�����
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
                }
            }
        }

        /**
         * �ı��¼��仯������
         */
        private void OnTextChange(TextPoint StartPoint, TextPoint EndPoint, int Hint)
        {
            //�����ȡ���� TextPoint -> TextDocument -> Document
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

            LogLineChangedAction(ref StartPoint,ref ReplacingText, ref ReplacedText);

            Context.LastDocContent = Content;
            sem.Release();
        }

        private void LogLineChangedAction(ref TextPoint StartPoint,ref String ReplacingText, ref String ReplacedText)
        {
            ContentAction contentAction = ContentAction.contentUnknown;
            if (ContentUtil.IsInsertEvent(ReplacingText,ReplacedText))
            {
                contentAction = ContentAction.contentInsert;
            }
            else if (ContentUtil.IsDeleteEvent(ReplacingText, ReplacedText))
            {
                contentAction = ContentAction.contentDelete;
            }else if (ContentUtil.IsReplaceEvent(ReplacingText, ReplacedText))
            {
                contentAction = ContentAction.contentReplace;
            }
            FlushBuffer(contentAction, ReplacedText,ReplacingText, StartPoint.Line, StartPoint.LineCharOffset);
        }

        private void clearContext()
        {
            Context.ActiveDoc = null;
            Context.LastDocContent = null;
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

        /*====================== Edit State Method End ==================================*/

        public String GetDocumentContent(Document targetDocument)
        {
            if (targetDocument == null)
            {
                //�ڼ�������һ�в��ᱻִ�е���ֻ�ǵ������쳣����
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
            //���¼�������DocmentClose��ʱ�� ProjectItem�ᴥ���쳣��Ҳ���Ǵ�ʱdocument������Project����
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

        public void FlushBuffer(ContentAction Op, String From, String To,int Line,int LineOffSet)
        {
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

            Context.HappenTime = DateTime.Now.Ticks;//���ò���ʱ��

            list.Add(new KeyValuePair<string, object>(
                ContentUtil.ToUTF8(RecordKey.Line.ToString()),
                ContentUtil.ToUTF8(Line.ToString())
            ));

            list.Add(new KeyValuePair<string, object>(
                ContentUtil.ToUTF8(RecordKey.LineOffset.ToString()),
                ContentUtil.ToUTF8(LineOffSet.ToString())
            ));

            list.Add(new KeyValuePair<string, object>(
                ContentUtil.ToUTF8(RecordKey.HappenTime.ToString()),
                Context.HappenTime
            ));

            Logger.LogInfo("content",list);
        }

        public int GetContentDelta(String CurrentContent)
        {
            return CurrentContent.Length - Context.LastDocContent.Length;
        }

        /*====================== Get Property Method Start ==================================*/
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
