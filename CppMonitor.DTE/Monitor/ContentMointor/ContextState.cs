using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanjingUniversity.CppMonitor.Monitor.ContentMointor
{
    class ContextState
    {
        // 当前编辑的文档对象
        private Document _ActiveDoc;

        // 上一次编辑之后的文档内容
        private String _LastDocContent;

        public ContextState(
            Document ActiveDoc, String LastDocContent
        )
        {
            _ActiveDoc = ActiveDoc;
            _LastDocContent = LastDocContent;
        }

        public Document ActiveDoc
        {
            get { return _ActiveDoc; }
            set { _ActiveDoc = value; }
        }

        public String LastDocContent
        {
            get { return _LastDocContent; }
            set { _LastDocContent = value; }
        }

        #region 记录变化发生的时间
        private long happentime;

        public long HappenTime
        {
            get
            {
                return happentime;
            }
            set
            {
                happentime = value;
            }
        }
        #endregion
    }
}
