using Catawba.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using Catawba.Commands;
using System.Windows;

namespace Catawba.Commands
{
    public class LoginCommand : CommandBase
    {
        private LoginViewModel loginViewModel;

        /// <summary>
        /// Command binded to the "Login" button in the login page.
        /// If the login is successful and the login error message
        /// was visible, the message gets reverted to collapsed.
        /// </summary>
        /// <param name="loginViewModel"></param>
        public LoginCommand(LoginViewModel loginViewModel)
        {
            this.loginViewModel = loginViewModel;
        }

        public override void Execute(object parameter)
        {
            if (loginViewModel.visibility == Visibility.Visible) loginViewModel.visibility = Visibility.Hidden;
            loginViewModel.login();
        }
    }
}
