using Catawba.Commands;
using Catawba.Database;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows;

namespace Catawba.ViewModels
{
    /// <summary>
    /// This viewModelBase is for viewModels that contain a datagrid and is used to manage multiple client objects.
    /// Currently it is only used for the AddInfoViewModel and ViewInfoViewModel
    /// </summary>
    public class DataViewModelBase : ViewModelBase
    {
        public ViewModelHandler viewHandler { get; set; }
        public NavigateClientForm navigateUpdateForm { get; set; }
        public DeleteClientCommand deleteClientCommand { get; set; }
        public GenerateReportCommand generateReportCommand { get; set; }
        public ClearAllClientsCommand clearAllClientsCommand { get; set; }

        Staff currUser { get; set; }

        public List<string> availableServices { get; }

        /// <summary>
        /// Property that is bound to the currently selected item of the clientList datagrid.
        /// </summary>
        public Client selectedClient
        {
            get => _selectedClient;
            set
            {
                // update necessary components that use the currently selected client
                _selectedClient = value;
                navigateUpdateForm.setClient(value);
                deleteClientCommand.setClient(value);
                OnPropertyChanged(nameof(navigateUpdateForm));
                OnPropertyChanged(nameof(deleteClientCommand));
            }
        }
        private Client _selectedClient;

        /// <summary>
        /// ObservableCollection used to populate a datagrid with a list of clients.
        /// </summary>
        public ObservableCollection<Client> clientList
        {
            get => _clientList;
            set
            {
                // update necessary components that require a client list
                _clientList = value;
                deleteClientCommand = new DeleteClientCommand(value,this);
                generateReportCommand = new GenerateReportCommand(value, this);
                OnPropertyChanged(nameof(clientList));
            }
        }
        private ObservableCollection<Client> _clientList = new ObservableCollection<Client>();

        public DataViewModelBase(ViewModelHandler viewHandler)
        {
            currUser = viewHandler.loginPage.loginAccount;
            availableServices = DatabaseStore.getAllServices();
            navigateUpdateForm = new NavigateClientForm(viewHandler, this, selectedClient,false);
            deleteClientCommand = new DeleteClientCommand(clientList,this);
            clearAllClientsCommand = new ClearAllClientsCommand(this);
            generateReportCommand = new GenerateReportCommand(clientList, this);
        }

        /// <summary>
        /// Performs a database delete for the given client.
        /// </summary>
        /// <param name="client"></param>
        public void deleteClient(Client client)
        {
            DatabaseStore.deleteClient(client);

        }

        /// <summary>
        /// Performs a database update for the given client.
        /// </summary>
        /// <param name="client"></param>
        public void updateClient(Client client)
        {
            DatabaseStore.updateClient(client);
        }

        /// <summary>
        /// Restore client data in the datagrid to what it was before any modification.
        /// </summary>
        /// <param name="originalClient"></param>
        public void restoreClient(Client originalClient)
        {
            if (clientList.IndexOf(selectedClient) != -1)
                clientList[clientList.IndexOf(selectedClient)] = originalClient;
        }

        /// <summary>
        /// Clears all clients from the client list and removes them from the datagrid
        /// </summary>
        public void clearAllClients()
        {
            clientList.Clear();
        }

        /// <summary>
        /// Creates a report with the list of clients a user searched for.
        /// Calls the generateReport() function in Database store which does all the work. 
        /// </summary>
        public void generateReport()
        {
            try
            {
                displayWaitingPopup("Attempting to generate Report...");
                AllowUIToUpdate();

                if (clientList.Count > 0)
                {
                    DatabaseStore.generateReport(clientList, currUser);
                }
                else
                {
                    displayConfirmationPopup("No clients to produce report with");
                }
            }
            catch (Exception e)
            {
                displayConfirmationPopup($"An error occured while generating report: \"{e.Message}\"");
            }
            displayConfirmationPopup("Report created");
        }
    }
}
