using Catawba.Commands;
using Catawba.Database;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using Excel = Microsoft.Office.Interop.Excel;

namespace Catawba.ViewModels
{
    public class AddInfoViewModel : DataViewModelBase
    {
        //private DatabaseConnector database;
        // Commands for buttons on the page
        public ICommand readDataFile { get; set; }
        public ICommand SubmitClients { get; set; }
        public ICommand NavigateToForm { get; set; }
        public UpdateAllCommand addAndUpdateCommand { get; set; }
        public RandomClientCommand randomClientCommand { get; set; }

        /// <summary>
        /// Initialize the view model with a reference to the view model handler and commands.
        /// </summary>
        /// <param name="viewHandler"></param>addAndUpdateCommand
        public AddInfoViewModel(ViewModelHandler viewHandler) : base(viewHandler)
        {
            this.viewHandler = viewHandler;
            randomClientCommand = new RandomClientCommand(this);
            addAndUpdateCommand = new UpdateAllCommand(this, true);
            readDataFile = new ReadDataFile(this);
            SubmitClients = new SubmitClients(this);
            NavigateToForm = new NavigateClientForm(viewHandler, this, new Client(), true);
        }

        /// <summary>
        /// Add currentclient to the client list(observableCollection)
        /// </summary>
        public void addClient(Client client)
        {
            // add client to client list in DataViewModelBase
            clientList.Add(client);

            // create form to new client
            NavigateToForm = new NavigateClientForm(viewHandler, this, new Client(), true);
        }

        public void addRandomClient(Client client)
        {
            clientList.Add(client);
            //for (int i = 0; i < 100; i++)
            //{
            //    clientList.Add(ClientRandomizer.RandomClient());
            //}
        }

        /// <summary>
        /// If a duplicate client is identified, this updates the existing client
        /// that matches the duplicate with any new information the duplicate contains.
        /// Assumes that the duplicate has newer and more relevant information. 
        /// </summary>
        /// <param name="addNewClients"></param>
        public void updateExisting(bool addNewClients)
        {
            try
            {
                if (addNewClients)
                {
                    displayWaitingPopup("Attempting to add and update clients...");
                    AllowUIToUpdate();

                    DatabaseStore.updateClient(clientList, true);
                    clientList.Clear();

                    closePopup(false);
                    displayTimedPopup("Client successfully entered and updated.");
                }
                else
                {
                    displayWaitingPopup("Attempting to update clients...");
                    AllowUIToUpdate();

                    DatabaseStore.updateClient(clientList, false);
                    clientList.Clear();

                    closePopup(false);
                    displayTimedPopup("Client update successful.");
                }
            }
            catch (Exception e)
            {
                closePopup(false);
                displayConfirmationPopup(e.Message);
            }
        }

        /// <summary>
        /// Submits the pending clients in the datagrid view to the database
        /// </summary>
        public void submitNewClients()
        {
            try
            {
                displayWaitingPopup("Attempting to add clients...");
                AllowUIToUpdate();
                clientList = DatabaseStore.addClients(clientList);
                if (clientList.Count == 0)
                {
                    closePopup(false);
                    displayTimedPopup("Clients submitted successfully.");
                }
                else
                {
                    //closePopup(false);
                    displayOptionPopup("Duplicate clients detected, Would you like to update current client information?", new UpdateAllCommand(this,false));
                }
            }
            catch (Exception e)
            {
                closePopup(false);
                displayConfirmationPopup(e.Message);
            }
        }

