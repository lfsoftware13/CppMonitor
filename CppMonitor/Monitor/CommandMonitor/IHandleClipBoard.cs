using NanjingUniversity.CppMonitor.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanjingUniversity.CppMonitor.Monitor.CommandMonitor
{
    interface IHandleClipBoard
    {
        void handleText(ILoggerDao Logger);

        void handleFileDrop(ILoggerDao Logger);

        void handleImage(ILoggerDao Logger);

        void handleAudio(ILoggerDao Logger);
    }
}
