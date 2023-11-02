using Catawba.ViewModels;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catawba.Commands
{
    public class ReadDataFile : CommandBase
    {
        public AddInfoViewModel vm;
        public ReadDataFile(AddInfoViewModel vm)
        {
            this.vm = vm;
        }
        public override void Execute(object parameter)
        {

            string filename = "";
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select an Excel File to Process";
            openFileDialog.Filter = "Excel files (*.xlsx) | *.xlsx| All Files(*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                filename = openFileDialog.FileName;
                vm.readExcelFile(filename);
            }
        }
    }
}
