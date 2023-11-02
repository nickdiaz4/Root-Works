using Catawba;
using Catawba.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;

namespace Catawba.Commands
{
    public class SubmitClients : CommandBase
    {
        private readonly AddInfoViewModel addInfoViewModel ;

        /// <summary>
        /// Command binded to the "Submit New Clients" button in the Add Client Info page.
        /// Adds all pending clients in the data grid view to the database.
        /// </summary>
        /// <param name="addInfoViewModel"></param>
        public SubmitClients(AddInfoViewModel addInfoViewModel)
        {
            this.addInfoViewModel = addInfoViewModel;
        }
        public override void Execute(object parameter)
        {
            // call function in addInfoViewModel for submitting new clients
            addInfoViewModel.submitNewClients();
        }
    }
}
