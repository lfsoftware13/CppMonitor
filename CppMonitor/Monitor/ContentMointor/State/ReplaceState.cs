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
                //FlushBuffer();
                return;
            }

            Tuple<string, string> ReplaceText = ContentUtil.GetReplaceText(
                StartPoint, Context.LastDocContent, DocContent
            );
            String ReplacingText = ReplaceText.Item1;
            String ReplacedText = ReplaceText.Item2;

            // 如果发生了删除事件，切换到删除状态
            if (ContentUtil.IsDeleteEvent(ReplacingText, ReplacedText))
            {
                Context.TransferToDeleteState(
                    StartPoint, EndPoint, DocContent
                );
                return;
            }

            // 如果发生了插入事件，切换到插入状态
            if (ContentUtil.IsInsertEvent(ReplacingText, ReplacedText))
            {
                Context.TransferToInsertState(
                    StartPoint, EndPoint, DocContent
                );
                return;
            }

            // 处理文本替换事件
            HandleReplaceText(StartPoint, ReplacingText, ReplacedText);
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
            Context.LineBeforeFlush = StartPoint.Line;
            Context.LineOffsetBeforeFlush = StartPoint.LineCharOffset;
            FlushBuffer();
        }

        private void HandleReplaceText(TextPoint StartPoint,
            String ReplacingText, String ReplacedText)
        {
            Context.LineBeforeFlush = StartPoint.Line;
            Context.LineOffsetBeforeFlush = StartPoint.LineCharOffset;

            FlushBuffer();
        }
    }
}
