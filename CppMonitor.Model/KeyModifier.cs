using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CppMonitor.Model
{
    public class KeyModifier
    {
        public bool containsAlt;
        public bool containsControl;
        public bool containsShift;
        public bool containsWindows;

        public bool containsCaps;

        public KeyModifier()
        {
            containsAlt = false;
            containsControl = false;
            containsShift = false;
            containsWindows = false;

            containsCaps = false;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(containsAlt?1:0);
            stringBuilder.Append(containsControl ? 1:0);
            stringBuilder.Append(containsShift ? 1:0);
            stringBuilder.Append(containsWindows ? 1:0);
            stringBuilder.Append(containsCaps ? 1:0);

            return stringBuilder.ToString();
        }
    }
}
