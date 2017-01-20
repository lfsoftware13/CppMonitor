﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NanjingUniversity.CppMonitor.DAO.imp;

namespace NanjingUniversity.CppMonitor.DAO
{
    class LoggerFactory
    {
        private static LoggerFactory _LoggerFactory;

        public static LoggerFactory loggerFactory
        {
            get
            {
                if (_LoggerFactory == null)
                {
                    _LoggerFactory = new LoggerFactory();
                }
                return _LoggerFactory;
            }
        }

        private LoggerFactory() { }


        public ILoggerDao getLogger(String key)
        {
            ILoggerDao logger = null;
            switch(key){
                case "Build":
                     logger = new BuildLoggerImpl();
                     break;
                case "Content":
                     logger = new ContentLoggerImpl();
                     break;
                case "File":
                     logger = new FileLoggerImpl();
                     break;
                default:
                     logger = null;
                     break;
            }
            return logger;
        }

    }
}
