using NanjingUniversity.CppMonitor.Monitor.BuildMonitor;
using NanjingUniversity.CppMonitor.Monitor.CommandMonitor;
using NanjingUniversity.CppMonitor.Monitor.ContentMointor;
using NanjingUniversity.CppMonitor.Monitor.DebugMonitor;
using NanjingUniversity.CppMonitor.Monitor.FileMonitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanjingUniversity.CppMonitor.Monitor
{
    class MonitorFactory
    {
        private static MonitorFactory _MonitorFactory;

        public static MonitorFactory monitorFactory
        {
            get
            {
                if (_MonitorFactory == null)
                {
                    _MonitorFactory = new MonitorFactory();
                }
                return _MonitorFactory;
            }
        }

        private MonitorFactory() { }

        public IBindEvent GetEventBinder(String key)
        {
            IBindEvent res=null;
            switch(key){
                case "Build":
                    res = new BuildBindEvent();
                    break;
                case "Command":
                    res = new CommandBindEvent();
                    break;
                case "Content":
                    res = new ContentBindEvent();
                    break;
                case "Debug":
                    res = new DebugBindEvent();
                    break;
                case "File":
                    res = new FileBindEvent();
                    break;
                default:
                    break;
            }
            return res;
        }

    }
}
