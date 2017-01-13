using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanjingUniversity.CppMonitor.Monitor.ContentMointor.State
{
    class InsertAfterEnterState : IEditState
    {
        private ContentBindEvent Context;

        // 在这个状态下第几次发生了插入事件
        private int InsertNum = 0;

        // 回车之后的第一次输入的长度
        private int FirstLength = 0;

        public InsertAfterEnterState(ContentBindEvent Context)
        {
            this.Context = Context;
        }

        public void LogInfo(TextPoint StartPoint,
            TextPoint EndPoint, String DocContent)
        {
            if (StartPoint == null || EndPoint == null || DocContent == null)
            {
                FlushBuffer();
                return;
            }

            Tuple<string, string> ReplaceText = ContentUtil.GetReplaceText(
                StartPoint, Context.LastDocContent, DocContent
            );
            String ReplacingText = ReplaceText.Item1;
            String ReplacedText = ReplaceText.Item2;

            // 如果发生了文本删除事件，则切换到删除状态
            if (ContentUtil.IsDeleteEvent(ReplacingText, ReplacedText)) {
                Context.TransferToDeleteState(
                    StartPoint, EndPoint, DocContent
                );
                return;
            }

            // 如果发生了文本替换事件，则切换到文本替换状态
            if (ContentUtil.IsReplaceEvent(ReplacingText, ReplacedText)) {
                Context.TransferToReplaceState(StartPoint, ReplacingText, ReplacedText);
                return;
            }

            // 处理文本插入事件
            HandleInsertText(StartPoint, EndPoint, DocContent);
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

        private void HandleInsertText(TextPoint StartPoint,
            TextPoint EndPoint, String DocContent)
        {
            int NowOffset = StartPoint.AbsoluteCharOffset;
            int OffsetDiff = NowOffset - Context.LastEndOffset;
            int DeltaLength = Context.GetContentDelta(DocContent);
            String InsertedText = ContentUtil.GetInsertedText(StartPoint, EndPoint);

            // 如果满足以下条件中的任意一个，则聚合所要插入的内容
            // 1、被插入字符长度 = 前后两次偏移之差
            // 2、被插入的字符是"\r\n"，而且前后字符偏移只差1，
            //    说明插入紧接着的是换行符，这是观察VS而得到的结论，
            // 3、被插入的字符是一个字符，而且前面全部是空白符，
            //    同时前后字符偏移只差1，说明这是换行之后插入的第一个字符，
            //    这是观察VS而得出的结论
            // 4、被插入的字符是一个字符，字符偏移等于换行后第一次插入
            //    (包括空白符）的长度，说明这是换行之后插入的第二个字符，
            //    这是观察VS而得出的结论
            ++InsertNum;
            if (InsertNum == 1)
            {
                FirstLength = InsertedText.Length;
                if (IsFirstCharAfterEnter(InsertedText, OffsetDiff))
                {
                    Context.Buffer.Append(InsertedText);
                }
                else
                {
                    TransferToInsertState(StartPoint, EndPoint, DocContent);
                }
            }
            else if (InsertNum == 2)
            {
                if (IsSecondCharAfterEnter(OffsetDiff))
                {
                    Context.Buffer.Append(InsertedText);
                }
                else
                {
                    TransferToInsertState(StartPoint, EndPoint, DocContent);
                }
            }
            else
            {
                TransferToInsertState(StartPoint, EndPoint, DocContent);
            }
        }

        private void TransferToInsertState(TextPoint StartPoint,
            TextPoint EndPoint, String DocContent)
        {
            Context.SetState(new InsertState(Context));
            Context.ReLog(StartPoint, EndPoint, DocContent);
        }

        private bool isTextAfterEnter(String InsertedText, int OffsetDiff)
        {
            int End = InsertedText.Length - 1;
            for (int i = 0; i < End; ++i)
            {
                if (!isWhiteSpace(InsertedText[i])) return false;
            }
            return OffsetDiff == 1;
        }

        private bool IsFirstCharAfterEnter(String InsertedText, int OffsetDiff)
        {
            return ContentUtil.IsTypeEnter(InsertedText, OffsetDiff)
                || isTextAfterEnter(InsertedText, OffsetDiff);
        }

        private bool IsSecondCharAfterEnter(int OffsetDiff)
        {
            return FirstLength == OffsetDiff;
        }

        private bool isWhiteSpace(char c)
        {
            return c == ' ' || c == '\t' || c == '\n';
        }
    }
}
