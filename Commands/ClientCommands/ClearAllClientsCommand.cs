using Catawba.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catawba.Commands
{
    public class ClearAllClientsCommand : CommandBase
    {
        DataViewModelBase addInfoViewModel;

        /// <summary>
        /// Command binded to the "Clear Pending" button on the Add Client Info page
        /// and to the "Clear" button on the View Client Info page. Used to remove all clients in
        /// the list displayed by the data grid view. 
        /// </summary>
        /// <param name="addInfoViewModel"></param>
        public ClearAllClientsCommand(DataViewModelBase addInfoViewModel)
        {
            this.addInfoViewModel = addInfoViewModel;
        }
        public override void Execute(object parameter)
        {
            addInfoViewModel.clearAllClients();
        }
    }
}
