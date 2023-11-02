using Catawba.ViewModels;

namespace Catawba.Commands.ClientCommands
{
    public class RefreshClientsCommand : CommandBase
    {
        private ViewInfoViewModel viewInfoViewModel;

        /// <summary>
        /// Command binded to the "Refresh Clients" button in the View Client
        /// Info page. Updates clients in the data grid view box with any new information
        /// that could have been added while being viewed.
        /// </summary>
        /// <param name="viewInfoViewModel"></param>
        public RefreshClientsCommand(ViewInfoViewModel viewInfoViewModel)
        {
            this.viewInfoViewModel = viewInfoViewModel;
        }

        public override void Execute(object parameter)
        {
            viewInfoViewModel.refreshClients();
        }
    }
}