using Catawba.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catawba.Commands
{
    public class SearchClientsCommand : CommandBase
    {
        private SearchClientsViewModel searchPage;
        private bool showAll;

        /// <summary>
        /// Command binded to the "Search" button in the Search Client View. Fetches clients
        /// from the database that match with the user's search parameters. 
        /// </summary>
        /// <param name="searchPage"></param>
        /// <param name="showAll"> True if the "Show All" button was selected, false otherwise. Displays all
        ///                         clients currently in the database. </param>
        public SearchClientsCommand(SearchClientsViewModel searchPage, bool showAll)
        {
            this.searchPage = searchPage;
            this.showAll = showAll;
        }


        public override void Execute(object parameter)
        {
            searchPage.searchClients(showAll);
        }
    }
}
