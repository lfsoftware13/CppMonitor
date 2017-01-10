using NanjingUniversity.CppMonitor.Monitor.BuildMonitor.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanjingUniversity.CppMonitor.Monitor.BuildMonitor
{
    class BuildBindEvent : IBindEvent
    {
        public static BuildMonitorManager _Manager;
        public static BuildMonitorManager Manager
        {
            get
            {
                if (_Manager == null)
                {
                    _Manager = new BuildMonitorManager();
                }
                return _Manager;
            }
            set
            {
                _Manager = value;
            }
        }

        void IBindEvent.RegisterEvent()
        {
            ProjectEngineRegister engine = ProjectEngineRegister.projectEngineRegister;
            engine.Register();

            DteBuildRegister build = DteBuildRegister.dteBuildRegister;
            build.Register();
        }
    }
}
