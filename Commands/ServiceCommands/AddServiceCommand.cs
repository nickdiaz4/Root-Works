using Catawba.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catawba.Commands
{
    public class AddServiceCommand : CommandBase
    {
        AddInfoFormViewModel ClientForm;
        SearchClientsViewModel SearchForm;
        string ServiceName;

        /// <summary>
        /// Command to add a relevant service to a client that is being added or modified.
        /// Adds the service the that individual client's services list.
        /// </summary>
        /// <param name="clientForm"></param>
        /// <param name="serviceName"></param>
        public AddServiceCommand(AddInfoFormViewModel clientForm, string serviceName)
        {
            this.ClientForm = clientForm;
            this.ServiceName = serviceName;
        }
        
        // Overloading AddServiceCommand for search client form and actions
        public AddServiceCommand(SearchClientsViewModel searchForm, string serviceName)
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
                ClientForm.addServiceItem(ServiceName);
            }
            else
            {
               // SearchForm.addServiceItem(ServiceName);
            }
        }
    }
}
