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

namespace NanjingUniversity.CppMonitor.Monitor.ContentMointor
{
    class ContentBindEvent : IBindEvent
    {
        private enum Operation
        {
            Insert, Delete, Null
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

        //最后一次编辑的位置
        private int LastStartOffset;

        //最后一次编辑操作的类型
        private Operation LastOperation;

        //所编辑内容的缓冲区
        private StringBuilder Buffer;

        //当前编辑的文档对象
        private Document ActiveDoc;

        //上一次编辑之后的文档内容
        private String LastDocContent;

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

            Buffer = new StringBuilder();

            LastOperation = Operation.Null;
        }
         
        void IBindEvent.RegisterEvent()
        {
            TextEvents.LineChanged += OnTextChange;

            DocEvents.DocumentOpened += OnDocOpened;
            DocEvents.DocumentClosing += OnDocClosing;
        }

        private void OnTextChange(TextPoint StartPoint, TextPoint EndPoint, int Hint)
        {
            //获得增加的文本
            EditPoint StartEdit = StartPoint.CreateEditPoint();
            String InsertedText = StartEdit.GetText(EndPoint);

            //删除文本
            if (InsertedText.Equals(""))
            {
                HandleTextDeleted(StartPoint, EndPoint);
            }
            //增加文本
            else
            {
                HandleTextInserted(StartPoint, InsertedText);
            }

            LastDocContent = getDocContent();
            LastStartOffset = StartPoint.AbsoluteCharOffset;
        }

        private void HandleTextDeleted(TextPoint StartPoint, TextPoint EndPoint)
        {
            String DelText = GetDeletedText(StartPoint);

            //第一次编辑文本
            if (LastStartOffset == -1)
            {
                Debug.Assert(LastOperation == Operation.Null);
                Debug.Assert(Buffer.Length == 0);

                Buffer.Append(DelText);
            }
            else
            {
                Debug.Assert(LastStartOffset != -1);

                int NowOffset = StartPoint.AbsoluteCharOffset;
                int DelLength = GetDeletedTextLength();

                //如果上次也是删除事件并且删除位置连续，聚合删除内容
                if (LastOperation == Operation.Delete
                    && LastStartOffset - NowOffset == DelLength)
                {
                    Buffer.Insert(0, DelText);
                }
                else
                {
                    FlushBuffer();
                    Buffer.Append(DelText);
                }
            }

            LastOperation = Operation.Delete;
        }

        private void HandleTextInserted(TextPoint StartPoint, String InsertedText)
        {
            //第一次编辑文本
            if (LastStartOffset == -1)
            {
                Debug.Assert(LastOperation == Operation.Null);
                Debug.Assert(Buffer.Length == 0);

                Buffer.Append(InsertedText);
            }
            else
            {
                Debug.Assert(LastStartOffset != -1);

                int NowOffset = StartPoint.AbsoluteCharOffset;
                int InsertLength = InsertedText.Length;

                //如果上次也是插入事件并且插入位置连续，聚合插入内容
                if (LastOperation == Operation.Insert
                    && (NowOffset - LastStartOffset == InsertLength))
                {
                    Buffer.Append(InsertedText);
                }
                else
                {
                    FlushBuffer();
                    Buffer.Append(InsertedText);
                }
            }

            LastOperation = Operation.Insert;
        }

        private void OnDocOpened(Document Doc)
        {
            if (ActiveDoc != null)
            {
                FlushBuffer();
            }

            ActiveDoc = Doc;
            LastStartOffset = -1;
            LastDocContent = getDocContent();
            LastOperation = Operation.Null;
        }

        private void OnDocClosing(Document Doc)
        {
            FlushBuffer();

            ActiveDoc = null;
            LastStartOffset = -1;
            LastDocContent = null;
            LastOperation = Operation.Null;
        }

        private String getDocContent()
        {
            if (ActiveDoc == null)
            {
                ActiveDoc = Dte2.ActiveDocument;
            }

            TextDocument Doc = (TextDocument)ActiveDoc.Object("TextDocument");
            EditPoint DocStart = Doc.StartPoint.CreateEditPoint();
            return DocStart.GetText(Doc.EndPoint);
        }

        private void FlushBuffer()
        {
            List<KeyValuePair<String, Object>> list = new List<KeyValuePair<string, object>>();

            list.Add(new KeyValuePair<string, Object>(
                RecordKey.Operation.ToString(),
                LastOperation.ToString()
            ));

            String Content = Buffer.ToString();
            list.Add(new KeyValuePair<string, object>(
                RecordKey.Content.ToString(), Content
            ));
            Buffer.Clear();

            list.Add(new KeyValuePair<string, object>(
                RecordKey.FileName.ToString(), ActiveDoc.Name
            ));

            list.Add(new KeyValuePair<string, object>(
                RecordKey.Offset.ToString(), LastStartOffset
            ));

            Logger.LogInfo(list);
        }

        private String GetDeletedText(TextPoint StartPoint)
        {
            String[] Lines = LastDocContent.Split(new char[] { '\n' });
            int StartOffset = StartPoint.LineCharOffset;
            int StartLine = StartPoint.Line;
            int DelCharNum = GetDeletedTextLength();

            String DelText = "";
            // 只删除一行
            if (Lines[StartLine - 1].Length - StartOffset >= DelCharNum)
            {
                DelText = Lines[StartLine - 1].Substring(
                    StartOffset - 1, DelCharNum
                );
            }
            // 删除多行
            else
            {
                StringBuilder Temp = new StringBuilder("");
                // 获得删除的第一行
                String FirstLine = Lines[StartLine - 1];
                Temp.Append(FirstLine.Substring(StartOffset - 1)).Append('\n');
                // 获得删除的剩余行
                for (int i = StartLine; DelCharNum > Temp.Length; ++i)
                {
                    if (DelCharNum - Temp.Length > Lines[i].Length)
                    {
                        Temp.Append(Lines[i].Substring(0, Lines[i].Length)).Append('\n');
                    }
                    else
                    {
                        Temp.Append(Lines[i].Substring(0, DelCharNum - Temp.Length));
                    }
                }

                DelText = Temp.ToString();
            }

            return DelText;
        }

        private int GetDeletedTextLength()
        {
            String CurrentDoc = getDocContent();
            return LastDocContent.Length - CurrentDoc.Length;
        }

        private bool IsWhitespace(String str)
        {
            foreach (char c in str)
            {
                if (!(c == '\t' || c == '\n'
                    || c == ' ' || c == '\r')) return false;
            }
            return true;
        }
    }
}
