using EnvDTE;
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

            // 如果发生了以下任意一种情况，认为用户使用了IDE的自动补全功能
            // 1、被替换文本和替换文本的长度都大于0，即发生了替换操作，
            //    这个结论是因为通过对VS的观察，在插入状态下发生替换，表示
            //    发生了IDE的自动补全，否则，替换操作会在删除状态下被捕获
            //Tuple<string, string> ReplaceText = Context.GetReplaceText(StartPoint, DocContent);
            //String ReplacingText = ReplaceText.Item1;
            //String ReplacedText = ReplaceText.Item2;
            //if (ReplacingText.Length > 0 && ReplacedText.Length > 0)
            //{
            //    HandleAutoComplete(ReplaceText);
            //    return;
            //}

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

        //private void HandleAutoComplete(Tuple<string, string> ReplaceText)
        //{
        //    Context.FlushBuffer(
        //            ContentBindEvent.Operation.AutoComplete,
        //            ReplaceText.Item2, ReplaceText.Item1
        //    );
        //    Context.Buffer.Clear();
        //}

    }
}
