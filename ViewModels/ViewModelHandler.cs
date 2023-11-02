using Catawba.Commands;
using Catawba.Database;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;

namespace Catawba.ViewModels
{
    public class ViewModelHandler : ViewModelBase
    {
        public HomeViewModel homePage { get; set; }
        public AddInfoViewModel addInfoPage { get; set; }
        public ViewInfoViewModel viewInfoPage { get; set; }
        public LoginViewModel loginPage { get; set; }
        public MngAccsViewModel mngAccsPage { get; set; }
        public MngServicesViewModel mngServicesPage { get; set; }
        public string loggedAccount { get; set; }
        public ObservableCollection<Client> Clients { get; } = new ObservableCollection<Client>();
        public LogoutCommand logoutCommand { get; set; }

        private object _currentViewModel;
        public object currentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged(nameof(currentViewModel));

                // If moving to a DataViewModel make sure the selectedClient is null so it does not appear on view load
                if (currentViewModel is DataViewModelBase)
                {
                    DataViewModelBase view = (DataViewModelBase)currentViewModel;
                    view.selectedClient = null;
                }
                // ^ but with staff accounts
                if(currentViewModel is MngAccsViewModel)
                {
                    MngAccsViewModel view = (MngAccsViewModel)currentViewModel;
                    view.selectedAccount = null;
                }
                // ^ but with services
                if(currentViewModel is MngServicesViewModel)
                {
                    MngServicesViewModel view = (MngServicesViewModel)currentViewModel;
                    view.selectedService = null;
                }
            }
        }
        
        private Visibility _adminVisibility;
        public Visibility adminVisibility
        {
            get
            {
                return _adminVisibility;
            }
            set
            {
                _adminVisibility = value;

                OnPropertyChanged(nameof(adminVisibility));
            }
        }

        private string _currentUser;

        public string currentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged(nameof(currentUser));
            }
        }

        public ViewModelHandler()
        {
            adminVisibility = Visibility.Collapsed;
            currentUser = "";

            // create Navigation service
            navigator = new NavigationService(this);

            // create new instances of login viewModel since it is the first model the user will see
            // and will be used to validate the database connection
            loginPage = new LoginViewModel(this);

            logoutCommand = new LogoutCommand(this);

            // set default page to login page
            currentViewModel = loginPage;
        }

        /// <summary>
        /// Initialize all viewmodels and set their navigator.
        /// </summary>
        public void initViewModels()
        {
            // create new instances of main viewModels that should not be recreated
            homePage = new HomeViewModel(this);
            viewInfoPage = new ViewInfoViewModel(this);
            addInfoPage = new AddInfoViewModel(this);
            mngAccsPage = new MngAccsViewModel(this);
            mngServicesPage = new MngServicesViewModel(this);

            // set the navigation service for each view model after the service and viewmodels have been created
            homePage.navigator = navigator;
            viewInfoPage.navigator = navigator;
            addInfoPage.navigator = navigator;
            mngAccsPage.navigator = navigator;
            mngServicesPage.navigator = navigator;
        }
        public void logout()
        {
            currentViewModel = loginPage;
            DatabaseStore.activeAccount = null;
            currentUser = "";
            navigator.removeNavigationCommands()
;
        }
        public void login()
        {
            navigator.setNavigationCommands();
            currentViewModel = homePage;
        }
    }
}
