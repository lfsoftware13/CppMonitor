using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanjingUniversity.CppMonitor.Util.Common
{
    public enum SolutionAction
    {
        solutionOpen,
        solutionClose,
        solutionRename,
        solAddProject,
        solDelProject,
        solRenameProject,
    }

    public enum DebugAction
    {
        debugStart,
        debugBreak,
        debugContinue,
        debugExit,
        debugException,
        debugExpNothandle,
    }

    public enum BreakpointAction
    {
        bpAdd,
        bpDelete,
        bpChangeCondition,
        bpEnable,
        bpDisable,
        bpChangeAttri,
    }

    public enum CommandAction
    {
        cmdSave,
        cmdStartUndo,
        cmdStartRedo,
        cmdEndUndo,
        cmdEndRedo,
        cmdCopyText,
        cmdPasteText,
        cmdCutText,
        cmdCopyFile,
        cmdPasteFile,
        cmdCutFile,
    }
}
