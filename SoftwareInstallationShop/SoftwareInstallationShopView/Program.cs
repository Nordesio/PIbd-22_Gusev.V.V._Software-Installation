using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Unity;
using Unity.Lifetime;
using SoftwareInstallationShopBusinessLogic.BusinessLogics;
using SoftwareInstallationShopContracts.BusinessLogicsContracts;
using SoftwareInstallationShopContracts.StoragesContracts;
using SoftwareInstallationShopFileImplement.Implements;

namespace SoftwareInstallationShopView
{
    static class Program
    {
        private static IUnityContainer container = null;

        public static IUnityContainer Container { get { if (container == null) container = BuildUnityContainer(); return container; } }
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(Container.Resolve<FormMain>());
            SoftwareInstallationShopFileImplement.FileDataListSingleton.SaveData();
        }
        private static IUnityContainer BuildUnityContainer()
        {
            var currentContainer = new UnityContainer();

            currentContainer.RegisterType<IComponentStorage, ComponentStorage>(new HierarchicalLifetimeManager());
            currentContainer.RegisterType<IOrderStorage, OrderStorage>(new HierarchicalLifetimeManager());
            currentContainer.RegisterType<IPackageStorage, PackageStorage>(new HierarchicalLifetimeManager());
            currentContainer.RegisterType<IComponentLogic, ComponentLogic>(new HierarchicalLifetimeManager());
            currentContainer.RegisterType<IOrderLogic, OrderLogic>(new HierarchicalLifetimeManager());
            currentContainer.RegisterType<IPackageLogic, PackageLogic>(new HierarchicalLifetimeManager());

            return currentContainer;
        }
    }
}
