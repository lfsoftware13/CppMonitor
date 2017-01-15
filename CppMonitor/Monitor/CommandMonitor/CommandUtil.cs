using System;
using EnvDTE;
using EnvDTE80;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanjingUniversity.CppMonitor.Monitor.CommandMonitor
{
    class CommandUtil
    {
        public CommandUtil()
        {

        }
        public string getDocContent(ProjectItem projectItem)
        {
            Document temp = projectItem.Document;
            if (temp != null)
            {
                TextDocument txt = (TextDocument)temp.Object("TextDocument");
                EditPoint DocStart = txt.StartPoint.CreateEditPoint();
                return DocStart.GetText(txt.EndPoint);
            }
            else
            {
                return null;
            }
        }

        public string ToString(byte[] bytes)
        {
            string response = string.Empty;

            foreach (byte b in bytes)
                response += (Char)b;

            return response;
        }
    }
}
