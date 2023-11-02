using Catawba.Entities;
using Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;

using Excel = Microsoft.Office.Interop.Excel;

namespace Catawba.Database
{
    public static class DatabaseStore
    {
        public static string activeAccount;

        // Database column names
        //private static string rwcid = "rwcid";
        //private static string firstName = "firstName";
        //private static string middleInitial = "middleInitial";
        //private static string lastName = "lastName";
        private static string dob = "dateOfBirth";
        //private static string address = "address";
        //private static string city = "city";
        //private static string state = "state";
        //private static string county = "county";
        //private static string zipCode = "zipCode";
        //private static string phoneNumber = "phoneNumber";
        private static string income = "income";
        //private static string maritalStatus = "maritalStatus";
        private static string numHousehold = "numberOfHouseholdMembers";
        private static string medicareStatus = "medicareStatus";
        private static string medicaidStatus = "medicaidStatus";
        private static string socialWorkerStatus = "socialWorkerStatus";
        //private static string notes = "notes";
        //private static string eFirst = "emergencyContactFirstName";
        //private static string eLast = "emergencyContactLastName";
        //private static string ePhone = "emergencyContactPhoneNumber";
        //private static string eRelationship = "emergencyContactRelationship";
        private static string clientServices = "clientServices";

        // Client information columns
        private static int rwcidCol = 0;
        private static int firstNameCol = 1;
        private static int middleNameCol = 2;
        private static int lastNameCol = 3;
        //private static int dobCol = 4;
        private static int addressCol = 5;
        private static int cityCol = 6;
        private static int stateCol = 7;
        private static int countyCol = 8;
        private static int zipcodeCol = 9;
        private static int phoneNumberCol = 10;
        //private static int incomeCol = 11;
        private static int maritalStatusCol = 12;
        //private static int numberOfHouseholdMembersCol = 13;
        //private static int medicareStatusCol = 14;
        //private static int medicaidStatusCol = 15;
        //private static int socialWorkerStatusCol = 16;
        private static int notesCol = 17;
        private static int emergencyContactFirstNameCol = 18;
        private static int emergencyContactLastNameCol = 19;
        private static int emergencyContactPhonenumberCol = 20;
        private static int emergencyCOntactRelationshipCol = 21;
        private static int aptNumCol = 22;
        private static int poBoxCol = 23;
        private static int dateMetCol = 24;

        /***************************************************************/

        // Staff information columns
        private static int usernameCol = 0;

        private static int staffFirstNameCol = 1;
        private static int staffLastNameCol = 2;
        //int adminCol = 3; <- Add if needed for CRUD operations
        /****************************************************************/

        // Service information columns
        //int sIDCol = 0; <- Add if needed for CRUD operations
        private static int serviceNameCol = 0;

        /****************************************************************
         *
         *
         *                       CLIENT FUNCTIONS
         *
         *
        /****************************************************************/

        /// <summary>
        /// Verifies that a client pending submission does not already exist in the database.
        /// Duplicate clients are identified by identical first and last names, and birthday.
        /// </summary>
        /// <param name="clients"> List of pending clients to be checked against the database</param>
        /// <returns>dupeClients: a list of clients that have been marked as duplicates. </returns>
        public static ObservableCollection<Client> isInDatabaseGroup(ObservableCollection<Client> clients)
        {
            ObservableCollection<Client> dupeClients = new ObservableCollection<Client>();
            MySqlConnection connection = DatabaseConnector.GetConnection();
            Dictionary<string, Client> clientObjectAndInfoPair = new Dictionary<string, Client>();
            string dupeCheckQuery = "SELECT firstName,lastName,dateOfBirth FROM client WHERE ";

            // for each client in the list prepare the paramterized statement
            for (int index = 0; index < clients.Count; index++)
            {
                Client currentClient = clients[index];
                string indexValue = index.ToString();
                if (index == 0)
                {
                    dupeCheckQuery += $"(firstName=@firstName{indexValue} AND lastName=@lastName{indexValue} AND dateOfBirth=@dob{indexValue})";
                }
                else
                {
                    dupeCheckQuery += $" OR (firstName=@firstName{indexValue} AND lastName=@lastName{indexValue} AND dateOfBirth=@dob{indexValue})";
                }

                // add reference information for client object
                clientObjectAndInfoPair.Add($"{currentClient.FirstName.ToLower()},{currentClient.LastName.ToLower()},{Convert.ToDateTime(currentClient.Dob).ToString("MM/dd/yyyy").ToLower()}", currentClient);
            }

            // open connection
            connection.Open();

            // prepare command
            MySqlCommand dupeCheckCommand = new MySqlCommand(dupeCheckQuery, connection);

            // fill parameter values in command
            for (int index = 0; index < clients.Count; index++)
            {
                Client currentClient = clients[index];
                string indexValue = index.ToString();

                dupeCheckCommand.Parameters.AddWithValue($"@firstName{indexValue}", currentClient.FirstName);
                dupeCheckCommand.Parameters.AddWithValue($"@lastName{indexValue}", currentClient.LastName);
                dupeCheckCommand.Parameters.AddWithValue($"@dob{indexValue}", Convert.ToDateTime(currentClient.Dob));
            }

            // execute command with reader
            MySqlDataReader dupeReader = dupeCheckCommand.ExecuteReader();

            // reader will contain duplicate clients so add those to the dupe list
            while (dupeReader.Read())
            {
                string clientInfo = $"{dupeReader.GetString(0).ToLower()},{dupeReader.GetString(1).ToLower()},{Convert.ToDateTime(dupeReader[dob]).ToString("MM/dd/yyyy").ToLower()}";
                dupeClients.Add(clientObjectAndInfoPair[clientInfo]);
            }

            connection.Close();
            return dupeClients;
        }

