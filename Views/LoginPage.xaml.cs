using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Catawba.Views
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : UserControl
    {

        private void PasswordChanged(object sender, RoutedEventArgs e)
        {
            if(this.DataContext != null)
            {
                ((dynamic)this.DataContext).loginAccount.Password = ((PasswordBox)sender).Password;
            }
        }
        public LoginPage()
        {
            InitializeComponent();
        }
    }
}
