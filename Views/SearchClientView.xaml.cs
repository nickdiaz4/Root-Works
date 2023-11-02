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
    /// Interaction logic for SearchClientView.xaml
    /// </summary>
    public partial class SearchClientView : UserControl
    {
        public SearchClientView()
        {
            InitializeComponent();
        }

        private void ComboBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var comboBox = (ComboBox)sender;
            if (!comboBox.IsDropDownOpen)
            {
                e.Handled = true;
            }
        }
    }
}