        /// <summary>
        /// Adds a list of valid clients to the "client" table in the database, while any
        /// duplicates that are identified are stored in a collection for further action.
        /// If the addition fails, the transaction is rolled back to its previous state,
        /// and the entire list of clients to be added is returned. 
        /// </summary>
        /// <param name="clients"> List of pending clients to be added to the database</param>
        /// <returns> A list of duplicates to display on the data grid. </returns>
        public static ObservableCollection<Client> addClients(ObservableCollection<Client> clients)
        {
            validateAccount();

            // new transaction because isInDatabase() would use the same connection and cause conflict
            MySqlConnection transactionConn = DatabaseConnector.GetConnection();
            ObservableCollection<Client> dupeClient = isInDatabaseGroup(clients);
            // open sql connection
            transactionConn.Open();

            // Start a transaction so all clients or no clients are added
            MySqlTransaction transaction = transactionConn.BeginTransaction();

            // parameterize insert statement, will also output the rwcid of the client inserted
            string insertStatement = "INSERT INTO client VALUES (NULL,@first,@middle,@last,@dob" +
                                     ",@address,@city,@state,@county,@zip,@phone,@income,@marital" +
                                     ",@household,@medicare,@medicaid,@social,@notes,@eFirst,@eLast" +
                                     ",@ephone,@eRelationship,@aptNum,@poBox,@dateMet); SELECT LAST_INSERT_ID();";

            MySqlCommand command = new MySqlCommand("", transactionConn, transaction);
            MySqlCommand serviceCommand = new MySqlCommand("", transactionConn, transaction);
            MySqlDataReader idReader;

            try
            {
                // check duplicate count and remove any duplicates while setting their duplicate flag to true
                if (dupeClient.Count != 0)
                {
                    foreach (Client dupe in dupeClient)
                    {
                        dupe.isDupe = true;
                        clients.Remove(dupe);
                    }
                }

                foreach (Client client in clients)
                {
                    int rwcid;

                    command.Parameters.Clear();
                    command.CommandText = insertStatement;
                    // Set parameterized values.
                    // If value is null replace with DBNull to database recognizes it as a null value
                    command.Parameters.AddWithValue("@first", client.FirstName ?? (Object)DBNull.Value);
                    command.Parameters.AddWithValue("@middle", client.MiddleName ?? (Object)DBNull.Value);
                    command.Parameters.AddWithValue("@last", client.LastName ?? (Object)DBNull.Value);
                    command.Parameters.AddWithValue("@dob", Convert.ToDateTime(client.Dob));
                    command.Parameters.AddWithValue("@address", client.Address ?? (Object)DBNull.Value);
                    command.Parameters.AddWithValue("@city", client.City ?? (Object)DBNull.Value);
                    command.Parameters.AddWithValue("@state", client.State ?? (Object)DBNull.Value);
                    command.Parameters.AddWithValue("@county", client.County ?? (Object)DBNull.Value);
                    command.Parameters.AddWithValue("@zip", client.Zipcode ?? (Object)DBNull.Value);
                    command.Parameters.AddWithValue("@phone", client.PhoneNumber ?? (Object)DBNull.Value);
                    command.Parameters.AddWithValue("@income", client.Income ?? (Object)DBNull.Value);
                    command.Parameters.AddWithValue("@notes", client.Notes ?? (Object)DBNull.Value);
                    command.Parameters.AddWithValue("@marital", client.MaritalStatus ?? (Object)DBNull.Value);
                    command.Parameters.AddWithValue("@household", client.HouseholdMembers ?? (Object)DBNull.Value);
                    command.Parameters.AddWithValue("@medicare", client.MedicareStatus ?? (Object)DBNull.Value);
                    command.Parameters.AddWithValue("@medicaid", client.MedicaidStatus ?? (Object)DBNull.Value);
                    command.Parameters.AddWithValue("@social", client.SocialWorkStatus ?? (Object)DBNull.Value);
                    command.Parameters.AddWithValue("@eFirst", client.EContact.EmergencyFirstName ?? (Object)DBNull.Value);
                    command.Parameters.AddWithValue("@eLast", client.EContact.EmergencyLastName ?? (Object)DBNull.Value);
                    command.Parameters.AddWithValue("@ephone", client.EContact.EmergencyPhoneNumber ?? (Object)DBNull.Value);
                    command.Parameters.AddWithValue("@eRelationship", client.EContact.EmergencyRelationship ?? (Object)DBNull.Value);
                    command.Parameters.AddWithValue("@aptNum", client.AptNum ?? (Object)DBNull.Value);
                    command.Parameters.AddWithValue("@poBox", client.PoBox ?? (Object)DBNull.Value);
                    command.Parameters.AddWithValue("@dateMet", Convert.ToDateTime(client.DateMet));

                    // get client id from most recent insert
                    idReader = command.ExecuteReader();
                    idReader.Read();
                    rwcid = idReader.GetInt32(0);
                    idReader.Close();

                    if (client.Services.Count != 0)
                    {
                        // for each client services add it into the clientServices table
                        int serviceCount = client.Services.Count;
                        string insertServices = "INSERT INTO clientServices (rwcid,sid) VALUES ";
                        for (int count = 1; count <= serviceCount; count++)
                        {
                            if (count != serviceCount)
                                insertServices += "((" + rwcid + ") ,(SELECT sid FROM services WHERE serviceName=@serviceName" + count.ToString() + ")),";
                            else
                            {
                                insertServices += "((" + rwcid + ") ,(SELECT sid FROM services WHERE serviceName=@serviceName" + count.ToString() + "))";
                            }
                        }
                        serviceCommand.Parameters.Clear();
                        serviceCommand.CommandText = insertServices;

                        int serviceNum = 1;
                        foreach (string service in client.Services)
                        {
                            serviceCommand.Parameters.AddWithValue("@serviceName" + serviceNum.ToString(), service);
                            serviceNum++;
                        }

                        // execute command
                        serviceCommand.ExecuteNonQuery();
                    }
                }
                transaction.Commit();
                transactionConn.Close();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                transactionConn.Close();
                return clients;
            }
            return dupeClient;
        }

