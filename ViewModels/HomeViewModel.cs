using Catawba.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Windows;

namespace Catawba.ViewModels
{
    /// <summary>
    /// ViewModel used to controls the context of the home view
    /// </summary>
    public class HomeViewModel : ViewModelBase
    {
        public ViewModelHandler viewHandler { get; }
        public HomeViewModel(ViewModelHandler handler)
        {
            this.viewHandler = handler;
        }
    }
}
