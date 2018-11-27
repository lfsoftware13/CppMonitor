using MonitorInterop.ServiceInterop;
using NanjingUniversity.CppMonitor.Common;
using NanjingUniversity.CppMonitor.DAO;

namespace NanjingUniversity.CppMonitor.ServiceInterop
{
    public class MonitorService : IMonitorService, SMonitorService
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
            string pa = AddressCommon.getCommonAppDataPath();
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

        public string GetMonitorPath()
        {
            return AddressCommon.getAppDataPath();
        }

        public string GetBrowserFilePath()
        {
            return AddressCommon.ChromePath;
        }


    }
}
