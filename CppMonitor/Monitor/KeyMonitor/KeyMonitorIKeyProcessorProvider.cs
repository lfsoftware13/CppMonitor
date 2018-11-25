using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text.Editor;
using System.ComponentModel.Composition;
using System.Windows.Input;
using System.Diagnostics;
using Microsoft.VisualStudio.Utilities;

namespace NanjingUniversity.CppMonitor.Monitor.KeyMonitor
{
    [Export(typeof(IKeyProcessorProvider))]
    [TextViewRole(PredefinedTextViewRoles.PrimaryDocument)]
    [ContentType("any")]
    [Name("KeyMonitorIKeyProcessorProvider")]
    [Order(Before = "default")]
    class KeyMonitorIKeyProcessorProvider : IKeyProcessorProvider
    {
        [ImportingConstructor]
        public KeyMonitorIKeyProcessorProvider()
        {

        }
        KeyProcessor IKeyProcessorProvider.GetAssociatedProcessor(IWpfTextView wpfTextView)
        {
            if (wpfTextView == null)
                return null;

            return new KeyMonitorKeyProcessor();
        }
    }

    class KeyMonitorKeyProcessor : KeyProcessor
    {
        public override void PreviewKeyDown(KeyEventArgs args)
        {
            Debug.WriteLine("key down"+args.Key);
        }
    }
}
