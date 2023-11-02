using Catawba.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catawba.Commands
{
    public class RemoveServiceCommand : CommandBase
    {
        AddInfoFormViewModel ClientForm;
        SearchClientsViewModel SearchForm;
        string ServiceName;

        /// <summary>
        /// Command to remove a service from a client's services list. Used to get rid of any 
        /// services that are no longer applicable to an individual client.
        /// </summary>
        /// <param name="clientForm"></param>
        /// <param name="serviceName"></param>
        public RemoveServiceCommand(AddInfoFormViewModel clientForm, string serviceName)
        {
            this.ClientForm = clientForm;
            this.ServiceName = serviceName;
        }
        // Overloading RemoveServiceCommand for the search clients form and actions
        public RemoveServiceCommand(SearchClientsViewModel searchForm, string serviceName)
        {
            this.SearchForm = searchForm;
            this.ServiceName = serviceName;
        }
        public override void Execute(object parameter)
        {
            // ClientForm null when using the search client form
            // Note: I tried catching a NullRefernceException instead
            //       as well as a general Exception, but the error
            //       would not be caught
            if (ClientForm != null)
            {
                ClientForm.removeServiceItem(ServiceName);
            }
            else
            {
                //SearchForm.removeServiceItem(ServiceName);
            }
        }
    }
}
