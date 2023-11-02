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
    /// Interaction logic for mngServicesPage.xaml
    /// </summary>
    public partial class mngServicesPage : UserControl
    {
        public mngServicesPage()
        {
            InitializeComponent();
        }

        private void servicesDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DependencyObject dep = (DependencyObject)e.OriginalSource;

            while ((dep != null) && !(dep is DataGridCell))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }

            if (dep == null)
                return;

            if (dep is DataGridCell)
            {
                servicesDataGrid.SelectedItem = null;
            }
        }

    }
}