        /// <summary>
        /// Permanently deletes a client from the database.
        /// </summary>
        /// <param name="client"> The selected client to be deleted. </param>
        public static void deleteClient(Client client)
        {
            validateAccount();

            // new transaction because isInDatabase() would use the same connection and cause conflict
            MySqlConnection transactionConn = DatabaseConnector.GetConnection();

            // open sql connection
            transactionConn.Open();
            //if in the DB
            if (isInDatabase(client))
            {
                MySqlTransaction transaction = transactionConn.BeginTransaction();
                // parameterize
                String deleteStatement = "DELETE FROM client WHERE firstName = @first AND lastName = @last AND dateOfBirth = @dob";

                MySqlCommand command = new MySqlCommand(deleteStatement, transactionConn);
                command.Transaction = transaction;

                command.Parameters.Clear();
                command.CommandText = deleteStatement;

                command.Parameters.AddWithValue("@first", client.FirstName ?? (Object)DBNull.Value);
                command.Parameters.AddWithValue("@last", client.LastName ?? (Object)DBNull.Value);
                command.Parameters.AddWithValue("@dob", Convert.ToDateTime(client.Dob));

                command.ExecuteNonQuery();

                transaction.Commit();
                transactionConn.Close();
            }
            //IF NOT
            else
            {
                Debug.WriteLine("Client could not be found for deletion");
            }
        }

        /// <summary>
        /// Fetches a list of clients from the database that match the user's
        /// search parameters. 
        /// </summary>
        /// <param name="command"> the SQL command containing the parameters to search the "client" table. </param>
        /// <returns> A list of clients matching the search parameters. </returns>
        public static ObservableCollection<Client> getClients(MySqlCommand command)
        {
            validateAccount();

            MySqlDataReader queryReader;
            MySqlConnection connection = DatabaseConnector.GetConnection();

            ObservableCollection<Client> clients = new ObservableCollection<Client>();
            command.Connection = connection;
            connection.Open();
            queryReader = command.ExecuteReader();

            while (queryReader.Read())
            {
                Client client = new Client();
                client.rwcid = queryReader.GetInt32(rwcidCol);
                client.FirstName = queryReader.GetValue(firstNameCol).ToString();
                client.MiddleName = queryReader.GetValue(middleNameCol).ToString();
                client.LastName = queryReader.GetValue(lastNameCol).ToString();
                client.Dob = Convert.ToDateTime(queryReader[dob]).ToString("MM/dd/yyyy");
                client.Address = queryReader.GetValue(addressCol).ToString();
                client.City = queryReader.GetValue(cityCol).ToString();
                client.State = queryReader.GetValue(stateCol).ToString();
                client.County = queryReader.GetValue(countyCol).ToString();
                client.Zipcode = queryReader.GetValue(zipcodeCol).ToString();
                client.PhoneNumber = queryReader.GetValue(phoneNumberCol).ToString();
                client.Income = Convert.IsDBNull(queryReader[income]) ? null : (int?)queryReader[income];
                client.MaritalStatus = queryReader.GetValue(maritalStatusCol).ToString();
                client.HouseholdMembers = Convert.IsDBNull(queryReader[numHousehold]) ? null : (sbyte?)queryReader[numHousehold];
                client.MedicareStatus = Convert.IsDBNull(queryReader[medicareStatus]) ? null : (bool?)queryReader[medicareStatus];
                client.MedicaidStatus = Convert.IsDBNull(queryReader[medicaidStatus]) ? null : (bool?)queryReader[medicaidStatus];
                client.SocialWorkStatus = Convert.IsDBNull(queryReader[socialWorkerStatus]) ? null : (bool?)queryReader[socialWorkerStatus];
                client.Notes = queryReader.GetValue(notesCol).ToString();
                client.EContact.EmergencyFirstName = queryReader.GetValue(emergencyContactFirstNameCol).ToString();
                client.EContact.EmergencyLastName = queryReader.GetValue(emergencyContactLastNameCol).ToString();
                client.EContact.EmergencyPhoneNumber = queryReader.GetValue(emergencyContactPhonenumberCol).ToString();
                client.EContact.EmergencyRelationship = queryReader.GetValue(emergencyCOntactRelationshipCol).ToString();
                client.Services = Convert.IsDBNull(queryReader[clientServices]) ? new List<string>() : new List<string>(queryReader[clientServices].ToString().Split(','));
                client.AptNum = queryReader.GetValue(aptNumCol).ToString();
                client.PoBox = queryReader.GetValue(poBoxCol).ToString();
                client.DateMet = Convert.IsDBNull(queryReader[dateMetCol]) ? null : Convert.ToDateTime(queryReader[dateMetCol]).ToString("MM/dd/yyyy");
                clients.Add(client);
            }

            queryReader.Close();
            connection.Close();
            return clients;
        }

        public static void updateClient(Client client)
        {
            updateClient(new ObservableCollection<Client>(new Client[] { client }),false);
        }

