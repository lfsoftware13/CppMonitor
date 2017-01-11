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

        // 回车之后第一次输入的长度
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

            // 如果满足以下条件中的任意一个，则聚合所要插入的内容
            // 1、被插入字符长度 = 前后两次偏移之差
            // 2、被插入的字符是"\r\n"，而且前后字符偏移只差1，
            //    说明插入紧接着的是换行符，这是观察VS而得到的结论，
            // 3、被插入的字符是一个字符，而且前面全部是空白符，
            //    同时前后字符偏移只差1，这是观察VS而得出的结论
            int NowOffset = StartPoint.AbsoluteCharOffset;
            int OffsetDiff = NowOffset - Context.LastStartOffset;
            String InsertedText = Context.GetInsertedText(StartPoint, EndPoint);

            if (DeltaLength == OffsetDiff ||
                Context.IsEnterFollow(InsertedText, OffsetDiff) ||
                isTextAfterEnter(InsertedText, OffsetDiff))
            {
                Context.Buffer.Append(InsertedText);
            }
            else
            {
                Context.TransferToInsertState(StartPoint, EndPoint);
            }
        }

        public void FlushBuffer()
        {

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
