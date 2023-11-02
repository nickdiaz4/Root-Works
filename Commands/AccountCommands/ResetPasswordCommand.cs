using Catawba.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catawba.Commands
{
    public class ResetPasswordCommand : CommandBase
    {
        private MngAccsViewModel mngAccsViewModel;
        private Staff account;

        /// <summary>
        /// Command binded to the "Reset Password" button in the manage 
        /// accounts administrator page. 
        /// </summary>
        /// <param name="mngAccsViewModel"></param>
        public ResetPasswordCommand(MngAccsViewModel mngAccsViewModel)
        {
            this.mngAccsViewModel = mngAccsViewModel;
        }

        public void setAccount(Staff account)
        {
            this.account = account;
        }

        public override void Execute(object parameter)
        {
            mngAccsViewModel.resetPassword();
        }
    }
}
