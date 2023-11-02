using Catawba.Commands;
using Catawba.Commands.ClientCommands;
using Catawba.Database;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;

namespace Catawba.ViewModels
{
    public class ViewInfoViewModel : DataViewModelBase
    {
        public ICommand navigateSearch { get; set; }
        public SearchClientsCommand searchAllCommand { get; set; }
        public RefreshClientsCommand refreshClientsCommand { get; set; }
        public MySqlCommand refreshQueryCommand;
        public ViewModelHandler handler { get; }

        /// <summary>
        /// Initialize view model with a reference to the view handler and set navigation commands.
        /// </summary>
        /// <param name="viewHandler"></param>
        public ViewInfoViewModel(ViewModelHandler viewHandler) : base(viewHandler)
        {
            this.viewHandler = viewHandler;
            refreshClientsCommand = new RefreshClientsCommand(this);
            searchAllCommand = new SearchClientsCommand(new SearchClientsViewModel(viewHandler, this), true);
            navigateSearch = new NavigateCommand(viewHandler, new SearchClientsViewModel(viewHandler, this));
        }

        /// <summary>
        /// Sets the client list by executing the specified MySQL command.
        /// </summary>
        /// <param name="searchCommand">The MySQL command to execute.</param>
        public void setClientList(MySqlCommand searchCommand)
        {
            clientList.Clear();
            displayWaitingPopup("Fetching clients...");
            AllowUIToUpdate();
            clientList = DatabaseStore.getClients(searchCommand);
            closePopup(false);
        }

        /// <summary>
        /// Replays the last sql select command used to populate the client list
        /// </summary>
        public void refreshClients()
        {
            if (refreshQueryCommand != null)
                setClientList(refreshQueryCommand);
        }
    }
}
