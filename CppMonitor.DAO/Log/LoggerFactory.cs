using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NanjingUniversity.CppMonitor.DAO.decorator;
using NanjingUniversity.CppMonitor.DAO.imp;

namespace NanjingUniversity.CppMonitor.DAO
{
    public class LoggerFactory
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
            ILoggerDao subLogger = null;
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
                case "Command":
                     logger = new CommandLoggerImpl();
                     break;
                case "Debug":
                     logger = new DebugLoggerImpl();
                     break;
                case "Key":
                    subLogger = new KeyLoggerImpl();
                    logger = new KeyLoggerDecorator(subLogger);
                    break;
                case "Summary":
                    logger = new SummaryLoggerImpl();
                    break;
                default:
                     logger = null;
                     break;
            }
            return logger;
        }

    }
}
