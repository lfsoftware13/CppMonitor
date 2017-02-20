using System;
using EnvDTE;
using EnvDTE80;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualStudio.VCProjectEngine;
using NanjingUniversity.CppMonitor.DAO;

namespace NanjingUniversity.CppMonitor.Monitor.CommandMonitor
{
    class CommandUtil
    {

        public CommandUtil()
        {

        }
        public string getDocContent(ProjectItem projectItem)
        {
            Document temp = projectItem.Document;
            if (temp != null)
            {
                TextDocument txt = (TextDocument)temp.Object("TextDocument");
                EditPoint DocStart = txt.StartPoint.CreateEditPoint();
                return DocStart.GetText(txt.EndPoint);
            }
            else
            {
                return null;
            }
        }

        private List<string> LoopFilter(IVCCollection vcFilters)
        {
            List<string> allPath = new List<string>();
            for (int i = 1; i <= vcFilters.Count; i++)
            {
                VCFilter f = vcFilters.Item(i) as VCFilter;
                IVCCollection files = f.Files as IVCCollection;
                for (int j = 1; j <= files.Count; j++)
                {   //all files in current filter
                    VCFile file = files.Item(j) as VCFile;
                    string path = file.FullPath;
                    allPath.Add(path);
                }
                IVCCollection NextFilters = f.Filters as IVCCollection;
                if (NextFilters != null)
                {
                    List<string> inPath = LoopFilter(NextFilters);
                    allPath.AddRange(inPath);
                }
            }

            return allPath;
        }

        private List<string> GetSelectedItemPaths(DTE2 dte)
        {
            List<string> list = new List<string>();
            Array items = (Array)dte.ToolWindows.SolutionExplorer.SelectedItems;
            string projectDirectory = ""; 
            if(items!=null){
                foreach (UIHierarchyItem selItem in items)
                {
                    if (selItem.Object is EnvDTE.Project)
                    {
                        //System.Windows.Forms.MessageBox.Show("Project node is selected: " + selItem.Name);
                        Project project = selItem.Object as Project;
                        projectDirectory = Path.GetDirectoryName(project.FullName);
                    }
                    else if (selItem.Object is EnvDTE.ProjectItem)
                    {
                        //System.Windows.Forms.MessageBox.Show("Project item node is selected: " + selItem.Name);
                        var item = selItem.Object as ProjectItem;

                        if (item.Kind.Equals(EnvDTE.Constants.vsProjectItemKindVirtualFolder))
                        {       //projectItem is a filter(don't have path)
                            //MessageBox.Show("Virtual Folder");
                            VCFilter f = item.Object as VCFilter;
                            IVCCollection vcc = f.Files as IVCCollection;
                            for (int i = 1; i <= vcc.Count; i++)
                            {
                                VCFile vcf = vcc.Item(i) as VCFile;
                                String path = vcf.FullPath;
                                list.Add(path);
                            }
                            IVCCollection NextFilters = f.Filters as IVCCollection;
                            if (NextFilters != null)
                            {
                                List<string> inPath = LoopFilter(NextFilters);
                                list.AddRange(inPath);
                            }

                        }
                        else if (item.Kind.Equals(EnvDTE.Constants.vsProjectItemKindPhysicalFile))
                        {
                            string path = item.Properties.Item("FullPath").Value.ToString();
                            list.Add(path);
                        }
                        else if (item.Kind.Equals(EnvDTE.Constants.vsProjectItemKindPhysicalFolder))
                        {

                        }
                                                  
                    }
                    else if (selItem.Object is EnvDTE.Solution)
                    {
                        //System.Windows.Forms.MessageBox.Show("Solution node is selected: " + selItem.Name);
                    }
                    else
                    {   //Not a ProjectItem ;maybe is a physicalFolder (but in C++ project,physicalFolder not in IDE)
                        IDataObject id = Clipboard.GetDataObject();
                        object ob = id.GetData("CF_VSREFPROJECTITEMS");
                        //MemoryStream memory = (MemoryStream)ob;
                        //byte[] buffer = memory.ToArray();
                        //ASCIIEncoding encoding = new ASCIIEncoding();
                        //string constructedString = encoding.GetString(buffer);
                        //Testlist.Add(new KeyValuePair<String, object>("Action_Detail", constructedString));
                        //Logger.LogInfo(Testlist);
                    }
                }
            }
            
            return list;
        }

        public List<string> GetSelectedFilePaths(DTE2 dte)
        {
            List<string> IE = GetSelectedItemPaths(dte);
            List<string> list = new List<string>();
            foreach(string path in IE){
                //MessageBox.Show(path);
                if(Directory.Exists(path)){
                    string sourceDirectory = @path;
                    var txtFiles = Directory.EnumerateFiles(sourceDirectory, "*", SearchOption.AllDirectories);
                    foreach (string currentFile in txtFiles)
                    {
                        list.Add(currentFile);
                    }
                }
                else if (File.Exists(path))
                {
                    list.Add(path);
                }
                else
                {
                    MessageBox.Show("Wrong file/directory path");
                }
            }
            return list;
        }


        private Project GetSelectedProject(DTE2 dte)
        {

            Project project = null;

            //从被选中对象中获取工程对象
            List<string> list = new List<string>();
            Object[] items = dte.ToolWindows.SolutionExplorer.SelectedItems as object[];
            EnvDTE.UIHierarchyItem item = items[0] as EnvDTE.UIHierarchyItem;

            if (item.Object is EnvDTE.Project)
            {
                //被选中的就是项目本生
                //System.Windows.Forms.MessageBox.Show("Project node is selected: " + item.Name);
                project = item.Object as Project;
            }
            else if (item.Object is EnvDTE.ProjectItem)
            {
                //被选中的是项目下的子项
                //System.Windows.Forms.MessageBox.Show("Project item node is selected: " + item.Name);
                ProjectItem pro = item.Object as ProjectItem;
                project = pro.ContainingProject;
            }

            return project;

        }

        public string GetSelectedProjectPath(DTE2 dte)
        {
            string path = "";
            //获取被选中的工程
            Project project = GetSelectedProject(dte);

            if (project != null)
            {
                //全名包括*.csproj这样的文件命
                path = project.FullName;
            }

            //去掉工程的文件名
            path = Path.GetDirectoryName(path);

            return path;

        }
    }
}
