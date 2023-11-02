using Catawba.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catawba.Commands
{
    public class ClearSearchParametersCommand : CommandBase
    {
        private SearchClientsViewModel searchViewModel;

        /// <summary>
        /// Command binded to the "Clear All" button in the search clients page.
        /// This resets the search parameters to nothing. 
        /// </summary>
        /// <param name="searchViewModel"></param>
        public ClearSearchParametersCommand(SearchClientsViewModel searchViewModel)
        {
            this.searchViewModel = searchViewModel;
        }
        public override void Execute(object parameter)
        {
            searchViewModel.ClearSearchParameters();
        }
    }
}
