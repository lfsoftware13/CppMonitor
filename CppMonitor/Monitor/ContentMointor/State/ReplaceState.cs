using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanjingUniversity.CppMonitor.Monitor.ContentMointor.State
{
    class ReplaceState : IEditState
    {
        private ContentBindEvent Context;

        private String ReplacedText = String.Empty;

        public ReplaceState(ContentBindEvent Context)
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
                TransferToDeleteState(StartPoint, EndPoint);
                return;
            }

            Context.HandleInsertText(StartPoint, EndPoint, DocContent);
        }

        private void TransferToDeleteState(TextPoint StartPoint, TextPoint EndPoint)
        {
            FlushBuffer();
            Context.SetState(new DeleteState(Context));
            Context.ReLog(StartPoint, EndPoint);
        }

        public void FlushBuffer()
        {
            if (Context.Buffer.Length > 0)
            {
                Context.FlushBuffer(
                    ContentBindEvent.Operation.Replace,
                    ReplacedText, Context.Buffer.ToString()
                );
            }
            ReplacedText = String.Empty;
        }

        /**
         * 刚刚检测到替换事件的发生，则调用这个方法
         */
        public void JustReplace(TextPoint StartPoint,
            String ReplacingText, String ReplacedText)
        {
            this.ReplacedText = ReplacedText;
            Context.Buffer.Append(ReplacingText);
            Context.StartOffsetBeforeFlush = StartPoint.AbsoluteCharOffset;
        }
    }
}
