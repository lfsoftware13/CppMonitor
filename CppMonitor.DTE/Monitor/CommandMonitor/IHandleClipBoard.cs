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
        void handleText();

        void handleFileDrop();

        void handleImage();

        void handleAudio();

        void handleVSProjectItem();
    }
}
