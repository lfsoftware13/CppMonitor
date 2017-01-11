﻿using EnvDTE;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanjingUniversity.CppMonitor.Monitor.ContentMointor.State
{
    class InsertState : IEditState
    {
        private ContentBindEvent Context;

        public InsertState(ContentBindEvent Context)
        {
            this.Context = Context;
        }

        public void LogInfo(TextPoint StartPoint, TextPoint EndPoint, String DocContent)
        {
            // 如果文本内容没有变化而被调用，清空缓冲区
            if (StartPoint == null || EndPoint == null || DocContent == null)
            {
                FlushBuffer();
                return;
            }

            // 如果发生了删除事件，切换上下文的状态
            int DeltaLength = Context.GetContentDelta(DocContent);
            if (DeltaLength < 0)
            {
                Context.TransferToDeleteState(StartPoint, EndPoint);
                return;
            }

            Context.HandleInsertText(StartPoint, EndPoint, DocContent);
        }

        public void FlushBuffer()
        {
            if (Context.Buffer.Length > 0)
            {
                Context.FlushBuffer(
                    ContentBindEvent.Operation.Insert,
                    String.Empty,
                    Context.Buffer.ToString()
                );
            }
        }

    }
}
