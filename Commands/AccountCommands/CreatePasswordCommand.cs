using Catawba.ViewModels;
using Catawba.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catawba.Commands
{
    public class CreatePasswordCommand : CommandBase
    {
        private MngAccsViewModel mngAccsViewModel;

        /// <summary>
        /// Command binded to the "Create Password" button in the manage accounts
        /// administrator page. Calls the createPassword() function in the manage
        /// accounts viewmodel. 
        /// </summary>
        /// <param name="mngAccsViewmodel"></param>
        public CreatePasswordCommand(MngAccsViewModel mngAccsViewmodel)
        {
            this.mngAccsViewModel = mngAccsViewmodel;
        }

        public override void Execute(object parameter)
        {
            mngAccsViewModel.createPassword();
        }
    }
}
