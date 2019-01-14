using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System.Windows.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;
using CppMonitor.Model;
using NanjingUniversity.CppMonitor.Util.Common;

namespace NanjingUniversity.CppMonitor.MEFMonitor.KeyMonitor
{
    [Export(typeof(IKeyProcessorProvider))]
    [Name("CppMonitorKeyProcessor")]
    [Order(Before = "DefaultKeyProcessor")]
    [ContentType("code")]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    internal class CppMonitorKeyProcessor : IKeyProcessorProvider
    {
        public KeyProcessor GetAssociatedProcessor(IWpfTextView wpfTextView)
        {
            if (PersistentObjectManager.isLogOn)
            {
                return new KeyMonitorKeyProcessor((ITextView)wpfTextView);
            }
            return null;
        }
    }

    internal sealed class KeyMonitorKeyProcessor:KeyProcessor
    {
        ITextView textView;
        Document relatedDocument;

        public KeyMonitorKeyProcessor(ITextView textView)
        {
            this.textView = textView;
            relatedDocument = Util.Util.utilInstance.getDocumentInfoFromTextView(textView);
        }

        public override void TextInput(TextCompositionEventArgs args)
        {
            base.TextInput(args);
            if (!String.IsNullOrEmpty(args.Text))
            {
                string filePath;
                string projectName;
                Util.Util.utilInstance.getProjectInfo(textView, relatedDocument, out projectName, out filePath);

                KeyModifier keyModifier = checkKeyModifier();

                Util.KeyEventLogUtil.logKeyEvent(KeyAction.keyInput.ToString(), filePath, projectName, args.Text, keyModifier);
            }
        }

        public override void KeyDown(KeyEventArgs args)
        {
            base.KeyDown(args);

            string filePath;
            string projectName;

            if (relatedDocument == null)
            {
                relatedDocument = Util.Util.utilInstance.getDocumentInfoFromTextView(textView);
            }
            Util.Util.utilInstance.getProjectInfo(textView,relatedDocument,out projectName,out filePath);

            KeyModifier keyModifier = checkKeyModifier();

            
            Util.KeyEventLogUtil.logKeyEvent(KeyAction.keyDown.ToString(), filePath, projectName, translateKey(args), keyModifier);
        }

        public override void KeyUp(KeyEventArgs args)
        {
            base.KeyUp(args);

            string filePath;
            string projectName;
            Util.Util.utilInstance.getProjectInfo(textView, relatedDocument, out projectName, out filePath);

            KeyModifier keyModifier = checkKeyModifier();

            Util.KeyEventLogUtil.logKeyEvent(KeyAction.keyUp.ToString(), filePath, projectName, translateKey(args), keyModifier);
        }

        private KeyModifier checkKeyModifier()
        {
            KeyModifier keyModifier = new KeyModifier();

            keyModifier.containsAlt = (Keyboard.Modifiers & ModifierKeys.Alt) != 0;
            keyModifier.containsControl = (Keyboard.Modifiers & ModifierKeys.Control) != 0;
            keyModifier.containsShift = (Keyboard.Modifiers & ModifierKeys.Shift) != 0;
            keyModifier.containsWindows = (Keyboard.Modifiers & ModifierKeys.Windows) != 0;

            bool isCapsLockOn = System.Windows.Forms.Control
                        .IsKeyLocked(System.Windows.Forms.Keys.CapsLock);
            keyModifier.containsCaps = isCapsLockOn;

            return keyModifier;
        }

        private string translateKey(KeyEventArgs args)
        {
            string key = ConstantCommon.UNKNOWN_KEY;
            switch (args.Key)
            {
                case Key.System:
                    key = args.SystemKey.ToString();
                    break;
                case Key.ImeProcessed:
                    key = args.ImeProcessedKey.ToString();
                    break;
                default:
                    key = args.Key.ToString();
                    break;
            }
            return key;
        }
    }
}
