using Catawba.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;


namespace Catawba.Commands
{
    public class DeleteServiceCommand : CommandBase
    {
        private MngServicesViewModel serviceDataViewModel;

        /// <summary>
        /// Command binded to the "Delete" button in the Manage Services administrator page.
        /// Used to permanently delete a service from the database.
        /// </summary>
        /// <param name="serviceDataViewModel"></param>
        public DeleteServiceCommand(MngServicesViewModel serviceDataViewModel)
        {
            this.serviceDataViewModel = serviceDataViewModel;
        }

        public override void Execute(object parameter)
        {
            serviceDataViewModel.deleteService();
        }
    }
}
