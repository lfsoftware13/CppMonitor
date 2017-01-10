using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NanjingUniversity.CppMonitor.DAO.imp
{
    class ILoggerDaoImpl_stub :ILoggerDao
    {
        private StreamWriter Writer;

        public ILoggerDaoImpl_stub()
        {
            try
            {
                Writer = new StreamWriter(new FileStream(
                               "C:/Users/Shura/Desktop/database.txt", FileMode.OpenOrCreate | FileMode.Append
                           ));
                Writer.WriteLine("#" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                Writer.Flush();
            }
            catch (IOException)
            {

            }
        }

        ~ILoggerDaoImpl_stub()
        {
            try
            {
                if (Writer != null)
                {
                    Writer.Close();
                }
            }
            catch (IOException)
            {

            }
        }

        public Boolean LogInfo(List<KeyValuePair<String, Object>> list)
        {
            foreach(KeyValuePair<String, Object> one in list) {
                Writer.WriteLine(one.Key + " : " + one.Value.ToString());
            }

            Writer.Flush();
            
            return true;
        }
            
        
    }
}
