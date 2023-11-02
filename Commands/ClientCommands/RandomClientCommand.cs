using Catawba.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catawba.Commands
{
    public class RandomClientCommand : CommandBase
    {
        private AddInfoViewModel addClientPage;

        /// <summary>
        /// For testing and presentation purposes. Creates and adds a client with
        /// arbitrary information to the pending clients list.
        /// </summary>
        /// <param name="addClientPage"></param>
        public RandomClientCommand(AddInfoViewModel addClientPage)
        {
            this.addClientPage = addClientPage;
        }
        public override void Execute(object parameter)
        {
            addClientPage.addRandomClient(ClientRandomizer.RandomClient());
        }
    }
}
