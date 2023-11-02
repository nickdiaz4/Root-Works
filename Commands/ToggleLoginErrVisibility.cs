using System;
using System.Collections.Generic;
using System.Text;
using Catawba.ViewModels;
using System.Windows;

namespace Catawba.Commands
{
    public class ToggleLoginErrVisibility : CommandBase
    {
        private LoginViewModel loginViewModel;

        /// <summary>
        /// Simple command that changes the visibility of the login error message to
        /// visible rather than collapsed when a user enters invalid login credentials.
        /// </summary>
        /// <param name="loginViewModel"></param>
        public ToggleLoginErrVisibility(LoginViewModel loginViewModel)
        {
            this.loginViewModel = loginViewModel;
        }

        public override void Execute(object Parameter)
        {
            loginViewModel.visibility = Visibility.Visible;
        }

    }
}
