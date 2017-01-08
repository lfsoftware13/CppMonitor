using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanjingUniversity.CppMonitor.Monitor.BuildMonitor
{
    class BuildBindEvent : IBindEvent
    {
        void IBindEvent.RegisterEvent()
        {
            ProjectEngineRegister engine = ProjectEngineRegister.projectEngineRegister;
            engine.Register();

            DteBuildRegister build = DteBuildRegister.dteBuildRegister;
            build.Register();
        }
    }
}
