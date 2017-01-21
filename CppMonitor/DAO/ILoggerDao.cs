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
        Boolean LogInfo(string target,List<KeyValuePair<String, Object>> list);

        void clearLog();
        //确保日志文件是否存在
        void ensureTableExist();
    }
}
