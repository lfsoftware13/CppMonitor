using CppMonitor.Model;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;
using NanjingUniversity.CppMonitor.Util.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanjingUniversity.CppMonitor.MEFMonitor.KeyMonitor
{
    [Export(typeof(IVsTextViewCreationListener))]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    [Order(Before = "default")]
    [ContentType("code")]
    class CppMonitorTextViewLisenter : IVsTextViewCreationListener
    {
        [Import]
        internal IVsEditorAdaptersFactoryService AdapterService = null;

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            if (PersistentObjectManager.isLogOn)
            {
                ITextView textView = AdapterService.GetWpfTextView(textViewAdapter);
                if (textView == null)
                    return;

                textView.Properties.GetOrCreateSingletonProperty(
                    () => new TypeCharFilter(textViewAdapter, textView));
            }
            
        }

    }

    //主要用于捕获回车和换行事件
    internal sealed class TypeCharFilter : IOleCommandTarget
    {
        IOleCommandTarget nextCommandHandler;
        ITextView textView;

        Document relatedDocument = null;
        Dictionary<uint, string> targetCmdIdToStrDict;

        /// <summary>
        /// Add this filter to the chain of Command Filters
        /// </summary>
        internal TypeCharFilter(IVsTextView adapter, ITextView textView)
        {
            this.textView = textView;
            relatedDocument = Util.Util.utilInstance.getDocumentInfoFromTextView(textView);

            targetCmdIdToStrDict = new Dictionary<uint, string>(2);
            targetCmdIdToStrDict.Add((uint)VSConstants.VSStd2KCmdID.BACKSPACE, "backspace");
            targetCmdIdToStrDict.Add((uint)VSConstants.VSStd2KCmdID.RETURN, "enter");

            adapter.AddCommandFilter(this, out nextCommandHandler);
        }

        /// <summary>
        /// Get user input and update Typing Speed meter. Also provides public access to
        /// IOleCommandTarget.Exec() function
        /// </summary>
        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            if (pguidCmdGroup == VSConstants.VSStd2K && targetCmdIdToStrDict.ContainsKey(nCmdID))
            {
                string projectName;
                string filePath;

                if (relatedDocument == null)
                {
                    relatedDocument = Util.Util.utilInstance.getDocumentInfoFromTextView(textView);
                }
                Util.Util.utilInstance.getProjectInfo(textView,relatedDocument,out projectName, out filePath);

                string charInfo = targetCmdIdToStrDict[nCmdID];

                Debug.WriteLine("inchar : "+ charInfo + " : project : " + projectName + " filePath : " + filePath);

                Util.KeyEventLogUtil.logKeyEvent("keyView",filePath,projectName,charInfo,new KeyModifier());
            }

            

            int hr = VSConstants.S_OK;
            hr = nextCommandHandler.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
            return hr;
        }


        int IOleCommandTarget.QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            return nextCommandHandler.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }
    }
}
