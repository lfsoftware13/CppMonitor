using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using NanjingUniversity.CppMonitor.Util.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanjingUniversity.CppMonitor.MEFMonitor.Util
{
    class Util
    {
        private static object objLock = new object();

        private static Util _utilInstance = null;

        private ITextDocumentFactoryService TextDocumentFactoryService = null;

        private Util()
        {
            TextDocumentFactoryService = GetTextDocumentFactoryService();
        }

        public static Util utilInstance
        {
            get
            {
                if (_utilInstance == null)
                {
                    lock (objLock)
                    {
                        if (_utilInstance == null)
                        {
                            _utilInstance = new Util();
                        }
                    }
                }
                return _utilInstance;
            }
        }

        private ITextDocumentFactoryService GetTextDocumentFactoryService()
        {
            IComponentModel componentModel =
             (IComponentModel)ServiceProvider.GlobalProvider.GetService(
              typeof(SComponentModel));
            return componentModel.GetService<ITextDocumentFactoryService>();
        }

        

        public Document getDocumentInfoFromTextView(ITextView textView)
        {

            string textViewFilePath = getFilePathFromTextView(textView);

            if (textViewFilePath == null)
            {
                return null;
            }

            DTE2 dte = PersistentObjectManager.dte2;
            Document targetDocument = null;
            Documents documents = dte.Documents;
            foreach (Document document in documents)
            {
                if (document.FullName.Equals(textViewFilePath)) ;
                {
                    targetDocument = document;
                    break;
                }
            }

            return targetDocument;
        }

        public string getFilePathFromTextView(ITextView textView)
        {
            if (TextDocumentFactoryService == null)
            {
                Debug.WriteLine("MEF Monitor Error : " + this.ToString() + " : no TextDocumentFactoryService");
                return null;
            }

            if (textView == null)
            {
                return null;
            }

            ITextBuffer vsTextBuffer = textView.TextBuffer;
            if (vsTextBuffer == null)
            {
                return null;
            }

            ITextDocument textDocument = null;
            TextDocumentFactoryService.TryGetTextDocument(vsTextBuffer, out textDocument);

            if (textDocument == null)
            {
                return null;
            }

            string filePath = textDocument.FilePath;
            return filePath;
        }

        public void getProjectInfo(ITextView textView, Document relatedDocument, out string projectName, out string filePath)
        {
            projectName = ConstantCommon.UNKNOWN_PROJECTNAME;
            filePath = ConstantCommon.UNKNOWN_FILEPATH;

            if (relatedDocument == null)
            {
                filePath = getFilePathFromTextView(textView);
                if (filePath == null)
                {
                    filePath = ConstantCommon.UNKNOWN_FILEPATH;
                }
                return;
            }

            filePath = relatedDocument.FullName;
            projectName = relatedDocument.ProjectItem.ProjectItems.ContainingProject.Name;
        }


    }

}
