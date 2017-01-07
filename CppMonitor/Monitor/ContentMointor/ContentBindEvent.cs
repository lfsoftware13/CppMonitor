﻿using System;
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
        private enum Operation {
            Add, Delete, Null
        }

        private DTE2 Dte2;

        private Events DteEvents;

        private TextEditorEvents TextEvents;

        private DocumentEvents DocEvents;

        private ILoggerDao Logger;

        //最后一次编辑的位置
        private TextPoint LastEditPoint;

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

            Logger = LoggerFactory.loggerFactory.getLogger("Content");

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
            String AddText = StartEdit.GetText(EndPoint);

            //删除文本
            if (AddText.Equals(""))
            {
                HandleTextDeleted(StartPoint);
            }
        }

        private void HandleTextDeleted(TextPoint StartPoint)
        {
            String DelText = GetDeletedText();

            //第一次编辑文本
            if (LastEditPoint == null)
            {
                Debug.Assert(LastOperation == Operation.Null);
                Debug.Assert(Buffer.Length == 0);

                Buffer.Append(DelText);
            }
            else
            {
                Debug.Assert(LastEditPoint != null);

                int LastOffset = LastEditPoint.AbsoluteCharOffset;
                int NowOffset = StartPoint.AbsoluteCharOffset;
                int DelLength = GetDeletedTextLength();

                //如果上次也是删除事件并且删除位置连续，聚合删除内容
                if (LastOperation == Operation.Delete
                    && LastOffset - NowOffset == DelLength)
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

        private void OnDocOpened(Document Doc)
        {
            if (ActiveDoc != null)
            {
                FlushBuffer();
            }

            ActiveDoc = Doc;
            LastEditPoint = null;
            LastDocContent = getDocContent();
            LastOperation = Operation.Null;
        }

        private void OnDocClosing(Document Doc)
        {
            FlushBuffer();

            ActiveDoc = null;
            LastEditPoint = null;
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

        }

        private String GetDeletedText()
        {
            return null;
        }

        private int GetDeletedTextLength()
        {
            return 0;
        }
    }
}
