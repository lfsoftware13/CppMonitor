using NanjingUniversity.CppMonitor.Monitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanjingUniversity.CppMonitor.DTEMonitor
{
    public class DTE_Initializer
    {
        public void initializeMonitor()
        {
            String[] list = { "Build", "Command", "Content", "Debug", "File", "Key" };

            foreach (String key in list)
            {
                IBindEvent bind = MonitorFactory.monitorFactory.GetEventBinder(key);
                if (bind != null)
                {
                    bind.RegisterEvent();
                }
            }
        }
    }
}
