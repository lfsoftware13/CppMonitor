using MonitorInterop.ServiceInterop;
using NanjingUniversity.CppMonitor.Common;
using NanjingUniversity.CppMonitor.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NanjingUniversity.CppMonitor.ServiceInterop
{
    class MonitorService : IMonitorService, SMonitorService
    {

        private Microsoft.VisualStudio.OLE.Interop.IServiceProvider serviceProvider;

        public MonitorService(Microsoft.VisualStudio.OLE.Interop.IServiceProvider sp)
        {
            serviceProvider = sp;
        }


        public void ClearMonitorLog()
        {
            DBHelper dbHelper = DBHelper.getInstance();
            dbHelper.clearLog("TableName");
        }

        public string BeforeUpload()
        {
            DBHelper dbHelper = DBHelper.getInstance();
            return GetMonitorDBPath();
        }

        public string GetMonitorDBPath()
        {
            return AddressCommon.DBFilePath; 
        }

        public string GetFileModulePath()
        {
            return AddressCommon.FileModuleRootPath;
        }
    }
}