        public static void updateClient(ObservableCollection<Client> clients, bool addNewClients)
        {
            validateAccount();

            // check to add clients first
            if (addNewClients)
            {
                clients = addClients(clients);
            }

            // return of there are no clients left
            if (clients.Count == 0)
            {
                return;
            }

            MySqlDataReader idReader;
            // new transaction because isInDatabase() would use the same connection and cause conflict
            MySqlConnection transactionConn = DatabaseConnector.GetConnection();

            // open sql connection
            transactionConn.Open();

            MySqlTransaction transaction = transactionConn.BeginTransaction();

            int rwcid;

            // parameterize
            String updateStatement = "UPDATE client SET address = COALESCE(@address, address), city = COALESCE(@city, city), " +
                                    "state = COALESCE(@state, state), county = COALESCE(@county, county), " +
                                    "zipCode = COALESCE(@zip, zipCode), phoneNumber = COALESCE(@phone, phoneNumber), " +
                                    "income = COALESCE(@income, income), maritalStatus = COALESCE(@marital, maritalStatus), " +
                                    "notes = COALESCE(@notes, notes), numberOfHouseholdMembers = COALESCE(@household, numberOfHouseholdMembers), " +
                                    "medicareStatus = COALESCE(@medicare, medicareStatus), medicaidStatus = COALESCE(@medicaid, medicaidStatus), " +
                                    "socialWorkerStatus = COALESCE(@social, socialWorkerStatus), " +
                                    "emergencyContactFirstName = COALESCE(@eFirst, emergencyContactFirstName), " +
                                    "emergencyContactLastName = COALESCE(@eLast, emergencyContactLastName), " +
                                    "emergencyContactPhoneNumber = COALESCE(@ephone, emergencyContactPhoneNumber)," +
                                    " emergencyContactRelationship = COALESCE(@eRelationship, emergencyContactRelationship), " +
                                    "aptNumber = COALESCE(@aptNum, aptNumber), poBoxNumber = COALESCE(@poBox, poBoxNumber), " +
                                    "addedDate = COALESCE(@dateMet, addedDate) WHERE firstName = @first AND lastName = @last AND dateOfBirth = @dob;";

            MySqlCommand command = new MySqlCommand("", transactionConn);
            try
            {
                foreach (Client client in clients)
                {
                    command.Parameters.Clear();
                    command.CommandText = updateStatement;

                    command.Parameters.AddWithValue("@first", client.FirstName ?? (Object)DBNull.Value);
                    command.Parameters.AddWithValue("@middle", client.MiddleName ?? (Object)DBNull.Value);
                    command.Parameters.AddWithValue("@last", client.LastName ?? (Object)DBNull.Value);
                    command.Parameters.AddWithValue("@dob", Convert.ToDateTime(client.Dob));
                    command.Parameters.AddWithValue("@address", client.Address ?? (Object)DBNull.Value);
                    command.Parameters.AddWithValue("@city", client.City ?? (Object)DBNull.Value);
                    command.Parameters.AddWithValue("@state", client.State ?? (Object)DBNull.Value);
                    command.Parameters.AddWithValue("@county", client.County ?? (Object)DBNull.Value);
                    command.Parameters.AddWithValue("@zip", client.Zipcode ?? (Object)DBNull.Value);
                    command.Parameters.AddWithValue("@phone", client.PhoneNumber ?? (Object)DBNull.Value);
                    command.Parameters.AddWithValue("@income", client.Income ?? (Object)DBNull.Value);
                    command.Parameters.AddWithValue("@notes", client.Notes ?? (Object)DBNull.Value);
                    command.Parameters.AddWithValue("@marital", client.MaritalStatus ?? (Object)DBNull.Value);
                    command.Parameters.AddWithValue("@household", client.HouseholdMembers ?? (Object)DBNull.Value);
                    command.Parameters.AddWithValue("@medicare", client.MedicareStatus ?? (Object)DBNull.Value);
                    command.Parameters.AddWithValue("@medicaid", client.MedicaidStatus ?? (Object)DBNull.Value);
                    command.Parameters.AddWithValue("@social", client.SocialWorkStatus ?? (Object)DBNull.Value);
                    command.Parameters.AddWithValue("@eFirst", client.EContact.EmergencyFirstName ?? (Object)DBNull.Value);
                    command.Parameters.AddWithValue("@eLast", client.EContact.EmergencyLastName ?? (Object)DBNull.Value);
                    command.Parameters.AddWithValue("@ephone", client.EContact.EmergencyPhoneNumber ?? (Object)DBNull.Value);
                    command.Parameters.AddWithValue("@eRelationship", client.EContact.EmergencyRelationship ?? (Object)DBNull.Value);
                    command.Parameters.AddWithValue("@aptNum", client.AptNum ?? (Object)DBNull.Value);
                    command.Parameters.AddWithValue("@poBox", client.PoBox ?? (Object)DBNull.Value);
                    command.Parameters.AddWithValue("@dateMet", Convert.ToDateTime(client.DateMet));

                    command.ExecuteNonQuery();


                    // Add new services back in if services were modified
                    if (client.Services.Count != 0 && client.shouldUpdateServices)
                    {
                        // get the rwcid
                        string getrwicd = "Select rwcid from client where firstName = @first  and  lastName = @last and dateOfBirth = @dob;";

                        MySqlCommand getRwcidCommand = new MySqlCommand(getrwicd, transactionConn);

                        getRwcidCommand.Parameters.AddWithValue("@first", client.FirstName ?? (Object)DBNull.Value);
                        getRwcidCommand.Parameters.AddWithValue("@last", client.LastName ?? (Object)DBNull.Value);
                        getRwcidCommand.Parameters.AddWithValue("@dob", Convert.ToDateTime(client.Dob));

                        idReader = getRwcidCommand.ExecuteReader();
                        idReader.Read();
                        rwcid = idReader.GetInt32(0);
                        idReader.Close();

                        // delete all the services associated with the client
                        string deleteServicesQuery = "Delete from clientServices where clientServices.rwcid = @rwcid;";

                        MySqlCommand deleteServicesCommand = new MySqlCommand(deleteServicesQuery, transactionConn);

                        deleteServicesCommand.Parameters.AddWithValue("@rwcid", rwcid);
                        deleteServicesCommand.ExecuteNonQuery();


                        // for each client services add it into the clientServices table
                        int serviceCount = client.Services.Count;
                        string insertServices = "INSERT INTO clientServices (rwcid,sid) VALUES ";

                        for (int count = 1; count <= serviceCount; count++)
                        {
                            if (count != serviceCount)
                                insertServices += "((" + rwcid + ") ,(SELECT sid FROM services WHERE serviceName=@serviceName" + count.ToString() + ")),";
                            else
                            {
                                insertServices += "((" + rwcid + ") ,(SELECT sid FROM services WHERE serviceName=@serviceName" + count.ToString() + "))";
                            }
                        }
                        command.Parameters.Clear();
                        command.CommandText = insertServices;

                        int serviceNum = 1;
                        foreach (string service in client.Services)
                        {
                            command.Parameters.AddWithValue("@serviceName" + serviceNum.ToString(), service);
                            serviceNum++;
                        }

                        // execute command
                        command.ExecuteNonQuery();
                    }
                }
                transaction.Commit();
                transactionConn.Close();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                Debug.WriteLine(e.Message);
                transactionConn.Close();
            };
        }

