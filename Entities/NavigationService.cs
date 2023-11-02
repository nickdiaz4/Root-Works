using Catawba.Commands;
using Catawba.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catawba
{
    public class NavigationService
    {

        public NavigateCommand navigateHome { get; set; }
        public NavigateCommand navigateAddInfo { get; set;}
        public NavigateCommand navigateViewInfo { get; set;}
        public NavigateCommand navigateLogin { get; set;}
        public NavigateCommand navigateAdminHome { get; set;}
        public NavigateCommand navigateMngAccs { get; set;}
        public NavigateCommand navigateMngServices { get; set;}
        ViewModelHandler viewHandler;
        public NavigationService(ViewModelHandler viewHandler)
        {
            //navigateHome = new NavigateCommand(viewHandler, viewHandler.homePage);
            //navigateAddInfo = new NavigateCommand(viewHandler, viewHandler.addInfoPage);
            //navigateViewInfo = new NavigateCommand(viewHandler, viewHandler.viewInfoPage);
            //navigateLogin = new NavigateCommand(viewHandler, viewHandler.loginPage);
            //navigateMngAccs = new NavigateCommand(viewHandler, viewHandler.mngAccsPage);
            //navigateMngServices = new NavigateCommand(viewHandler, viewHandler.mngServicesPage);
            this.viewHandler = viewHandler;
        }

        public void setNavigationCommands()
        {
            navigateHome = new NavigateCommand(viewHandler, viewHandler.homePage);
            navigateAddInfo = new NavigateCommand(viewHandler, viewHandler.addInfoPage);
            navigateViewInfo = new NavigateCommand(viewHandler, viewHandler.viewInfoPage);
            navigateLogin = new NavigateCommand(viewHandler, viewHandler.loginPage);
            
            if (viewHandler.adminVisibility == System.Windows.Visibility.Visible)
            {
                navigateMngAccs = new NavigateCommand(viewHandler, viewHandler.mngAccsPage);
                navigateMngServices = new NavigateCommand(viewHandler, viewHandler.mngServicesPage);
            }
        }

        public void removeNavigationCommands()
        {
            navigateHome = null;
            navigateAddInfo = null;
            navigateViewInfo = null;
            navigateLogin = null;
            navigateMngAccs = null;
            navigateMngServices = null;
        }
    }
}
