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
    [TextViewRole(PredefinedTextViewRoles.Document)]
    [ContentType("code")]
    [Name("KeyMonitorIKeyProcessorProvider")]
    [Order(Before = "DefaultKeyProcessor")]
    internal class KeyMonitorIKeyProcessorProvider : IKeyProcessorProvider
    {
        [ImportingConstructor]
        public KeyMonitorIKeyProcessorProvider()
        {

        }
        public KeyProcessor GetAssociatedProcessor(IWpfTextView wpfTextView)
        {
            if (wpfTextView == null)
                return null;

            return new KeyMonitorKeyProcessor();
        }
    }

    internal sealed class KeyMonitorKeyProcessor : KeyProcessor
    {
        public override void KeyDown(KeyEventArgs args)
        {
            base.KeyDown(args);
            Debug.WriteLine("KeyDown received: {0}", args.Key);
        }
    }
}
