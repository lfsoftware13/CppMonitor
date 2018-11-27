using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanjingUniversity.CppMonitor.Monitor.ContentMointor.State
{
    interface IEditState
    {
        void LogInfo(TextPoint StartPoint, TextPoint EndPoint,
            ref String ReplacingText, ref String ReplacedText);

        void FlushBuffer();
    }
}
