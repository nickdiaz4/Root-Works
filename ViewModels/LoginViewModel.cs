using Catawba.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Catawba.Database;
using System.Diagnostics;
using System.Windows;

namespace Catawba.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        public Staff loginAccount { get; set; }
        public ICommand LoginCommand { get; set; }
        public ICommand ToggleLoginErrVisibility { get; set; }
        public ViewModelHandler viewHandler { get; set; }

        //for error message visibility
        private Visibility _visibility;
        public Visibility visibility
        {
            get
            {
                return _visibility;
            }
            set
            {
                _visibility = value;

                OnPropertyChanged(nameof(visibility));
            }
        }


        public LoginViewModel(ViewModelHandler viewHandler)
        {
            this.viewHandler = viewHandler;
            loginAccount = new Staff();
            LoginCommand = new LoginCommand(this);
            ToggleLoginErrVisibility = new ToggleLoginErrVisibility(this);
            visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Attempts to log an account in with user entered credentials.
        /// Checks if the entered credentials match with an entry
        /// in the database then fetches the administrator status
        /// associated with that account to determine what view
        /// the account will see. 
        /// </summary>
        public void login()
        {
            if (!DatabaseStore.isValidConnection())
            {
                displayConfirmationPopup("Unable to connect to database.");
                return;
            }
            else
            {
                viewHandler.initViewModels();
            }

            closePopup(false);

            try
            {
                if (DatabaseStore.staffLogin(loginAccount.UserName, loginAccount.Password))
                {
                    DatabaseStore.getAdminStatus(loginAccount);
                    if (loginAccount.Admin == true)
                    {
                        viewHandler.adminVisibility = Visibility.Visible;
                    }
                    else
                    {
                        viewHandler.adminVisibility = Visibility.Collapsed;
                    }

                    // set username of successful login in viewhandler to show on different views
                    viewHandler.currentUser = loginAccount.UserName;
                    DatabaseStore.activeAccount = loginAccount.UserName;
                    loginAccount = new Staff();
                    viewHandler.login();
                }
                else
                {
                    ToggleLoginErrVisibility.Execute(this);
                }
            }
            catch (Exception ex)
            {
                displayConfirmationPopup(ex.Message);
            }
        }
    }
}
