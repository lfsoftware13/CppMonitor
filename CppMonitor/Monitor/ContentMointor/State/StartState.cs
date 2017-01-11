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

            if (Context.GetContentDelta(DocContent) < 0)
            {
                Context.SetState(new DeleteState(Context));
            }
            else
            {
                Context.SetState(new InsertState(Context));
            }

            // 重新响应事件
            Context.ReLog(StartPoint, EndPoint);
        }

        public void FlushBuffer()
        {
            return;
        }
    }
}
