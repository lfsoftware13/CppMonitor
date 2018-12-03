using NanjingUniversity.CppMonitor.Util.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanjingUniversity.CppMonitor.Util.Util
{
    public class SolutionUtil
    {
        public static string getSolutionName()
        {
            string solutionName = ConstantCommon.UNKNOWN_SOLUTIONNAME;
            try
            {
                string solutionFullPath = PersistentObjectManager.dte2.Solution.FullName;

                int lindex = solutionFullPath.LastIndexOf("\\");
                int diff = solutionFullPath.LastIndexOf(".") - lindex;
                solutionName = solutionFullPath.Substring(lindex + 1, diff - 1);
                
            }
            catch (Exception)
            {
                //处理Solution为null的情况
                solutionName = ConstantCommon.UNKNOWN_SOLUTIONNAME;
            }
            return solutionName;
        }
    }
}
