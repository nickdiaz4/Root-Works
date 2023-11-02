using Catawba.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Windows;
using System.Diagnostics;
using Catawba.Database;
using System.Globalization;

namespace Catawba.ViewModels
{
    public class MngServicesViewModel : ViewModelBase
    {
        public ICommand addNewServiceCommand { get; set; }
        public Services newService { get; set; }
        public ViewModelHandler viewHandler { get; set; }
        public DeleteServiceCommand deleteServiceCommand { get; set; }
        public DisplayPromptCommand displayDeletePrompt { get; set; }
        private Services _selectedService;
        public Services selectedService
        {
            get => _selectedService;
            set
            {
                _selectedService = value;
            }
        }

        // list that holds all services in the DB
        private ObservableCollection<Services> _serviceList = new ObservableCollection<Services>();
        public ObservableCollection<Services> serviceList
        {
            get => _serviceList;
            set
            {
                _serviceList = value;
                OnPropertyChanged(nameof(_serviceList));
            }
        }

        public MngServicesViewModel(ViewModelHandler viewHandler)
        {
            this.viewHandler = viewHandler;
            serviceList = DatabaseStore.getServices("SELECT serviceName FROM services");
            deleteServiceCommand = new DeleteServiceCommand(this);
            displayDeletePrompt = new DisplayPromptCommand(this, "Are you sure you want to delete this service?", deleteServiceCommand);
            newService = new Services();
            addNewServiceCommand = new AddNewServiceCommand(this);
        }

        /// <summary>
        /// Adds a new service to the database. First verifies that the entered
        /// service name doe not already exist in the database. If it does exist, 
        /// the query is denied and the user is notified. If false, the service
        /// gets added and the new service is displayed on the services data grid on the page.
        /// </summary>
        public void addNewService()
        {
            try
            {
                if (!DatabaseStore.isInDatabase(newService))
                {
                    newService.serviceName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(newService.serviceName.Trim());
                    serviceList.Add(newService);
                    DatabaseStore.addService(newService);
                    newService = new Services();
                    OnPropertyChanged(nameof(newService));
                }
                else
                {
                    displayConfirmationPopup("Service already exists");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Permanently deletes a service from the database. It first
        /// verifies that the service name exists in the table, and if it does
        /// exist, it gets deleted. If not, a popup notifying the user is displayed. 
        /// </summary>
        public void deleteService()
        {
            if (DatabaseStore.isInDatabase(selectedService))
            {
                DatabaseStore.deleteService(selectedService);
                serviceList.Remove(selectedService);
            }
            else
            {
                displayConfirmationPopup("Service does not exist");
            }

        }

    }
}
