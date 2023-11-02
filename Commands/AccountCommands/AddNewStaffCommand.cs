using Catawba.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catawba.Commands
{
    public class AddNewStaffCommand : CommandBase
    {
        private MngAccsViewModel mngAccsViewModel;

        /// <summary>
        /// Command binded to the "Add" button in the manage accounts 
        /// administrator page. Calls the addStaffAcc(0 function in the 
        /// manage accounts viewmodel. 
        /// </summary>
        /// <param name="mngAccsViewModel"></param>
        public AddNewStaffCommand(MngAccsViewModel mngAccsViewModel)
        {
            this.mngAccsViewModel = mngAccsViewModel;
        }

        public override void Execute(object parameter)
        {
            mngAccsViewModel.addStaffAcc();
        }
    }
}
