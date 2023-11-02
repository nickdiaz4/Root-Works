using Catawba.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Windows;
using System.Diagnostics;
using Catawba.Database;
using Catawba.Entities;


namespace Catawba.ViewModels
{
    public class MngAccsViewModel : ViewModelBase
    {
        public ViewModelHandler viewHandler { get; set; }
        public ICommand addNewAccCommand { get; set; }
        public ICommand createPasswordCommand { get; set; }
        public ResetPasswordCommand resetPasswordCommand { get; set; }
        public Staff newAccount { get; set; }
        public DeleteStaffAccountCommand deleteStaffAccountCommand { get; set; }
        public DisplayPromptCommand displayDeletePrompt { get; set; }
        // selection of a single staff account on the grid view
        private Staff _selectedAccount;
        public Staff selectedAccount
        {
            get => _selectedAccount;
            set
            {
                _selectedAccount = value;
                deleteStaffAccountCommand.setAccount(value);
                resetPasswordCommand.setAccount(value);
            }
        }

        // list that holds all staff accounts in the DB & visible in the grid
        private ObservableCollection<Staff> _staffAccounts = new ObservableCollection<Staff>();
        public ObservableCollection<Staff> staffAccounts
        {
            get => _staffAccounts;
            set
            {
                _staffAccounts = value;
                deleteStaffAccountCommand = new DeleteStaffAccountCommand(value, this);
                resetPasswordCommand = new ResetPasswordCommand(this);
                OnPropertyChanged(nameof(_staffAccounts));
            }
        }

        public MngAccsViewModel(ViewModelHandler viewHandler)
        {
            this.viewHandler = viewHandler;
            newAccount = new Staff();
            staffAccounts = DatabaseStore.getStaffAccounts("SELECT * FROM users");
            deleteStaffAccountCommand = new DeleteStaffAccountCommand(staffAccounts, this);
            displayDeletePrompt = new DisplayPromptCommand(this,"Are you sure you want to delete this account?",deleteStaffAccountCommand);
            addNewAccCommand = new AddNewStaffCommand(this);
            createPasswordCommand = new CreatePasswordCommand(this);
        }

        /// <summary>
        /// Generates a random 8-char long password using the 
        /// createUniqueKey() function found in the Security class.
        /// </summary>
        public void createPassword()
        {
            newAccount.Password = Security.createUniqueKey();
            OnPropertyChanged(nameof(newAccount));
        }

        /// <summary>
        /// Generates a new, random 8-char long password for the
        /// account selected from the grid view and updates the 
        /// password column in the database. Then, it displays the 
        /// new password in a popup (this will be the only other time
        /// a password can be seen and written down)
        /// </summary>
        public void resetPassword()
        {
            selectedAccount.Password = Security.createUniqueKey();
            DatabaseStore.resetPassword(selectedAccount);
            displayConfirmationPopup("Password reset\nNew password: " + selectedAccount.Password);
        }

        /// <summary>
        /// Adds a new employee account to the database by calling the addStaffAccount()
        /// function in DatabaseStore. First checks whether the username already exists.
        /// If true, a popup informing the user that it exists will display. If false, the
        /// account is added to the database. All fields in the Manage Services page
        /// are required to be filled out in order to add a new account. 
        /// </summary>
        public void addStaffAcc()
        {
            if (!DatabaseStore.isEmployee(newAccount.UserName))
            {
                if (newAccount.UserName != null && newAccount.FirstName != null && newAccount.LastName != null && newAccount.Password != null && newAccount.Admin != null)
                {
                    if (newAccount.UserName != "" && newAccount.FirstName != "" && newAccount.LastName != "")
                    {
                        DatabaseStore.addStaffAccount(newAccount);
                        staffAccounts.Add(newAccount);
                        newAccount = new Staff();
                        OnPropertyChanged(nameof(newAccount));
                    }
                    else displayConfirmationPopup("Invalid field entries");
                }
                else displayConfirmationPopup("Fill out all fields");
            }
            else displayConfirmationPopup("Employee already exists");
        }

        /// <summary>
        /// Permanently deletes a an employee's account from the database.
        /// First verifies that the current user is not the same as the 
        /// account being deleted; if true the deletion is denied, if false 
        /// the account proceeds to get deleted. This scenario only occurs 
        /// if the current user is an admin account and tries to delete themselves.
        /// </summary>
        public void deleteAccount()
        {
            if (selectedAccount.UserName != viewHandler.currentUser)
            {
                DatabaseStore.deleteStaffAccount(selectedAccount.UserName);
                staffAccounts.Remove(selectedAccount);
            }
            else
            {
                displayConfirmationPopup("Cannot delete the user you are signed in as.");
            }
        }
    }
}
