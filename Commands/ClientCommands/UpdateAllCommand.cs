using Catawba.Database;
using Catawba.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Catawba.Commands
{
    public class UpdateAllCommand : CommandBase
    {
        AddInfoViewModel addInfoViewmodel;
        bool addNewClients;

        /// <summary>
        /// Command binded to the "Add and Update" button in the Add Client Info page. 
        /// Adds any new pending clients to the database and updates duplicates with 
        /// the new information in the pending client. 
        /// </summary>
        /// <param name="addInfoViewmodel"></param>
        /// <param name="addNewClients"></param>
        public UpdateAllCommand(AddInfoViewModel addInfoViewmodel, bool addNewClients)
        {
            this.addInfoViewmodel = addInfoViewmodel;
            this.addNewClients = addNewClients;
        }
        public override void Execute(object parameter)
        {
            addInfoViewmodel.updateExisting(addNewClients);
        }
    }
}
