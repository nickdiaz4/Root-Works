using Catawba.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;

namespace Catawba.Commands
{
    public class DeleteClientCommand : CommandBase
    {
        //public ViewModelBase deleteClientPage;
        private Client client;
        private ObservableCollection<Client> clientList;
        private DataViewModelBase viewModel;

        /// <summary>
        /// Command binded to the "Delete" buttons on both the View Client Info and 
        /// Add Client Info pages. The Add Client info delete removes the selected client from the 
        /// list of pending clients to be added (not in the database). The View Client Info delete
        /// permanently deletes the selected client from the database.
        /// </summary>
        /// <param name="clientList"></param>
        /// <param name="viewModel"></param>
        public DeleteClientCommand(ObservableCollection<Client> clientList,DataViewModelBase viewModel)
        {
            this.clientList = clientList;
            this.viewModel = viewModel;
        }

        /// <summary>
        /// Set client for removal.
        /// This is called whenever the selected client is changed.
        /// </summary>
        /// <param name="client"></param>
        public void setClient(Client client)
        {
            this.client = client;
        }

        public override void Execute(object parameter)
        {
            if (viewModel is ViewInfoViewModel)
            {
                // Grant put code here
                viewModel.deleteClient(client);
                
            }
            clientList.Remove(client);
        }

    }
}
