using Catawba.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Catawba.Commands
{
    public class DisplayPromptCommand : CommandBase
    {
        ICommand command;
        string message;
        ViewModelBase viewModelBase;

        /// <summary>
        /// Command to display any one of the popups.
        /// </summary>
        /// <param name="viewModelBase"></param>
        /// <param name="message"></param>
        /// <param name="command"></param>
        public DisplayPromptCommand(ViewModelBase viewModelBase,string message, ICommand command)
        {
            this.viewModelBase = viewModelBase;
            this.message = message;
            this.command = command;
        }
        public override void Execute(object parameter)
        {
            viewModelBase.displayOptionPopup(message, command);
        }
    }
}
