using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanjingUniversity.CppMonitor.Monitor.KeyMonitor
{
    class KeyBindEvent : IBindEvent
    {
        DTE2 dte2;
        WindowEvents windowEvents;

        public void RegisterEvent()
        {
            dte2 = ServiceProvider.GlobalProvider.GetService(typeof(SDTE)) as DTE2;

            windowEvents = dte2.Events.WindowEvents;

            //windowEvents.WindowCreated += windowCreatedEventHandler;

        }

        public void windowCreatedEventHandler(Window Window){

            if (Window.Document == null)
            {
                return;
            }
            
            Debug.WriteLine("window create : "+Window.Document.FullName);
            
            IVsTextView vsTextView;

            IVsTextManager txtMgr = (IVsTextManager)ServiceProvider.GlobalProvider.GetService(typeof(SVsTextManager));
            int result = txtMgr.GetActiveView(1, null, out vsTextView);
            
            if (result == VSConstants.S_OK)
            {
                IVsEditorAdaptersFactoryService vsEditorAdaptersFactoryService = GetEditorAdaptersFactoryService();
                IWpfTextView wpfTextView = vsEditorAdaptersFactoryService.GetWpfTextView(vsTextView);
                
                IWpfTextViewHost wpfTextViewHost = vsEditorAdaptersFactoryService.GetWpfTextViewHost(vsTextView);
                
                KeyMonitorIKeyProcessorProvider keyMonitorIKeyProcessorProvider = new KeyMonitorIKeyProcessorProvider();
                keyMonitorIKeyProcessorProvider.GetAssociatedProcessor(wpfTextView);
            }
           
        }

        private Microsoft.VisualStudio.Editor.IVsEditorAdaptersFactoryService GetEditorAdaptersFactoryService()
        {
            Microsoft.VisualStudio.ComponentModelHost.IComponentModel componentModel =
             (Microsoft.VisualStudio.ComponentModelHost.IComponentModel)ServiceProvider.GlobalProvider.GetService(
              typeof(Microsoft.VisualStudio.ComponentModelHost.SComponentModel));
            return componentModel.GetService<Microsoft.VisualStudio.Editor.IVsEditorAdaptersFactoryService>();
        }
    }
}
