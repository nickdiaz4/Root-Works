using Catawba.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;

namespace Catawba.Commands
{
    public class AddNewServiceCommand : CommandBase
    {
        private MngServicesViewModel mngServicesViewModel;

        /// <summary>
        /// Command binded to the "Add" button in the Manage Services administrator page.
        /// Used to add a new service to the database.
        /// </summary>
        /// <param name="mngServicesViewModel"></param>
       public AddNewServiceCommand(MngServicesViewModel mngServicesViewModel)
        {
            this.mngServicesViewModel = mngServicesViewModel;
        }

        public override void Execute(object parameter)
        {
            mngServicesViewModel.addNewService();
        }

    }
}