        /// <summary>
        /// Creates a report using a list of clients that were searched for.
        /// </summary>
        /// <param name="clientList"> List of clients displayed in the View Info page. </param>
        /// <param name="currUser"> The account currently logged in and creating a report. </param>
        public static void generateReport(ObservableCollection<Client> clientList, Staff currUser)
        {
            validateAccount();

            Excel.Application xlApp = new Excel.Application();
            object misValue = System.Reflection.Missing.Value;
            Workbook workbook = xlApp.Workbooks.Add(misValue);
            Worksheet worksheet = (Worksheet)workbook.Sheets[1];
           
            // Initializing Column Names
            worksheet.Cells[1, 1] = "Date Met";
            worksheet.Cells[1, 2] = "Last Name";
            worksheet.Cells[1, 3] = "First Name";
            worksheet.Cells[1, 4] = "M.I.";
            worksheet.Cells[1, 5] = "Date of Birth";
            worksheet.Cells[1, 6] = "Address";
            worksheet.Cells[1, 7] = "P.O. Box";
            worksheet.Cells[1, 8] = "Apt. Num.";
            worksheet.Cells[1, 9] = "City";
            worksheet.Cells[1, 10] = "State";
            worksheet.Cells[1, 11] = "County";
            worksheet.Cells[1, 12] = "ZipCode";
            worksheet.Cells[1, 13] = "Phone Number";
            worksheet.Cells[1, 14] = "Income";
            worksheet.Cells[1, 15] = "Marital Status";
            worksheet.Cells[1, 16] = "Num. Household Members";
            worksheet.Cells[1, 17] = "Medicare Status";
            worksheet.Cells[1, 18] = "Medicaid Status";
            worksheet.Cells[1, 19] = "Social Worker Status";
            worksheet.Cells[1, 20] = "Notes";
            //worksheet.Cells[1, 21] = "Emergency Contact: Name";
            //worksheet.Cells[1, 22] = "Emergency Contact: Relationship";
            //worksheet.Cells[1, 23] = "Emergency Contact: Phone Number";
            worksheet.Cells[1, 21] = "Services";

            int i = 2;
            string endRange;

            // Populates Excel Sheet with every client's information
            // Clients are those that currently exist in clientList
            // i.e. those that are currently displayed in the grid view
            foreach (Client client in clientList)
            {
                worksheet.Cells[i, 1] = client.DateMet;
                worksheet.Cells[i, 2] = client.LastName;
                worksheet.Cells[i, 3] = client.FirstName;
                worksheet.Cells[i, 4] = client.MiddleName;
                worksheet.Cells[i, 5] = client.Dob;
                worksheet.Cells[i, 6] = client.Address;
                worksheet.Cells[i, 7] = client.PoBox;
                worksheet.Cells[i, 8] = client.AptNum;
                worksheet.Cells[i, 9] = client.City;
                worksheet.Cells[i, 10] = client.State;
                worksheet.Cells[i, 11] = client.County;
                worksheet.Cells[i, 12] = client.Zipcode;
                worksheet.Cells[i, 13] = client.PhoneNumber;
                worksheet.Cells[i, 14] = client.Income;
                worksheet.Cells[i, 15] = client.MaritalStatus;
                worksheet.Cells[i, 16] = client.HouseholdMembers;
                worksheet.Cells[i, 17] = client.MedicareStatus;
                worksheet.Cells[i, 18] = client.MedicaidStatus;
                worksheet.Cells[i, 19] = client.SocialWorkStatus;
                worksheet.Cells[i, 20] = client.Notes;
                //worksheet.Cells[i, 21] = client.EContact.EmergencyFullName;
                //worksheet.Cells[i, 22] = client.EContact.EmergencyRelationship;
                //worksheet.Cells[i, 23] = client.EContact.EmergencyPhoneNumber;
                string services = "";
                for (int idx = 0; idx < client.Services.Count; idx++)
                {
                    if (idx == client.Services.Count - 1) services += client.Services[idx];
                    else services += client.Services[idx] + "\n";
                }
                worksheet.Cells[i, 21] = services;
                i++;
            }
            // Row (i) currently = n + 1, where n = # of rows, so to ensure table's range is correct, decrement i
            i--;

            // Report information: Employee it was generated by and 
            // the date it was generated
            worksheet.Cells[i+1, 1] = currUser.UserName;
            worksheet.Cells[i+1, 2] = DateTime.Today;
            worksheet.Cells[i+1, 3] = DateTime.Now.ToString("h:mm:ss tt");
            Excel.Range range = worksheet.get_Range("A"+ i+1.ToString(), "C"+ i+1.ToString());


            // Formats the Excel Sheet as a table for improved readability
            endRange = "U" + i.ToString();
            Excel.Range SourceRange = worksheet.get_Range("A1", endRange);
            SourceRange.Worksheet.ListObjects.Add(XlListObjectSourceType.xlSrcRange, SourceRange, misValue, XlYesNoGuess.xlYes, misValue).Name = "Client Report";
            SourceRange.Select();
            SourceRange.Worksheet.ListObjects["Client Report"].TableStyle = "TableStyleMedium8";
            
            // Make text in cells wrap around for improved readability
            SourceRange.WrapText = true;

            // Adjust Services col width for improved visual
            Excel.Range widthRange = worksheet.get_Range("U1", endRange);
            widthRange.Columns.ColumnWidth = 20;

            // Adjust Address col width
            widthRange = worksheet.get_Range("F1", "F"+i.ToString());
            widthRange.Columns.ColumnWidth = 15;

            // Adjust city col width
            widthRange = worksheet.get_Range("I1", "I"+i.ToString());
            widthRange.Columns.ColumnWidth = 10;

            //Adjust County col width
            widthRange = worksheet.get_Range("K1","K"+i.ToString());
            widthRange.Columns.ColumnWidth = 10;

            //Make excel sheet read only to prevent changes made to clients in the report
            //if a client needs to be edited, it should be done in the DB
            SourceRange.Locked = true;
            worksheet.Protect("teamone2023");

            // Show the Save dialog
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Excel Workbook (*.xlsx)|*.xlsx";
            if (saveDialog.ShowDialog() != true)
            {
                // User canceled the dialog
                return;
            }
            workbook.SaveAs(saveDialog.FileName, Excel.XlFileFormat.xlOpenXMLWorkbook, misValue, misValue,
                                    false, false, Excel.XlSaveAsAccessMode.xlNoChange,
                                    Excel.XlSaveConflictResolution.xlUserResolution, true, misValue, misValue, misValue);

            workbook.Close(true, misValue, misValue);
            xlApp.Quit();

            // close excel process
            System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(xlApp);
            xlApp = null;
        }

