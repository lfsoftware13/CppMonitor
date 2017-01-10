using NanjingUniversity.CppMonitor.DAO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NanjingUniversity.CppMonitor.Monitor.ContentMointor
{
    class LoggerDAOImpl_Stub : ILoggerDao
    {
        private StreamWriter Writer;

        private const String PATH = "C:/Users/Y481L/Desktop/temp.txt";

        private const FileMode FILE_MODE = FileMode.Append | FileMode.OpenOrCreate;

        public LoggerDAOImpl_Stub()
        {
            Writer = new StreamWriter(new FileStream(PATH, FILE_MODE));
        }

        ~LoggerDAOImpl_Stub()
        {
            if (Writer != null) Writer.Close();
        }

        public Boolean LogInfo(List<KeyValuePair<String, Object>> list)
        {
            foreach(KeyValuePair<String, Object> pair in list) {
                Writer.WriteLine(pair.Key + " : " + pair.Value.ToString());
            }

            Writer.Flush();
            
            return true;
        }
    }
}
