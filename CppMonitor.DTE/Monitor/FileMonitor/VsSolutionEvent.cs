using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanjingUniversity.CppMonitor.Monitor.FileMonitor
{
    class VsSolutionEvents : IVsSolutionEvents3,IVsSolutionEvents4
    {

        public int OnAfterCloseSolution(object pUnkReserved)
        {
            return 0;
        }

        public int OnAfterClosingChildren(IVsHierarchy pHierarchy)
        {
            return 0;
        }

        public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
        {
            return 0;
        }

        public int OnAfterMergeSolution(object pUnkReserved)
        {
            return 0;
        }

        public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            uint result = 0;
            pHierarchy.AdviseHierarchyEvents(new ProjectVsHierarchyEvents(pHierarchy), out result);
            return 0;
        }

        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            return 0;
        }

        public int OnAfterOpeningChildren(IVsHierarchy pHierarchy)
        {
            return 0;
        }

        public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
        {
            return 0;
        }

        public int OnBeforeCloseSolution(object pUnkReserved)
        {
            return 0;
        }

        public int OnBeforeClosingChildren(IVsHierarchy pHierarchy)
        {
            return 0;
        }

        public int OnBeforeOpeningChildren(IVsHierarchy pHierarchy)
        {
            return 0;
        }

        public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
        {
            return 0;
        }

        public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
        {
            return 0;
        }

        public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
        {
            return 0;
        }

        public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
        {
            return 0;
        }

        public int OnAfterAsynchOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            return 0;
        }

        public int OnAfterChangeProjectParent(IVsHierarchy pHierarchy)
        {
            return 0;
        }

        public int OnAfterRenameProject(IVsHierarchy pHierarchy)
        {
            //uint result = 0;
            //pHierarchy.AdviseHierarchyEvents(new VsHierarchyEvents(pHierarchy), out result);
            return 0;
        }

        public int OnQueryChangeProjectParent(IVsHierarchy pHierarchy, IVsHierarchy pNewParentHier, ref int pfCancel)
        {
            return 0;
        }
    }
}
