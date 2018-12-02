using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpppMonitor.DTE.Monitor.BuildMonitor.Model
{
    /// <summary>
    /// 这个类 与 EnvDTE80.ErrorItem 功能一致
    /// 这边只保留一些对于后续分析有意义的字段
    /// </summary>
    public class ErrorListItem
    {
        public int ErrorLevel
        {
            get;
            set;
        }

        public String Description
        {
            get;
            set;
        }

        public String Project
        {
            get;
            set;
        }

        public String FileName
        {
            get;
            set;
        }

        public int Line
        {
            get;
            set;
        }

        public int Column
        {
            get;
            set;
        }
    }
}
