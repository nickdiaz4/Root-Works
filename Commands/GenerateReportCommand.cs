using Catawba.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using Catawba.Views;


namespace Catawba.Commands
{
    public class GenerateReportCommand : CommandBase
    {
        private DataViewModelBase dataViewModelBase;
        private ObservableCollection<Client> clientList;

        /// <summary>
        /// Command binded to the "Generate Report" button in the Client Info page.
        /// When invoked, it calls the generateReport() function in DatabaseStore to create
        /// a concise report of all clients currently existing in the data grid view. 
        /// </summary>
        /// <param name="clientList"> The list of clients that a user searched for and is displayed in the data grid.</param>
        /// <param name="dataViewModelBase"></param>
        public GenerateReportCommand(ObservableCollection<Client> clientList ,DataViewModelBase dataViewModelBase)
        {
            this.dataViewModelBase = dataViewModelBase;
            this.clientList = clientList;
        }

        public override void Execute(object parameter)
        {
            dataViewModelBase.generateReport();
        }
    }
}
