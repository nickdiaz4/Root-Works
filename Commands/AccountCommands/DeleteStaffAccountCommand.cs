using Catawba.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace Catawba.Commands
{
    public class DeleteStaffAccountCommand : CommandBase
    {
        private Staff account;
        private ObservableCollection<Staff> staffAccounts;
        private MngAccsViewModel mngAccsViewModel;
        /// <summary>
        /// Command binded to the "Delete" button in the manage accounts administrator
        /// page. Calls the deleteAccount() function in the manage accounts viewmodel.
        /// </summary>
        /// <param name="staffAccounts"></param>
        /// <param name="mngAccsViewModel"></param>
        public DeleteStaffAccountCommand(ObservableCollection<Staff> staffAccounts, MngAccsViewModel mngAccsViewModel)
        {
            this.staffAccounts = staffAccounts;
            this.mngAccsViewModel = mngAccsViewModel;
        }

        public void setAccount(Staff account)
        {
            this.account = account;
        }

        public override void Execute(object parameter)
        {
            mngAccsViewModel.deleteAccount();
        }
    }
}