        /***************************************************
        *
        *
        *                  SERVICES FUNCTIONS
        *
        *
        * **************************************************/

        /// <summary>
        /// Adds a new service to the "services" table in the databse. If the addition fails,
        /// the transaction is rolled back to a previous state and closed. 
        /// </summary>
        /// <param name="service"> The name of the service to be added. </param>
        public static void addService(Services service)
        {
            validateAccount();

            // new transaction because isInDatabase() would use the same connection and cause conflict
            MySqlConnection transactionConn = DatabaseConnector.GetConnection();

            // open sql connection
            transactionConn.Open();

            // Start a transaction so all clients or no clients are added
            MySqlTransaction transaction = transactionConn.BeginTransaction();

            string insertStatement = "INSERT INTO services VALUES (NULL, @serviceName)";

            MySqlCommand idfetch = new MySqlCommand(insertStatement, transactionConn, transaction);
            MySqlCommand command = new MySqlCommand("", transactionConn, transaction);
            MySqlCommand serviceCommand = new MySqlCommand("", transactionConn, transaction);

            string formattedService = service.serviceName;
            if (service.serviceName != null)
            {
                formattedService = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(service.serviceName.Trim());
            }
            try
            {
                command.Parameters.Clear();
                command.CommandText = insertStatement;
                // Set parameterized values.
                // If value is null replace with DBNull to database recognizes it as a null value
                command.Parameters.AddWithValue("@serviceName", formattedService ?? (Object)DBNull.Value);
                command.ExecuteNonQuery();
                transaction.Commit();
                transactionConn.Close();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                Debug.WriteLine(e.Message);
                transactionConn.Close();
            }
        }

        /// <summary>
        /// Permanently deletes a service from the "services" table in the database.
        /// </summary>
        /// <param name="service"> The name of the service to be deleted. </param>
        public static void deleteService(Services service)
        {
            validateAccount();

            // new transaction because isInDatabase() would use the same connection and cause conflict
            MySqlConnection transactionConn = DatabaseConnector.GetConnection();

            // open sql connection
            transactionConn.Open();

            // Remove the service from the DB if it exists
            MySqlTransaction transaction = transactionConn.BeginTransaction();
            String deleteStatement = "DELETE FROM services WHERE serviceName = @serviceName;";

            MySqlCommand command = new MySqlCommand(deleteStatement, transactionConn);
            command.CommandText = deleteStatement;
            command.Parameters.AddWithValue("@serviceName", service.serviceName ?? (Object)DBNull.Value);
            command.ExecuteNonQuery();
            transaction.Commit();
            transactionConn.Close();
        }

        //private static List<string> getClientServices(Client client)
        //{
        //    //string clientServiceQuery = "SELECT serviceName FROM clientServices JOIN services ON clientServices.sid = services.sid WHERE clientServices.rwcid = " + client.rwcid;
        //    string clientServiceQuery = "SELECT sid FROM clientServices WHERE clientServices.rwcid = " + client.rwcid;

        //    List<string> clientServices = new List<string>();

        //    // new transaction because isInDatabase() would use the same connection and cause conflict
        //    //MySqlConnection clientServicesConn = DatabaseConnector.GetConnection();
        //    using (MySqlConnection clientServicesConn = DatabaseConnector.GetConnection())
        //    {
        //        MySqlDataReader serviceReader;

        //        // open sql connection
        //        clientServicesConn.Open();

        //        MySqlCommand command = new MySqlCommand(clientServiceQuery, clientServicesConn);
        //        serviceReader = command.ExecuteReader();

        //        while (serviceReader.Read())
        //        {
        //            //clientServices.Add(serviceReader.GetString(0));
        //        }

        //        serviceReader.Close();
        //        clientServicesConn.Close();
        //        return clientServices;
        //    }
        //}

        /// <summary>
        /// Fetches the services belonging to an individual client and
        /// is displayed in a client's information summary.
        /// </summary>
        /// <returns> A list of services. </returns>
        public static List<String> getAllServices()
        {

                MySqlConnection connection = DatabaseConnector.GetConnection();

                MySqlDataReader queryReader;
                List<String> services = new List<String>();
                String serviceQuery = "SELECT serviceName FROM services";
                MySqlCommand command = new MySqlCommand(serviceQuery, connection);
                connection.Open();
                queryReader = command.ExecuteReader();
                while (queryReader.Read())
                {
                    services.Add(queryReader.GetString(0));
                }
                queryReader.Close();
                connection.Close();

                return services;
            }

