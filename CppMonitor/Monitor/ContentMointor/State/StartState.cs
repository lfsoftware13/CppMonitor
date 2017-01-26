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

        public void LogInfo(TextPoint StartPoint, TextPoint EndPoint,
            ref String ReplacingText, ref String ReplacedText)
        {
            if (ContentUtil.IsDeleteEvent(ReplacingText, ReplacedText))
            {
                Context.TransferToDeleteState(
                    StartPoint, EndPoint,
                    ref ReplacingText, ref ReplacedText
                );
            }
            else if (ContentUtil.IsInsertEvent(ReplacingText, ReplacedText))
            {
                Context.TransferToInsertState(
                    StartPoint, EndPoint,
                    ref ReplacingText, ref ReplacedText
                );
            }
            else if (ContentUtil.IsReplaceEvent(ReplacingText, ReplacedText))
            {
                Context.TransferToReplaceState(
                    StartPoint, EndPoint,
                    ref ReplacingText, ref ReplacedText
                );
            }

        }

        public void FlushBuffer()
        {
            return;
        }
    }
}
