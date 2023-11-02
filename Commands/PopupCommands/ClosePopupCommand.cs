using Catawba.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catawba.Commands
{
    public class ClosePopupCommand : CommandBase
    {
        private ViewModelBase viewModelBase;
        private bool executeOption;

        /// <summary>
        /// Commmand binded to the "Close" button on Confirmation Popups. Will remove it from 
        /// the screen. 
        /// </summary>
        /// <param name="viewModelBase"></param>
        /// <param name="executeOption"></param>
        public ClosePopupCommand(ViewModelBase viewModelBase, bool executeOption)
        {
            this.viewModelBase = viewModelBase;
            this.executeOption = executeOption;
        }

        public override void Execute(object parameter)
        {
            viewModelBase.closePopup(executeOption);
        }
    }
}
