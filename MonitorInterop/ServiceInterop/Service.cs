using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace MonitorInterop.ServiceInterop
{
    //[Guid(GUIDs.guidMyService)]
    [ComVisible(true)]
    public interface IMonitorService
    {
        void ClearMonitorLog();

        string BeforeUpload();

        string GetMonitorDBPath();

        string GetFileModulePath();

        string GetMonitorPath();

        string GetBrowserFilePath();

    }

    //[Guid(GUIDs.guidMyServiceString)]
    public interface SMonitorService
    {
    }
    
}
