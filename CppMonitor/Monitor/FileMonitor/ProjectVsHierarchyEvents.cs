using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanjingUniversity.CppMonitor.Monitor.FileMonitor
{
    class ProjectVsHierarchyEvents : IVsHierarchyEvents 
    {
        IVsHierarchy _pHierarchy = null;
        Object _name;
        public ProjectVsHierarchyEvents(IVsHierarchy pHierarchy)
        {
            this._pHierarchy = pHierarchy;
            _pHierarchy.GetProperty((uint) VSConstants.VSITEMID.Root, (int) __VSHPROPID.VSHPROPID_Caption, out _name);
        }
        public int OnInvalidateIcon(IntPtr hicon)
        {
            return 0;
        }

        public int OnInvalidateItems(uint itemidParent)
        {
            return 0;
        }

        public int OnItemAdded(uint itemidParent, uint itemidSiblingPrev, uint itemidAdded)
        {
            return 0;
        }

        public int OnItemDeleted(uint itemid)
        {
            return 0;
        }

        public int OnItemsAppended(uint itemidParent)
        {
            return 0;
        }

        public int OnPropertyChanged(uint itemid, int propid, uint flags)
        {
            if (itemid == (uint)VSConstants.VSITEMID.Root && propid == (int)__VSHPROPID.VSHPROPID_Caption)
            {
                Object newName = null;
                _pHierarchy.GetProperty(itemid, propid, out newName);
                FileLogUtil.logFileEvent(7,newName as string,_name as string,_name as string);
                _name = newName;
            }
           
            return 0; 
        }
    }
}