        /// <summary>
        /// Fetches all services from the "services" table in the database. This
        /// is used to populate the data grid in the Manage Services page.
        /// </summary>
        /// <param name="query"> SQL query to search for all values in the "services" table. </param>
        /// <returns></returns>
        public static ObservableCollection<Services> getServices(string query)
        {

            MySqlDataReader queryReader;
            MySqlConnection connection = DatabaseConnector.GetConnection();

            ObservableCollection<Services> serviceList = new ObservableCollection<Services>();
            MySqlCommand commandDatabase = new MySqlCommand(query, connection);
            connection.Open();
            queryReader = commandDatabase.ExecuteReader();
            while (queryReader.Read())
            {
                Services service = new Services();
                //service.sID = (int)queryReader.GetValue(sIDCol);
                service.serviceName = queryReader.GetValue(serviceNameCol).ToString();
                serviceList.Add(service);
            }
            connection.Close();
            return serviceList;
        }

        /***************************************************
         *
         *
         *                  STAFF FUNCTIONS
         *
         *
         * **************************************************/
        /// <summary>
        /// Fetches all staff accounts from the database to display in the
        /// Manage Accounts page.
        /// </summary>
        /// <param name="query"> SQL query to search for all values in the "users" table </param>
        /// <returns> A list of all accounts in the database. </returns>
        public static ObservableCollection<Staff> getStaffAccounts(string query)
        {

            MySqlDataReader queryReader;
            MySqlConnection connection = DatabaseConnector.GetConnection();

            ObservableCollection<Staff> staffAccounts = new ObservableCollection<Staff>();
            MySqlCommand commandDatabase = new MySqlCommand(query, connection);
            connection.Open();
            queryReader = commandDatabase.ExecuteReader();
            while (queryReader.Read())
            {
                Staff account = new Staff();
                account.UserName = queryReader.GetValue(usernameCol).ToString();
                account.FirstName = queryReader.GetValue(staffFirstNameCol).ToString();
                account.LastName = queryReader.GetValue(staffLastNameCol).ToString();
                //account.admin = queryReader.GetValue(adminCol).ToString();
                staffAccounts.Add(account);
            }
            connection.Close();
            return staffAccounts;
        }

        /// <summary>
        /// Permanently deletes an account from the "users" table in the database.
        /// </summary>
        /// <param name="Username"> The username of the account to be deleted. </param>
        public static void deleteStaffAccount(String Username)
        {
            validateAccount();

            // new transaction because isInDatabase() would use the same connection and cause conflict
            MySqlConnection transactionConn = DatabaseConnector.GetConnection();

            // open sql connection
            transactionConn.Open();
            MySqlTransaction transaction = transactionConn.BeginTransaction();
            String deleteAccountStatement = "DELETE FROM `users` WHERE username = @username;";

            MySqlCommand command = new MySqlCommand(deleteAccountStatement, transactionConn);
            command.CommandText = deleteAccountStatement;
            command.Parameters.AddWithValue("@username", Username ?? (Object)DBNull.Value);
            command.ExecuteNonQuery();
            transaction.Commit();
            transactionConn.Close();
        }

        /// <summary>
        /// Adds a new staff account to the "users" table in the database. If
        /// the addition fails, the transaction is rolled back to its previous
        /// state and closed. Only administrators can use this function.
        /// </summary>
        /// <param name="newAccount"> The account to be added to the database. </param>
        public static void addStaffAccount(Staff newAccount)
        {
            validateAccount();

            // new transaction because isInDatabase() would use the same connection and cause conflict
            MySqlConnection transactionConn = DatabaseConnector.GetConnection();

            // open sql connection
            transactionConn.Open();

            // Start a transaction so all clients or no clients are added
            MySqlTransaction transaction = transactionConn.BeginTransaction();
            string insertStatement = "INSERT INTO `users`(`username`, `firstName`, `lastName`, `admin`, `hash`) VALUES (@username,@firstname,@lastname,@admin, @Password)";
            MySqlCommand command = new MySqlCommand("", transactionConn, transaction);
            string hashedPW = Security.HashPassword(newAccount.Password);

            try
            {
                command.Parameters.Clear();
                command.CommandText = insertStatement;

                // Set parameterized values.
                // If value is null replace with DBNull to database recognizes it as a null value
                command.Parameters.AddWithValue("@username", newAccount.UserName.Trim() ?? (Object)DBNull.Value);
                command.Parameters.AddWithValue("@firstname", newAccount.FirstName.Trim() ?? (Object)DBNull.Value);
                command.Parameters.AddWithValue("@lastname", newAccount.LastName.Trim() ?? (Object)DBNull.Value);
                command.Parameters.AddWithValue("@admin", newAccount.Admin ?? (Object)DBNull.Value);
                command.Parameters.AddWithValue("@Password", hashedPW ?? (Object)DBNull.Value);
                command.ExecuteNonQuery();
                transaction.Commit();
                transactionConn.Close();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                Debug.WriteLine(e.Message);
                transactionConn.Close();
            }
        }
        /// <summary>
        /// Verifies that the credentials submitted in the login page
        /// match with a username and password in the database. If true, 
        /// the user proceeds to the Home page. If false, an error message
        /// is displayed.
        /// </summary>
        /// <param name="Username"> The username submitted by a user. </param>
        /// <param name="Password"> The password submitted by a user. </param>
        /// <returns> Boolean either allowing or denying access to the rest of the app. </returns>
        public static bool staffLogin(String Username, String Password)
        {

            string storedPW;
            MySqlDataReader queryReader;
            MySqlConnection connection = DatabaseConnector.GetConnection();

            // search by username and retrieve the stored hash
            string searchQuery = "Select `hash` FROM users WHERE username = @username";
            MySqlCommand command = new MySqlCommand(searchQuery, connection);
            command.Parameters.AddWithValue("@username", Username);
            connection.Open();
            queryReader = command.ExecuteReader();

            if (queryReader.Read())
            {
                storedPW = queryReader.GetValue(0).ToString();
                queryReader.Close();
                connection.Close();
                return Security.VerifyPassword(Password, storedPW);
            }
            else return false;
        }

