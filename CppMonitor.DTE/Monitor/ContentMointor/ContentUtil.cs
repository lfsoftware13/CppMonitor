using EnvDTE;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        /**
         * 判断文档是否是第一次编辑
         */
        public static bool IsFirstEdit(int Offset)
        {
            return Offset == -1;
        }

        /**
         * 判断当前文档是否是cpp文件或头文件
         */
        public static bool IsCppFile(String name)
        {
            try
            {
                String[] temp = name.Split(new char[] { '.' });
                return temp[1].Equals("h") || temp[1].Equals("cpp");
            }
            catch (Exception)
            {
                return false;
            }
        }

        /**
         * 获得前后两次文本快照的差异
         * 返回的元组中第一项为替换的文本，
         * 第二项为被替换的文本
         */
        public static Tuple<String, String> GetReplaceText(TextPoint StartPoint, TextPoint EndPoint,
            String LastDoc, String CurrentDoc)
        {
            int Start = GetCharOffset(StartPoint);
            int End = GetCharOffset(EndPoint);

            //处理边界异常
            if (LastDoc == null) {
                LastDoc = "";
            }
            int OldLength = LastDoc.Length;
            int NewLength = CurrentDoc.Length;

            //Debug.WriteLine("start:{0}:end:{1}:OldLength:{2}:NewLength:{3}",Start,End,OldLength,NewLength);

            return new Tuple<string, string>(
                CurrentDoc.Substring(Start, End - Start),
                LastDoc.Substring(Start,OldLength - (NewLength - End) - Start)
                );
        }

        /**
         * 根据TextPoint或者字符在文本快照字符串中的下标
         */
        public static int GetCharOffset(TextPoint StartPoint)
        {
            // 在textView中回车换行算一个字符，所以需要为每一个之前的行增加一个回车字符，Line是one-based
            return StartPoint.AbsoluteCharOffset + StartPoint.Line - 2;
        }

        public static String ToUTF8(String Str)
        {
            Encoding U16 = Encoding.Unicode;
            Encoding UTF8 = Encoding.UTF8;
            byte[] U16Bytes = U16.GetBytes(Str);
            byte[] UTF8Bytes = Encoding.Convert(U16, UTF8, U16Bytes);
            return UTF8.GetString(UTF8Bytes);
        }
    }
}
