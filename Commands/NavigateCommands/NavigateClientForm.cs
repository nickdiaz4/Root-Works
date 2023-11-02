using Catawba.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Catawba.Commands
{
    public class NavigateClientForm : CommandBase
    {
        private ViewModelHandler handler;
        private DataViewModelBase origin;
        private Client client;
        private bool isNewClient;

        /// <summary>
        /// Command binded to the "Add New","Modify" buttons in the Add Client Info
        /// page, and to the "Update" button in the View Client Info page. Changes the view to
        /// the "Modify Client" form.
        /// 
        /// "Add New" button will display a completely empty form to be filled out.
        /// "Modify" and "Update" will be pre-filled out with any information that was entered.
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="origin"></param>
        /// <param name="client"></param>
        /// <param name="isNewClient"></param>
        public NavigateClientForm(ViewModelHandler handler, DataViewModelBase origin, Client client,bool isNewClient) 
        {
            this.handler = handler;
            this.origin = origin;
            this.client = client;
            this.isNewClient = isNewClient;
        }
        public void setClient(Client client)
        {
            this.client = client;
        }

        public override void Execute(object parameter)
        {
            // If client does not equal NULL transition to the client form with client information pre-filled
            if (client != null)
            {
                handler.currentViewModel = new AddInfoFormViewModel(handler, origin, client, isNewClient);
            }
        }
    }
}
