using EnvDTE;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanjingUniversity.CppMonitor.Monitor.ContentMointor.State
{
    class DeleteState : IEditState
    {
        private ContentBindEvent Context;

        public DeleteState(ContentBindEvent Context)
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

            // 如果发生了文本替换事件，则切换到替换状态
            Tuple<String, String> ReplaceText = Context.GetReplaceText(StartPoint, DocContent);
            if (ReplaceText.Item2.Length > 0 && ReplaceText.Item1.Length > 0)
            {
                Context.TransferToReplaceState(
                    StartPoint, ReplaceText.Item2, ReplaceText.Item1
                );
                return;
            }

            // 如果发生了插入事件，则切换到插入状态状态
            if (ReplaceText.Item1.Length > 0)
            {
                Context.TransferToInsertState(StartPoint, EndPoint);
                return;
            }

            // 对删除事件进行处理
            HandleDeleteText(StartPoint, EndPoint, DocContent);
        }

        public void FlushBuffer()
        {
            if (Context.Buffer.Length > 0)
            {
                Context.FlushBuffer(
                    ContentBindEvent.Operation.Delete,
                    Context.Buffer.ToString(),
                    String.Empty
                );
            }
        }

        private void HandleDeleteText(TextPoint StartPoint,
            TextPoint EndPoint, String DocContent)
        {
            String DelText = GetDeletedText(StartPoint, DocContent);
            StringBuilder Buffer = Context.Buffer;

            // 第一次编辑文本或者刚从其他状态切换过来
            if (Context.LastStartOffset == -1 || Context.Buffer.Length == 0)
            {
                Debug.Assert(Buffer.Length == 0);

                Buffer.Append(DelText);
            }
            else
            {
                int NowOffset = StartPoint.AbsoluteCharOffset;
                int DelLength = -Context.GetContentDelta(DocContent);
                int OffsetDiff = Context.LastStartOffset - NowOffset;

                // 如果满足以下条件中的任意一个，则聚合所要删除的内容
                // 1、被删除字符长度 = 前后两次偏移之差
                // 2、被删除的字符是"\r\n"，而且前后偏移字符只差为1，
                //    说明删除紧接着的是换行符，这是观察VS而得到的结论
                if (OffsetDiff == DelLength || Context.IsEnterFollow(DelText, OffsetDiff))
                {
                    Buffer.Insert(0, DelText);
                }
                else
                {
                    Context.FlushBuffer(
                        ContentBindEvent.Operation.Delete,
                        Context.Buffer.ToString(),
                        String.Empty
                    );
                    Buffer.Append(DelText);
                }
            }

            Context.StartOffsetBeforeFlush = StartPoint.AbsoluteCharOffset;
        }

        private String GetDeletedText(TextPoint StartPoint, String CurrentDoc)
        {
            String[] Lines = Context.LastDocContent.Split(new char[] { '\n' });
            int StartOffset = StartPoint.LineCharOffset;
            int StartLine = StartPoint.Line;
            int DelCharNum = -Context.GetContentDelta(CurrentDoc);

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

    }
}
