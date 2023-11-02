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
    /// Interaction logic for mngAccsPage.xaml
    /// </summary>
    public partial class mngAccsPage : UserControl
    {
        public mngAccsPage()
        {
            InitializeComponent();
        }

        private void staffDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
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
                staffDataGrid.SelectedItem = null;
            }
        }
    }
}
