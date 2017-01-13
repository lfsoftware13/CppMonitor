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

        private String ReplacingText = String.Empty;

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
            if (ReplacingText.Length > 0 || ReplacedText.Length > 0)
            {
                Context.FlushBuffer(
                    ContentBindEvent.Operation.Replace,
                    ReplacedText, ReplacingText
                );
            }
            ReplacingText = String.Empty;
            ReplacedText = String.Empty;
        }

        private void HandleReplaceText(TextPoint StartPoint,
            String ReplacingText, String ReplacedText)
        {
            Context.LineBeforeFlush = StartPoint.Line;
            Context.LineOffsetBeforeFlush = StartPoint.LineCharOffset;
            this.ReplacingText = ReplacingText;
            this.ReplacedText = ReplacedText;
            FlushBuffer();
        }
    }
}