        /// <summary>
        /// Reads in data from excel file will abort if it encounters invalid or unsupported data. On success it will populate the 
        /// clientlist with all the clients it read in.
        /// </summary>
        /// <param name="filename"></param>
        public void readExcelFile(string filename)
        {

            // display popup while file is being loaded
            displayWaitingPopup("Attempting to load file...");
            AllowUIToUpdate();

            ObservableCollection<Client> tempClients = new ObservableCollection<Client>();

            Excel.Application dataFile = new Excel.Application();
            Excel.Workbook workBook = null;
            dataFile.Visible = false;

            // Check if the selected file is an Excel file. Abort process if not
            if (!filename.Contains(".xlsx"))
            {
                displayConfirmationPopup("Selected file is not an Excel file.");
                return;
            }
            else
            {
                try
                {
                    workBook = dataFile.Workbooks.Open(filename);
                }
                catch (Exception e)
                {
                    displayConfirmationPopup("Unable to open file.");
                    return;
                }
            }

            Excel.Worksheet mySheet;
            Excel.Range myRange;

            mySheet = (Excel.Worksheet)workBook.Worksheets[1];
            myRange = mySheet.UsedRange;

            List<string> databaseServices = DatabaseStore.getAllServices();

            // go through each row in the excel file attempting to save each cell into a client object
            for (int row = 2; row <= myRange.Rows.Count; row++)
            {
                try
                {
                    Client client = new Client();

                    // set string values to null if cell is empty otherwise set them to string value of cell
                    client.FirstName = (mySheet.Cells[row, 2] as Excel.Range).Value?.ToString();
                    client.MiddleName = (mySheet.Cells[row, 3] as Excel.Range).Value?.ToString();
                    client.LastName = (mySheet.Cells[row, 4] as Excel.Range).Value?.ToString();
                    client.Dob = (mySheet.Cells[row, 5] as Excel.Range).Value?.ToString();

                    // check for required information
                    if (client.FirstName == null || client.LastName == null || client.Dob == null)
                    {
                        displayConfirmationPopup($"Client on row {row} is missing required identifying information!");
                        tempClients.Clear();
                        break;
                    }

                    try
                    {
                        // convert dob to dateTime format
                        client.Dob = Convert.ToDateTime(client.Dob).ToString("MM/dd/yyyy");
                        // check if date met is null if not then convert to dateTime
                        if ((mySheet.Cells[row, 1] as Excel.Range).Value != null)
                        {
                            client.DateMet = Convert.ToDateTime((mySheet.Cells[row, 1] as Excel.Range).Value.ToString()).ToString("MM/dd/yyyy");
                        }
                        else
                        {
                            client.DateMet = null;
                        }
                    }
                    catch (System.FormatException e)
                    {
                        displayConfirmationPopup($"Invalid Date in row {row}.");
                        tempClients.Clear();
                        break;
                    }

                    client.MiddleName = (mySheet.Cells[row, 3] as Excel.Range).Value?.ToString();
                    client.Address = (mySheet.Cells[row, 6] as Excel.Range).Value?.ToString();
                    client.PoBox = (mySheet.Cells[row, 7] as Excel.Range).Value?.ToString();
                    client.AptNum = (mySheet.Cells[row, 8] as Excel.Range).Value?.ToString();
                    client.State = (mySheet.Cells[row, 9] as Excel.Range).Value?.ToString();

                    // check if state is invalid
                    if (client.State !=null && client.State.Equals("invalid"))
                    {
                        displayConfirmationPopup($"Invalid State in row {row}.");
                        tempClients.Clear();
                        break;
                    }

                    client.County = (mySheet.Cells[row, 10] as Excel.Range).Value?.ToString();
                    client.City = (mySheet.Cells[row, 11] as Excel.Range).Value?.ToString();
                    client.PhoneNumber = (mySheet.Cells[row, 13] as Excel.Range).Value?.ToString();
                    client.Zipcode = (mySheet.Cells[row, 12] as Excel.Range).Value?.ToString();
                    client.MaritalStatus = (mySheet.Cells[row, 19] as Excel.Range).Value?.ToString();

                    //Emergency Contact Excel file input fields here
                    client.EContact.EmergencyFirstName = (mySheet.Cells[row, 20] as Excel.Range).Value?.ToString();
                    client.EContact.EmergencyLastName = (mySheet.Cells[row, 21] as Excel.Range).Value?.ToString();
                    client.EContact.EmergencyPhoneNumber = (mySheet.Cells[row, 22] as Excel.Range).Value?.ToString();
                    client.EContact.EmergencyRelationship = (mySheet.Cells[row, 23] as Excel.Range).Value?.ToString();

                    // check if income is null
                    if ((mySheet.Cells[row, 14] as Excel.Range).Value != null)
                    {
                        client.Income = int.Parse((mySheet.Cells[row, 14] as Excel.Range).Value.ToString());
                    }
                    else
                    {
                        client.Income = null;
                    }

                    // check if HouseholdMembers is null
                    if ((mySheet.Cells[row, 15] as Excel.Range).Value != null)
                    {
                        client.HouseholdMembers = sbyte.Parse((mySheet.Cells[row, 15] as Excel.Range).Value.ToString());
                    }
                    else
                    {
                        client.HouseholdMembers = null;
                    }

                    // check if SocialWorkStatus is null
                    if ((mySheet.Cells[row, 16] as Excel.Range).Value != null)
                    {
                        client.SocialWorkStatus = bool.Parse((mySheet.Cells[row, 16] as Excel.Range).Value.ToString());
                    }
                    else
                    {
                        client.SocialWorkStatus = null;
                    }

                    // check if MedicaidStatus is null
                    if ((mySheet.Cells[row, 17] as Excel.Range).Value != null)
                    {
                        client.MedicaidStatus = bool.Parse((mySheet.Cells[row, 17] as Excel.Range).Value.ToString());
                    }
                    else
                    {
                        client.MedicaidStatus = null;
                    }

                    // check if MedicareStatus is null
                    if ((mySheet.Cells[row, 18] as Excel.Range).Value != null)
                    {
                        client.MedicareStatus = bool.Parse((mySheet.Cells[row, 18] as Excel.Range).Value.ToString());
                    }
                    else
                    {
                        client.MedicareStatus = null;
                    }

                    //compare tempList to serviceList; if something in tempList is not in serviceList then throw error
                    string servicesData = (mySheet.Cells[row, 24] as Excel.Range).Value?.ToString(); //the list of services in the Excel file row

                    List<string> fileServiceList = new List<string>();

                    // make sure services exist and it's not empty
                    if (servicesData != null)
                    {
                        servicesData = servicesData.Trim();
                        if (servicesData != "")
                        {
                            fileServiceList = servicesData.Split(',').ToList();
                        }
                    }

                    if (fileServiceList.Count != 0)
                        client.shouldUpdateServices = true;

                    // validate each service break from parsing the file if a service not in the DB is detected
                    foreach (string service in fileServiceList)
                    {
                        string formattedService = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(service.Trim());
                        if (!databaseServices.Contains(formattedService))
                        {
                            displayConfirmationPopup($"Invalid service in row {row}");
                            tempClients.Clear();
                            goto fileParseEnd;
                        }
                        else
                        {
                            client.addService(formattedService);
                        }
                    }

                    client.Notes = (string)(mySheet.Cells[row, 25] as Excel.Range).Text;

                    //if (client.Notes.Length > 500)
                    //{
                    //    displayConfirmationPopup($"Unable to add clients.\n The Notes column in row {row} has too many characters.");
                    //    tempClients.Clear();
                    //    break;
                    //}

                    tempClients.Add(client);
                }
                catch (Exception ex)
                {
                    displayConfirmationPopup($"A cell in row {row} is not formatted properly and/or contains an invalid data type.");
                    tempClients.Clear();
                    break;
                }
                displayConfirmationPopup($"File successfully loaded.");
            }

            // label to exit parsing if trying to add a service not in the DB
            fileParseEnd:

            // add new clients
            foreach (Client newClient in tempClients)
            {
                addClient(newClient);
            }

            // close file
            workBook.Close(false, false);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(workBook);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(dataFile);
            dataFile = null;
        }
    }
}