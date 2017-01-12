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

            // 如果发生了文本删除事件，则切换到删除状态
            int DeltaLength = Context.GetContentDelta(DocContent);
            if (DeltaLength < 0) {
                Context.TransferToDeleteState(StartPoint, EndPoint);
                return;
            }

            int NowOffset = StartPoint.AbsoluteCharOffset;
            int OffsetDiff = NowOffset - Context.LastStartOffset;
            String InsertedText = Context.GetInsertedText(StartPoint, EndPoint);
    
            // 如果满足以下条件中的任意一个，则聚合所要插入的内容
            // 1、被插入字符长度 = 前后两次偏移之差
            // 2、被插入的字符是"\r\n"，而且前后字符偏移只差1，
            //    说明插入紧接着的是换行符，这是观察VS而得到的结论，
            // 3、被插入的字符是一个字符，而且前面全部是空白符，
            //    同时前后字符偏移只差1，说明这是换行之后插入的第一个字符，
            //    这是观察VS而得出的结论
            // 4、倍插入的字符是一个字符，字符偏移等于换行后第一次插入
            //    (包括空白符）的长度，说明这是换行之后插入的第二个字符，
            //    这是观察VS而得出的结论
            ++InsertNum;
            if (InsertNum == 1)
            {
                FirstLength = InsertedText.Length;
                if (DeltaLength == OffsetDiff ||
                    Context.IsEnterFollow(InsertedText, OffsetDiff) ||
                    isTextAfterEnter(InsertedText, OffsetDiff))
                {
                    Context.Buffer.Append(InsertedText);
                }
                else
                {
                    TransferToInsertState(StartPoint, EndPoint);
                }
            }
            else if (InsertNum == 2)
            {
                if (FirstLength == OffsetDiff && InsertedText.Length == 1)
                {
                    Context.Buffer.Append(InsertedText);
                }
                else
                {
                    TransferToInsertState(StartPoint, EndPoint);
                }
            }
            else
            {
                TransferToInsertState(StartPoint, EndPoint);
            }

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

        private void TransferToInsertState(TextPoint StartPoint, TextPoint EndPoint)
        {
            Context.SetState(new InsertState(Context));
            Context.ReLog(StartPoint, EndPoint);
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

        private bool isWhiteSpace(char c)
        {
            return c == ' ' || c == '\t' || c == '\n';
        }
    }
}
