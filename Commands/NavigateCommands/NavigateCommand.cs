using Catawba.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Catawba.Commands
{
    public class NavigateCommand : CommandBase
    {
        public ViewModelHandler viewHandler { get; set; }
        public object destination { get; set; }

        /// <summary>
        /// Command binded to the button tabs that navigate to each of the pages. 
        /// </summary>
        /// <param name="viewHandler"> Page currently on. </param>
        /// <param name="destination"> Page that the user is going to. </param>
        public NavigateCommand(ViewModelHandler viewHandler, object destination)
        {
            this.viewHandler = viewHandler;
            this.destination = destination;
        }

        public override void Execute(object parameter)
        {
            // If returning to addInfoView from the client form then reset the client and update the navigation command so
            // information does not stay when returning to the client form
            if (viewHandler.currentViewModel is AddInfoFormViewModel && destination is AddInfoViewModel)
            {
                AddInfoViewModel viewModel = destination as AddInfoViewModel;
                viewModel.NavigateToForm = new NavigateClientForm(viewHandler, viewModel, new Client(), true);
            }

            // if navigating back from the client form, i.e. clicking return, set client to the original so values are not saved
            // parameter will be null if the command is not forced executed like in manageClientCommand
            if (viewHandler.currentViewModel is AddInfoFormViewModel && parameter == null)
            {
                AddInfoFormViewModel viewModel = viewHandler.currentViewModel as AddInfoFormViewModel;
                DataViewModelBase dataViewModel = destination as DataViewModelBase;
                dataViewModel.restoreClient(viewModel.originalClient);
            }
            viewHandler.currentViewModel = destination;
        }
    }
}
