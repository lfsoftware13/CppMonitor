using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanjingUniversity.CppMonitor.Util.Common
{
    public class PersistentObjectManager
    {
        private static object objLock = new object();

        private static DTE2 _dte2 = null;

        private static bool _isLogOn = false;


        /// <summary>
        /// getter and setter
        /// </summary>
        public static DTE2 dte2
        {
            get
            {
                if (_dte2 == null)
                {
                    lock (objLock)
                    {
                        if (_dte2 == null)
                        {
                            _dte2 = ServiceProvider.GlobalProvider.GetService(typeof(SDTE)) as DTE2;
                        }
                    }
                }
                return _dte2;
            }
        }
        
        public static bool isLogOn
        {
            set
            {
                lock (objLock)
                {
                    _isLogOn = value;
                }
            }

            get
            {
                lock (objLock)
                {
                    return _isLogOn;
                }
            }
        }
    }
}
