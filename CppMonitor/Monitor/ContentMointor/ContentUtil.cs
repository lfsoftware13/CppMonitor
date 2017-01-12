using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanjingUniversity.CppMonitor.Monitor.ContentMointor
{
    class ContentUtil
    {
        /**
         * 判断用户是否输入了回车键
         */
        public static bool IsTypeEnter(String Text, int OffsetDiff)
        {
            return Text.Equals("\r\n") && OffsetDiff == 1;
        }

        /**
         * 判断是否发生了删除事件
         */
        public static bool IsDeleteEvent(String ReplacingText, String ReplacedText)
        {
            return ReplacingText.Length == 0 && ReplacedText.Length > 0;
        }

        /**
         * 判断是否发生了插入事件
         */
        public static bool IsInsertEvent(String ReplacingText, String ReplacedText)
        {
            return ReplacingText.Length > 0 && ReplacedText.Length == 0;
        }

        /**
         * 判断是否发生了替换事件
         */
        public static bool IsReplaceEvent(String ReplacingText, String ReplacedText)
        {
            return ReplacingText.Length > 0 && ReplacedText.Length > 0;
        }

        public static bool IsFirstEdit(int Offset)
        {
            return Offset == -1;
        }

        /**
         * 获得前后两次文本快照的差异
         * 返回的元组中第一项为替换的文本，
         * 第二项为被替换的文本
         */
        public static Tuple<String, String> GetReplaceText(TextPoint StartPoint,
            String LastDoc, String CurrentDoc)
        {
            int Start = GetCharOffset(StartPoint);
            int OldLength = LastDoc.Length;
            int NewLength = CurrentDoc.Length;

            if (Start >= OldLength || Start >= NewLength)
            {
                return new Tuple<string, string>(
                    Start >= NewLength ? String.Empty : CurrentDoc.Substring(Start),
                    Start >= OldLength ? String.Empty : LastDoc.Substring(Start)
                );
            }

            // 截去两个文本后面相同的部分
            String OldDoc = LastDoc.Substring(Start);
            String NewDoc = CurrentDoc.Substring(Start);
            int OldIndex = OldDoc.Length - 1;
            int NewIndex = NewDoc.Length - 1;
            while (OldIndex >= 0 && NewIndex >= 0)
            {
                if (OldDoc[OldIndex] != NewDoc[NewIndex]) break;
                --OldIndex;
                --NewIndex;
            }

            return new Tuple<string, string>(
                NewDoc.Substring(0, NewIndex + 1),
                OldDoc.Substring(0, OldIndex + 1)
            );
        }

        public static String GetInsertedText(TextPoint StartPoint, TextPoint EndPoint)
        {
            EditPoint StartEdit = StartPoint.CreateEditPoint();
            String InsertedText = StartEdit.GetText(EndPoint);
            return InsertedText;
        }

        /**
         * 根据TextPoint或者字符在文本快照字符串中的下标
         */
        public static int GetCharOffset(TextPoint StartPoint)
        {
            // 观察VS得到的结论
            return StartPoint.AbsoluteCharOffset + StartPoint.Line - 2;
        }
    }
}