        /// <summary>
        /// Gets the administrator status for the account that logged in.
        /// This is used to determine what permissions the account can use
        /// and  what pages the account
        /// can view. 
        /// </summary>
        /// <param name="loginAccount"> The account that was granted access to use the app. </param>
        public static void getAdminStatus(Staff loginAccount)
        {
            MySqlDataReader queryReader;
            MySqlConnection connection = DatabaseConnector.GetConnection();

            // search by username and get admin status
            string searchQuery = "Select IF(users.admin = 1, 1, 0) FROM `users` WHERE username = @username";
            MySqlCommand command = new MySqlCommand(searchQuery, connection);
            command.Parameters.AddWithValue("@username", loginAccount.UserName);
            // hold result
            int accountCount = 0;
            connection.Open();
            queryReader = command.ExecuteReader();

            // get query result
            if (queryReader.Read())
            {
                accountCount = queryReader.GetInt32(0);

                // if result >= 1 then the password matches and login is verified
                if (accountCount >= 1) loginAccount.Admin = true;
                else loginAccount.Admin = false;
            }
            else loginAccount.Admin = false;

            queryReader.Close();
            connection.Close();
        }

        /// <summary>
        /// Resets a selected account's password to another randomly generated
        /// unique key. Only administrators can use this function. 
        /// </summary>
        /// <param name="account"> The selected account whose password is to be reset. </param>
        public static void resetPassword(Staff account)
        {
            validateAccount();

            // new transaction because isInDatabase() would use the same connection and cause conflict
            MySqlConnection transactionConn = DatabaseConnector.GetConnection();

            // open sql connection
            transactionConn.Open();
            MySqlTransaction transaction = transactionConn.BeginTransaction();

            string updateStatement = "UPDATE users SET hash = @newPassword WHERE username = @username";

            MySqlCommand command = new MySqlCommand(updateStatement, transactionConn);
            command.Transaction = transaction;

            command.Parameters.Clear();
            command.CommandText = updateStatement;

            string newPassword = Security.HashPassword(account.Password);
            command.Parameters.AddWithValue("@newPassword", newPassword ?? (Object)DBNull.Value);
            command.Parameters.AddWithValue("@username", account.UserName ?? (Object)DBNull.Value);
            command.ExecuteNonQuery();

            transaction.Commit();
            transactionConn.Close();
        }

        /***************************************************
         *
         *
         *              DATABASE CHECKING FUNCTIONS
         *
         *
         * **************************************************/

        /// <summary>
        /// Search database for specified client. The client's firstname,lastname,and dob are used to query the database.
        /// </summary>
        /// <param name="client">Client to be searched for.</param>
        /// <returns>TRUE if client exist and FALSE if not.</returns>
        public static bool isInDatabase(Client client)
        {
            MySqlDataReader queryReader;
            MySqlConnection connection = DatabaseConnector.GetConnection();

            // search by primary key firstname,lastname,dob
            string searchQuery = "SELECT COUNT(rwcid) FROM client WHERE firstName=@first AND lastName=@last AND dateOfBirth=@dob";
            MySqlCommand command = new MySqlCommand(searchQuery, connection);
            command.Parameters.AddWithValue("@first", client.FirstName);
            command.Parameters.AddWithValue("@last", client.LastName);
            command.Parameters.AddWithValue("@dob", Convert.ToDateTime(client.Dob));
            // hold result
            int clientCount = 0;
            connection.Open();
            queryReader = command.ExecuteReader();

            // get query result there's only one so no need to loop through
            queryReader.Read();

            // set result
            clientCount = queryReader.GetInt32(0);
            queryReader.Close();
            connection.Close();

            // if result >= 1 then that client exist else the client does not exist
            if (clientCount >= 1)
                return true;
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Overload of the above function to work with services instead of clients.
        /// </summary>
        /// <param name="service"> The service to be checked against the database</param>
        /// <returns> True if service exists, false otherwise. </returns>
        public static bool isInDatabase(Services service)
        {
            MySqlDataReader queryReader;
            MySqlConnection connection = DatabaseConnector.GetConnection();
            // search by primary key
            String searchQuery = "SELECT COUNT(sid) FROM services WHERE serviceName='" + service.serviceName + "'";
            MySqlCommand command = new MySqlCommand(searchQuery, connection);

            // hold result
            int serviceCount = 0;
            connection.Open();
            queryReader = command.ExecuteReader();

            // get query result there's only one so no need to loop through
            queryReader.Read();

            // set result
            serviceCount = queryReader.GetInt32(0);
            queryReader.Close();
            connection.Close();

            // if result >= 1 then that client exist else the client does not exist
            if (serviceCount >= 1)
                return true;
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks to see if an account exists in the database. Accounts are searched
        /// by username. 
        /// </summary>
        /// <param name="account"> The account to be searched for. </param>
        /// <returns> True if the account exiss, false otherwise. </returns>
        public static bool isEmployee(string accountName)
        {
            MySqlDataReader queryReader;
            MySqlConnection connection = DatabaseConnector.GetConnection();
            // search by primary key
            String searchQuery = "SELECT COUNT(username) FROM users WHERE username='" + accountName + "'";
            MySqlCommand command = new MySqlCommand(searchQuery, connection);

            // hold result
            int serviceCount = 0;
            connection.Open();
            queryReader = command.ExecuteReader();

            // get query result there's only one so no need to loop through
            if (queryReader.Read())
            {
                // set result
                serviceCount = queryReader.GetInt32(0);
                queryReader.Close();
                connection.Close();

                // if result >= 1 then that client exist else the client does not exist
                if (serviceCount >= 1) return true;
                else return false;
            }
            else return false;
        }

        /// <summary>
        /// checks if currently active account is in the database if not then forcefully exit the application.
        /// </summary>
        static void validateAccount()
        {
            if (!isEmployee(activeAccount))
                Environment.Exit(0);
        }

        public static bool isValidConnection()
        {
            MySqlConnection connection = DatabaseConnector.GetConnection();
            try
            {
                connection.Open();
                connection.Close();
                return true;
            }
            catch (MySqlException e)
            {
                return false;
            }
        }
    }
}