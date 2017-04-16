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
        // 最后一次编辑的位置
        private int _LastEndOffset;

        // 聚合的编辑内容的行内偏移
        private int _LineOffsetBeforeFlush;

        // 聚合的编辑内容的所在行
        private int _LineBeforeFlush;

        // 所编辑内容的缓冲区
        private StringBuilder _Buffer;

        // 当前编辑的文档对象
        private Document _ActiveDoc;

        // 上一次编辑之后的文档内容
        private String _LastDocContent;

        public ContextState(
            int LastEndOffset, int StartOffsetBeforeFlush,
            int LineBeforeFlush, StringBuilder Buffer,
            Document ActiveDoc, String LastDocContent
        )
        {
            _LastEndOffset = LastEndOffset;
            _Buffer = Buffer;
            _ActiveDoc = ActiveDoc;
            _LastDocContent = LastDocContent;
            _LineOffsetBeforeFlush = StartOffsetBeforeFlush;
            _LineBeforeFlush = LineBeforeFlush;
        }

        public int LastEndOffset
        {
            get { return _LastEndOffset; }
            set { _LastEndOffset = value; }
        }

        public int LineOffsetBeforeFlush
        {
            get { return _LineOffsetBeforeFlush; }
            set { _LineOffsetBeforeFlush = value; }
        }

        public int LineBeforeFlush
        {
            get { return _LineBeforeFlush; }
            set { _LineBeforeFlush = value; }
        }

        public StringBuilder Buffer {
            get { return _Buffer; }
            set { _Buffer = value; }
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
