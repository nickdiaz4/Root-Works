 using Catawba.Commands;
using Catawba.Database;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;

namespace Catawba.ViewModels
{
    public class AddInfoFormViewModel : ViewModelBase
    {
        // Command for returning to the previous page
        public NavigateCommand goBack { get; set; }
        public ManageClientCommand manageClientCommand { get; set; }
        public Client editingClient { get; set; }
        public Client originalClient { get; set; }
        public ViewModelHandler viewHandler { get; set; } 
        public DataViewModelBase returnLocation { get; set; }
        public bool isNewClient;
        public bool isReadOnly { get; set; }



        // Observable collection for displaying available services the client does not have
        private ObservableCollection<serviceItem> _AvailableServiceList = new ObservableCollection<serviceItem>();
        public ObservableCollection<serviceItem> AvailableServiceList
        {
            get => _AvailableServiceList;
            set
            {
                _AvailableServiceList = value;
                OnPropertyChanged(nameof(_AvailableServiceList));
            }
        }

        // Observable collection for displaying active services the client currently has
        private ObservableCollection<serviceItem> _ActiveServices = new ObservableCollection<serviceItem>();
        public ObservableCollection<serviceItem> ActiveServices
        {
            get => _ActiveServices;
            set
            {
                _ActiveServices = value;
                OnPropertyChanged(nameof(_ActiveServices));
            }
        }

        /// <summary>
        /// Pairs a service name with an action command that can either add or remove the service.
        /// </summary>
        public struct serviceItem
        {
            public static int ADD = 1;
            public static int REMOVE = 2;
            public string ServiceName { get;}
            public ICommand ActionCommand { get; set; }

            /// <summary>
            /// Set the new command for what should happen when that service is clicked.
            /// </summary>
            /// <param name="command"></param>
            public void setActionCommand(ICommand command)
            {
                ActionCommand = command;
            }

            /// <summary>
            /// initialize service item with the name of the service and the AddInfoFormViewModel containing the service.
            /// </summary>
            /// <param name="serviceName"></param>
            /// <param name="ClientForm"></param>
            public serviceItem(string serviceName, AddInfoFormViewModel ClientForm, int action)
            {
                this.ServiceName = serviceName;
                if (action == ADD){
                    this.ActionCommand = new AddServiceCommand(ClientForm, serviceName);
                }
                else if (action == REMOVE){
                    this.ActionCommand = new RemoveServiceCommand(ClientForm, serviceName);
                }
                else{
                    this.ActionCommand = null;
                }
            }
        }

        /// <summary>
        /// Initialize with references to the view handler, the view model the form originated from
        /// and the client that will be modified.
        /// </summary>
        /// <param name="viewHandler"></param>
        /// <param name="returnLocation"></param>
        /// <param name="client"></param>
        public AddInfoFormViewModel(ViewModelHandler viewHandler, DataViewModelBase returnLocation, Client client, bool isNewClient)
        {
            this.viewHandler = viewHandler;
            editingClient = client;
            originalClient = client.Copy();
            this.returnLocation = returnLocation;
            this.isNewClient = isNewClient;

            if (!isNewClient && returnLocation is ViewInfoViewModel)
                isReadOnly = true;
            else
            {
                isReadOnly = false;
            }

            // initialize all service items
            foreach (string service in DatabaseStore.getAllServices())
            {
                if (!client.Services.Contains(service))
                {
                    _AvailableServiceList.Add(new serviceItem(service, this, serviceItem.ADD));

                }
                else
                {
                    _ActiveServices.Add(new serviceItem(service, this, serviceItem.REMOVE));
                }

            }
            goBack = new NavigateCommand(viewHandler, returnLocation);
            manageClientCommand = new ManageClientCommand(isNewClient, client,returnLocation, this);
        }

        /// <summary>
        /// Visually Remove a service from the list of active services and add it to the available services.
        /// The service by name is removed from the Client itself.
        /// </summary>
        /// <param name="serviceName"></param>
        public void removeServiceItem(string serviceName)
        {
            editingClient.shouldUpdateServices = true;
            serviceItem service = new serviceItem();
            AddServiceCommand addCommand = new AddServiceCommand(this, serviceName);

            // search through all active services to find the one to remove
            foreach (serviceItem item in _ActiveServices)
            {
                if (item.ServiceName == serviceName)
                {
                    service = item;
                    editingClient.removeService(serviceName);
                    break;
                }
            }

            // remove from active list
            _ActiveServices.Remove(service);
            OnPropertyChanged(nameof(_ActiveServices));

            // set new Action command
            service.setActionCommand(addCommand);

            // add to available service list
            _AvailableServiceList.Add(service);
            OnPropertyChanged(nameof(_AvailableServiceList));
        }

        /// <summary>
        /// Visually remove a service from the list of available services and add it to the active services.
        /// The service by name is added from the Client itself.
        /// </summary>
        /// <param name="serviceName"></param>
        public void addServiceItem(string serviceName)
        {
            editingClient.shouldUpdateServices = true;
            serviceItem service = new serviceItem();
            RemoveServiceCommand removeCommand = new RemoveServiceCommand(this, serviceName);

            // add service to client list of services
            editingClient.addService(serviceName);

            // search through all available services to find the one to remove
            foreach (serviceItem item in _AvailableServiceList)
            {
                if (item.ServiceName == serviceName)
                {
                    service = item;
                    break;
                }
            }

            // remove from available list
            _AvailableServiceList.Remove(service);
            OnPropertyChanged(nameof(_AvailableServiceList));

            // set new Action command
            service.setActionCommand(removeCommand);

            // add to active services
            _ActiveServices.Add(service);
            OnPropertyChanged(nameof(_ActiveServices));
        }
    }
}