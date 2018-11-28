using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using MonitorInterop.ServiceInterop;
using NanjingUniversity.CppMonitor.ServiceInterop;
using NanjingUniversity.CppMonitor.DTEMonitor;
using NanjingUniversity.CppMonitor.Util.Common;

namespace NanjingUniversity.CppMonitor
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the information needed to show this package
    // in the Help/About dialog of Visual Studio.
    [ProvideAutoLoad(UIContextGuids.NoSolution)]
    [ProvideAutoLoad(UIContextGuids.SolutionExists)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideService(typeof(SMonitorService))]
    [Guid(GuidList.guidCppMonitorPkgString)]
    public sealed class CppMonitorPackage : Package
    {
        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public CppMonitorPackage()
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));

            IServiceContainer serviceContainer = this as IServiceContainer;
            ServiceCreatorCallback callback = new ServiceCreatorCallback(CreateService);
            serviceContainer.AddService(typeof(SMonitorService), callback, true);
        }

        private object CreateService(IServiceContainer container, Type serviceType)
        {
            if (typeof(SMonitorService) == serviceType)
            {
                return new MonitorService(this);
            }

            else
            {
                return null;
            }
        }



        /////////////////////////////////////////////////////////////////////////////
        // Overridden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            //询问使用Visual Studio的目的，如果是用于规定的练习，则记录日志
            //用于自己的练习则不记录日志
            bool isWriteLog = true;
            System.Windows.Forms.DialogResult res = System.Windows.Forms.MessageBox.Show(
                "做课堂练习或者进行考试？",
                "提示",
                System.Windows.Forms.MessageBoxButtons.YesNo,
                System.Windows.Forms.MessageBoxIcon.Information);

            if (res == System.Windows.Forms.DialogResult.No){
                //确定已经提交
                isWriteLog = false;
            }

            PersistentObjectManager.isLogOn = isWriteLog;//会决定MEF组件是否安装

            if (!isWriteLog){
                //如果是自己练习，则不需要注册插件直接就返回了
                return;
            }

            //否则注册插件进行日志的记录
            //注册DTE组件
            DTE_Initializer dTE_Initializer = new DTE_Initializer();
            dTE_Initializer.initializeMonitor();

            //MEF部分是自动装载的，所以不需要手动注册，这里 通过全局变量来控制是否记录日志
        }
       
        #endregion

    }
}
