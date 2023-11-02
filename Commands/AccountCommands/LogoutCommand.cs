using Catawba.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catawba.Commands
{
    public class LogoutCommand : CommandBase
    {
        ViewModelHandler viewHandler;

        public LogoutCommand(ViewModelHandler viewHandler)
        {
            this.viewHandler = viewHandler;
        }
        public override void Execute(object parameter)
        {
            viewHandler.logout();
        }
    }
}
