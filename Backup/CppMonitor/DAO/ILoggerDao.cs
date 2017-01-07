using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace NanjingUniversity.CppMonitor.DAO
{
    interface ILoggerDao
    {
        Boolean LogInfo(List<KeyValuePair<String, Object>> list);

    }
}
