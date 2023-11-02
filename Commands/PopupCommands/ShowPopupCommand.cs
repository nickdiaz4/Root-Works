using Catawba.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace Catawba.Commands
{
    public class ShowPopupCommand : CommandBase
    {
        private ViewModelBase viewModelBase;

        /// <summary>
        /// For testing purposes. Displays a message to ensure the display popup
        /// is working properly. 
        /// </summary>
        /// <param name="viewModelBase"></param>
        public ShowPopupCommand(ViewModelBase viewModelBase) 
        {
            this.viewModelBase = viewModelBase;
        }

        public override void Execute(object parameter)
        {
            viewModelBase.displayConfirmationPopup("This is a test popup");
            //viewModelBase.displayOptionPopup("this is an option", viewModelBase.closePopupCommand);
        }
    }
}
