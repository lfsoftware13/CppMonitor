﻿using EnvDTE;
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

        public void LogInfo(TextPoint StartPoint, TextPoint EndPoint,
            ref String ReplacingText, ref String ReplacedText)
        {
            // 如果发生了文本替换事件，切换到文本替换状态
            if (ContentUtil.IsReplaceEvent(ReplacingText, ReplacedText))
            {
                Context.TransferToReplaceState(
                    StartPoint, EndPoint,
                    ref ReplacingText, ref ReplacedText
                );
                return;
            }

            // 如果发生了删除事件，切换到删除状态
            if (ContentUtil.IsDeleteEvent(ReplacingText, ReplacedText))
            {
                Context.TransferToDeleteState(
                    StartPoint, EndPoint,
                    ref ReplacingText, ref ReplacedText
                );
                return;
            }

            // 处理文本插入事件
            HandleInsertText(StartPoint, EndPoint,
                ref ReplacingText, ref ReplacedText);
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
            TextPoint EndPoint, ref String ReplacingText,
            ref String ReplacedText)
        {
            StringBuilder Buffer = Context.Buffer;

            bool FirstEdit = ContentUtil.IsFirstEdit(Context.LastEndOffset);
            bool TransferFromOthers = Buffer.Length == 0;
            if (FirstEdit || TransferFromOthers)
            {
                Context.LineBeforeFlush = StartPoint.Line;
                Context.LineOffsetBeforeFlush = StartPoint.LineCharOffset;

                Buffer.Append(ReplacingText);
            }
            else
            {
                int InsertLength = ReplacingText.Length;
                int NowOffset = EndPoint.AbsoluteCharOffset;
                int OffsetDiff = NowOffset - Context.LastEndOffset;

                //// 被插入的字符是"\r\n"，而且前后字符偏移只差1，
                //// 说明插入紧接着的是换行符，这是观察VS而得到的结论，
                //// 这种情况下，切换到InsertAfterEnterState
                //if (ContentUtil.IsTypeEnter(InsertedText, OffsetDiff))
                //{
                //    Context.TransferToInsertAfterEnterState(InsertedText);
                //    return;
                //}
                // 如果满足以下条件中的任意一个，则聚合所要插入的内容
                // 1、被插入字符长度 = 前后两次偏移之差 
                if (OffsetDiff == InsertLength)
                {
                    Buffer.Append(ReplacingText);
                }
                else
                {
                    //将回车换行汇聚到前一次插入事件 实现按照换行切分插入事件的功能 fixbug6-26:在聚合回车换行的时候要保证内容连续
                    if(StartPoint.AbsoluteCharOffset==Context.LastEndOffset&&ReplacingText.EndsWith("\n")){
                        Buffer.Append(ReplacingText);
                        FlushBuffer();
                        Context.LineBeforeFlush = EndPoint.Line;
                        Context.LineOffsetBeforeFlush = EndPoint.LineCharOffset;
                    }
                    else
                    {
                        //fixbug6-26：在将之前改变刷入日志之后记录新修改的起始位置
                        FlushBuffer();
                        Context.LineBeforeFlush = StartPoint.Line;
                        Context.LineOffsetBeforeFlush = StartPoint.LineCharOffset;
                        Buffer.Append(ReplacingText);
                    }
                    
                }
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
