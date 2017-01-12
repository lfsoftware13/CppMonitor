﻿using EnvDTE;
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
        private int _LastStartOffset;

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
            int LastStartOffset, int StartOffsetBeforeFlush,
            int LineBeforeFlush, StringBuilder Buffer,
            Document ActiveDoc, String LastDocContent
        )
        {
            _LastStartOffset = LastStartOffset;
            _Buffer = Buffer;
            _ActiveDoc = ActiveDoc;
            _LastDocContent = LastDocContent;
            _LineOffsetBeforeFlush = StartOffsetBeforeFlush;
            _LineBeforeFlush = LineBeforeFlush;
        }

        public int LastStartOffset
        {
            get { return _LastStartOffset; }
            set { _LastStartOffset = value; }
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
    }
}
