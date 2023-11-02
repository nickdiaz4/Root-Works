using Catawba.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catawba.Commands
{
    public class ManageClientCommand : CommandBase
    {
        private DataViewModelBase senderModel;
        private Client client;
        private AddInfoFormViewModel clientForm;
        private bool isNewClient;

        /// <summary>
        /// Command binded to the "Save" button on the Add Client Form. 
        /// </summary>
        /// <param name="isNewClient"> True if the client was recently added on the Add Client Info page. False if client was
        ///                             modified from the View Info Page. </param>
        /// <param name="client"> The selected Client </param>
        /// <param name="senderModel"></param>
        /// <param name="clientForm"></param>
        public ManageClientCommand(bool isNewClient, Client client ,DataViewModelBase senderModel, AddInfoFormViewModel clientForm)
        {
            this.isNewClient = isNewClient;
            this.senderModel = senderModel;
            this.clientForm = clientForm;
            this.client = client;
        }
        public override void Execute(object parameter)
        {
            if (client.validateClient())
            {
                if (isNewClient)
                {
                    AddInfoViewModel addModel = senderModel as AddInfoViewModel;
                    addModel.addClient(client);
                }
                else if (senderModel is ViewInfoViewModel)
                {
                    senderModel.updateClient(client);
                }
                clientForm.goBack.Execute(this);
            }
            else
            {
                clientForm.displayConfirmationPopup(client.errorMessage);
            }
            
        }
    }
}
