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

            // TODO 判断发生了什么时间，要考虑替换事件，现在先只考虑删除事件

            // 如果发生了删除事件，切换上下文的状态
            int DeltaLength = Context.GetContentDelta(DocContent);
            if (DeltaLength < 0)
            {
                // 如果之前增加内容不为空，则清空缓存
                if (Context.Buffer.Length > 0)
                {
                    Context.FlushBuffer(ContentBindEvent.Operation.Insert);
                }

                Context.SetState(new DeleteState(Context));
                Context.ReLog(StartPoint, EndPoint);
                return;
            }

            StringBuilder Buffer = Context.Buffer;
            String InsertedText = GetInsertedText(StartPoint, EndPoint);

            //第一次编辑文本或者刚从其他状态切换过来
            if (Context.LastStartOffset == -1 || Context.Buffer.Length == 0)
            {
                Debug.Assert(Buffer.Length == 0);

                Buffer.Append(InsertedText);
            }
            else
            {
                int NowOffset = StartPoint.AbsoluteCharOffset;
                int InsertLength = InsertedText.Length;

                //如果上次也是插入事件并且插入位置连续，聚合插入内容
                if (NowOffset - Context.LastStartOffset == InsertLength)
                {
                    Context.Buffer.Append(InsertedText);
                }
                else
                {
                    Context.FlushBuffer(ContentBindEvent.Operation.Insert);
                    Context.Buffer.Append(InsertedText);
                }
            }
        }

        public void FlushBuffer()
        {
            if (Context.Buffer.Length > 0)
            {
                Context.FlushBuffer(ContentBindEvent.Operation.Insert);
            }
        }

        private String GetInsertedText(TextPoint StartPoint, TextPoint EndPoint)
        {
            EditPoint StartEdit = StartPoint.CreateEditPoint();
            String InsertedText = StartEdit.GetText(EndPoint);
            return InsertedText;
        }
    }
}
