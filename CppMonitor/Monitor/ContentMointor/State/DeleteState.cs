﻿using EnvDTE;
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

            Tuple<String, String> ReplaceText = ContentUtil.GetReplaceText(
                StartPoint, Context.LastDocContent, DocContent
            );
            String ReplacingText = ReplaceText.Item1;
            String ReplacedText = ReplaceText.Item2;

            // 如果发生了文本替换事件，则切换到替换状态
            if (ContentUtil.IsReplaceEvent(ReplacingText, ReplacedText))
            {
                Context.TransferToReplaceState(
                    StartPoint, EndPoint, DocContent
                );
                return;
            }

            // 如果发生了插入事件，则切换到插入状态
            if (ContentUtil.IsInsertEvent(ReplacingText, ReplacedText))
            {
                Context.TransferToInsertState(
                    StartPoint, EndPoint, DocContent
                );
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

            bool FirstEdit = ContentUtil.IsFirstEdit(Context.LastEndOffset);
            bool TransferFromOthers = Buffer.Length == 0;
            if (FirstEdit || TransferFromOthers)
            {
                Buffer.Append(DelText);
            }
            else
            {
                int NowOffset = EndPoint.AbsoluteCharOffset;
                int DelLength = -Context.GetContentDelta(DocContent);
                int OffsetDiff = Context.LastEndOffset - NowOffset;

                // 如果满足以下条件中的任意一个，则聚合所要删除的内容
                // 1、被删除字符长度 = 前后两次偏移之差
                // 2、被删除的字符是"\r\n"，而且前后偏移字符只差为1，
                //    说明删除紧接着的是换行符，这是观察VS而得到的结论
                if (OffsetDiff == DelLength || ContentUtil.IsTypeEnter(DelText, OffsetDiff))
                {
                    Buffer.Insert(0, DelText);
                }
                else
                {
                    FlushBuffer();
                    Buffer.Append(DelText);
                }
            }

            Context.LineBeforeFlush = StartPoint.Line;
            Context.LineOffsetBeforeFlush = StartPoint.LineCharOffset;
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
