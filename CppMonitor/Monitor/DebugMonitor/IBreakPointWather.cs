using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanjingUniversity.CppMonitor.Monitor.DebugMonitor
{
    interface IBreakPointWather
    {
        void OnBreakPointAdded();

        void OnBreakPointRemoved();

        void OnBreakPointEnabled();

        void OnBreakPointDisabled();
    }
}
