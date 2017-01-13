using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanjingUniversity.CppMonitor.Monitor.ContentMointor.State
{
    class StartState : IEditState
    {
        private ContentBindEvent Context;

        public StartState(ContentBindEvent Context)
        {
            this.Context = Context;
        }

        public void LogInfo(TextPoint StartPoint, TextPoint EndPoint, String DocContent)
        {
            if (StartPoint == null || EndPoint == null || DocContent == null)
            {
                return;
            }

            Tuple<string, string> ReplaceText = ContentUtil.GetReplaceText(
                StartPoint, Context.LastDocContent, DocContent
            );
            String ReplacingText = ReplaceText.Item1;
            String ReplacedText = ReplaceText.Item2;

            if (ContentUtil.IsDeleteEvent(ReplacingText, ReplacedText))
            {
                Context.TransferToDeleteState(
                    StartPoint, EndPoint, DocContent
                );
            }
            else if (ContentUtil.IsInsertEvent(ReplacingText, ReplacedText))
            {
                Context.TransferToInsertState(
                    StartPoint, EndPoint, DocContent
                );
            }
            else if (ContentUtil.IsReplaceEvent(ReplacingText, ReplacedText))
            {
                Context.TransferToReplaceState(
                    StartPoint, EndPoint, DocContent
                );
            }

        }

        public void FlushBuffer()
        {
            return;
        }
    }
}
