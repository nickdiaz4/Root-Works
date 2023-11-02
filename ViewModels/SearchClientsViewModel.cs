using Catawba.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using MySql.Data.MySqlClient;
using Catawba.Database;
using Catawba.Commands.ClientCommands;

namespace Catawba.ViewModels
{
    public class SearchClientsViewModel : ViewModelBase
    {
        public NavigateCommand goBack { get; set; }
        public SearchClientsCommand searchClientsCommand { get; set; }
        public ClearSearchParametersCommand clearParameters { get; }
        public ViewModelHandler viewHandler { get; set; }
        public Client client { get; set; }
        public ViewInfoViewModel clientViewPage { get; set; }
        public static ObservableCollection<SearchParameter> listOfSearchParameters { get; set; }

        // combobox options with their database data type 
        public static Dictionary<string, string> parameterTypePair = new Dictionary<string, string>()
        {
            {"Date", "string" },
            { "First Name", "string" },
            { "Last Name", "string" },
            { "Date of Birth", "string" },
            { "Address", "string" },
            {"P.O. Box", "string" },
            {"Apt. Num.", "string" },
            { "State", "string" },
            { "City","string" },
            { "County", "string" },
            { "Zipcode", "string" },
            { "Phone Number", "string" },
            { "Income", "int" },
            { "HouseHold Members", "sbyte" },
            { "Marital Status", "string" },
            { "Medicaid Status", "bool" },
            { "Social Worker Status", "bool"},
            { "Services", "service"}
        };

        // combobox options with their column name for building the sql select command
        public static Dictionary<string, string> parameterColumnNamePair = new Dictionary<string, string>()
        {
            {"Date", "client.addedDate" },
            { "First Name", "client.firstName" },
            { "Last Name", "client.lastName" },
            { "Date of Birth", "client.dateOfBirth" },
            { "Address", "client.address" },
            {"P.O. Box", "client.poBoxNumber" },
            {"Apt. Num.", "client.aptNumber" },
            { "State", "client.state" },
            { "City","client.city" },
            { "County", "client.county" },
            { "Zipcode", "client.zipCode" },
            { "Phone Number", "client.phoneNumber" },
            { "Income", "client.income" },
            { "HouseHold Members", "client.numberOfHouseholdMembers" },
            { "Marital Status", "client.maritalStatus" },
            { "Medicaid Status", "client.medicaidStatus" },
            { "Social Worker Status", "client.socialWorkerStatus"},
            { "Services", "clientServices"}
        };

        public SearchClientsViewModel(ViewModelHandler viewHandler, ViewInfoViewModel clientViewPage)
        {
            this.clientViewPage = clientViewPage;
            searchClientsCommand = new SearchClientsCommand(this,false);
            clearParameters = new ClearSearchParametersCommand(this);
            listOfSearchParameters = new ObservableCollection<SearchParameter>() { new SearchParameter()};
            goBack = new NavigateCommand(viewHandler, clientViewPage);
        }

        /// <summary>
        /// Automatically add an option to include another search parameter
        /// </summary>
        public static void addSearchParameter()
        {
            listOfSearchParameters.Add(new SearchParameter());
        }
        /// <summary>
        /// Removes a search parameter when the "Remove" option is chosen
        /// in the drop down list. 
        /// </summary>
        /// <param name="param"></param>
        public static void removeSearchParameter(SearchParameter param)
        {
            listOfSearchParameters.Remove(param);
        }

        /// <summary>
        /// Class that holds information for a search parameter.
        /// </summary>
        public class SearchParameter : ViewModelBase
        {
            public string operation { get; set; }
            public string itemDataType
            {
                get => _searchItem != null ? parameterTypePair[_searchItem] : null;
            }

            private string _searchItem;
            public string searchItem
            {
                get => _searchItem;
                set
                {

                    if (_searchItem == null)
                    {
                        addSearchParameter();
                    }

                    _searchItem = value;
                    // if user clicks empty spot in combo box remove search parameter
                    if (value.Equals(""))
                    {
                        removeSearchParameter(this);
                    }
                    else
                    {
                        // set default value for bools to active
                        if (itemDataType.Equals("bool"))
                        {
                            searchValue = "True";
                        }
                        // set default values for service to the first item
                        else if (itemDataType.Equals("service"))
                        {
                            searchValue = serviceList[0];
                            operation = "contains";
                        }
                        // set default for string and numbers
                        else
                        {
                            searchValue = "";
                            operation = "eq";
                        }

                        OnPropertyChanged(nameof(operation));
                        OnPropertyChanged(nameof(searchValue));
                        OnPropertyChanged(nameof(itemDataType));
                    }
                }
            } 
            public string searchValue { get; set; }
            public ObservableCollection<String> parameterList { get; set; }
            public ObservableCollection<String> serviceList { get; set; }
            public SearchParameter()
            {
                parameterList = defaultParameters();
                serviceList = new ObservableCollection<string>(DatabaseStore.getAllServices());
            }
        }

        /// <summary>
        /// Builds a select command based on given search parameters.
        /// </summary>
        /// <returns>Complete sql select command for given search parameters.</returns>
        public MySqlCommand buildSearchQuery()
        {
            // initial query to ensure all data points are collected 
            string searchQueryStart = "SELECT client.*,GROUP_CONCAT(DISTINCT services.serviceName SEPARATOR ',') AS clientServices " +
                                      "FROM client LEFT JOIN clientServices ON client.rwcid = clientServices.rwcid LEFT JOIN services ON clientServices.sid = services.sid ";
            string whereClause = "";
            string havingClause = "";
            string searchQueryEnd = " GROUP BY client.firstName, client.lastName,client.dateOfBirth";
            MySqlCommand searchCommand = new MySqlCommand();
            Dictionary<string, string[]> paramaterizedValues = new Dictionary<string, string []>();
            Dictionary<string, List<SearchParameter>> groupedParameters = new Dictionary<string, List<SearchParameter>>();

            // go through all search parameters and group identical ones
            for (int nextParam = 0; nextParam < listOfSearchParameters.Count; nextParam++)
            {
                // break on the last parameter because it is always empty
                if (nextParam == listOfSearchParameters.Count - 1)
                    break;

                SearchParameter parameter = listOfSearchParameters[nextParam];
                string dbColumnName = parameterColumnNamePair[parameter.searchItem];

                // add identical search option to the dictionary under dbcolumn name as key
                if (!groupedParameters.TryAdd(dbColumnName, new List<SearchParameter>(new SearchParameter[] { parameter })))
                {
                    groupedParameters[dbColumnName].Add(parameter);
                }
            }

            // go through all parameters to prepare parameterized values with the proper sql operation
            for (int uniqueParam = 0; uniqueParam < groupedParameters.Count; uniqueParam++)
            {

                // set current parameter group
                KeyValuePair<string, List<SearchParameter>> paramGroup = groupedParameters.ElementAt(uniqueParam);

                bool useHavingClause;

                // skip services since this is for the search WHERE clause and services requires a HAVING clause
                if (paramGroup.Key.Equals("clientServices"))
                {
                    // check if this is the first parameter for HAVING clause
                    if (havingClause.Equals(""))
                    {
                        havingClause += " HAVING ";
                    }

                    useHavingClause = true;
                }
                else
                {
                    // check if this is the first parameter for WHERE clause
                    if (whereClause.Equals(""))
                    {
                        whereClause += " WHERE ";
                    }

                    // open parentheses for sql statement to group values of identical column name
                    whereClause += " (";
                    useHavingClause = false;
                }
                
                // go through each parameter in the grouped list
                for (int nextParam = 0; nextParam < paramGroup.Value.Count; nextParam++)
                {
                    SearchParameter param = paramGroup.Value[nextParam];
                    string paramDataType = param.itemDataType;
                    string paramValue = param.searchValue.Trim();
                    string dbColumnName = parameterColumnNamePair[param.searchItem];
                    string paramaterizedName = "@" + dbColumnName + nextParam.ToString();
                    string operation = param.operation;

                    // check if the search parameter requires a HAVING clause
                    if (useHavingClause)
                    {
                        if (nextParam > 0) havingClause += " AND ";
                        havingClause += dbColumnName;
                    }
                    else
                    {
                        if (nextParam > 0)
                        {
                            if (param.itemDataType == "int" || param.itemDataType == "sbyte")
                            {
                                whereClause += " AND ";
                            }
                            else
                            {
                                whereClause += " OR ";
                            }
                        }

                        whereClause += dbColumnName;
                    }

                    // check data type for appropriate sql operations. Depends on what the user selected 
                    switch (paramDataType)
                    {
                        case "service":
                            if (operation.Equals("contains"))
                            {
                                havingClause += " LIKE " + paramaterizedName;
                                paramValue = "%" + paramValue + "%";

                            }
                            else if (operation.Equals("!contains"))
                            {
                                havingClause += " NOT LIKE " + paramaterizedName;
                                paramValue = "%" + paramValue + "%";
                            }
                            break;
                        case "string":
                            if (operation.Equals("eq"))
                            {
                                whereClause += " =" + paramaterizedName;
                            }
                            else if (operation.Equals("neq"))
                            {
                                whereClause += " <>" + paramaterizedName;
                            }
                            else if (operation.Equals("contains"))
                            {
                                whereClause += " LIKE " + paramaterizedName;
                                paramValue = "%" + paramValue + "%";

                            }
                            else if (operation.Equals("!contains"))
                            {
                                whereClause += " NOT LIKE " + paramaterizedName;
                                paramValue = "%" + paramValue + "%";
                            }
                            break;
                        case "int":
                        case "sbyte":
                            if (operation.Equals("eq"))
                            {
                                whereClause += " = " + paramaterizedName;
                            }
                            else if (operation.Equals("neq"))
                            {
                                whereClause += " <> " + paramaterizedName;
                            }
                            else if (operation.Equals("gt"))
                            {
                                whereClause += " > " + paramaterizedName;
                            }
                            else if (operation.Equals("gteq"))
                            {
                                whereClause += " >= " + paramaterizedName;
                            }
                            else if (operation.Equals("lt"))
                            {
                                whereClause += " < " + paramaterizedName;
                            }
                            else if (operation.Equals("lteq"))
                            {
                                whereClause += " <= " + paramaterizedName;
                            }
                            break;
                        case "bool":
                            whereClause += " = " + paramaterizedName;
                            break;
                        default:
                            break;
                    }
                    paramaterizedValues.Add(paramaterizedName, new string[] { paramDataType, paramValue });
                }

                if (!useHavingClause)
                {
                    // close off group
                    whereClause += ") ";
                }

                // if there are more grouped parameters and the next one is not clientServices insert an AND operator
                if (uniqueParam != groupedParameters.Count - 1 && !groupedParameters.ElementAt(uniqueParam + 1).Key.Equals("clientServices"))
                {
                    whereClause += " AND ";
                }
            }

            // complete the command text so it is ready to have parameterized values filled in
            searchCommand.CommandText = searchQueryStart + whereClause + searchQueryEnd + havingClause;

            // add paramterized values to the command
            foreach (KeyValuePair<string, string[]> entry in paramaterizedValues)
            {
                string valueDataType = entry.Value[0];
                object value = null;

                // check data type for proper conversion inot db
                switch (valueDataType)
                {
                    case "int":
                        int defaultNum;
                        value = int.TryParse(entry.Value[1], out defaultNum);
                        if ((bool)value)
                            value = defaultNum;
                        break;
                    case "sbyte":
                        sbyte defaultSbyte;
                        value = sbyte.TryParse(entry.Value[1], out defaultSbyte);
                        if ((bool)value)
                            value = defaultSbyte;
                        break;
                    case "bool":
                        value = (bool)bool.Parse(entry.Value[1]);
                        break;
                    default:
                        value = (string)entry.Value[1];
                        break;
                }
                searchCommand.Parameters.AddWithValue(entry.Key, value);
            }
            return searchCommand;
        }

        /// <summary>
        /// Statically creates a list of options for the user to add as search parameters.
        /// </summary>
        /// <returns></returns>
        public static ObservableCollection<string> defaultParameters()
        {
            ObservableCollection<string> parameters = new ObservableCollection<string>();

            parameters.Add("");
            foreach (KeyValuePair<string, string> entry in parameterTypePair)
            {
                parameters.Add(entry.Key);
            }

            return parameters;
        }   

        /// <summary>
        /// Executes the built sql search command or the premade query to show all clients in the database. After execution it
        /// sets the clientlist of the datagrid to the results of the query.
        /// </summary>
        /// <param name="showAll">Flag to show all clients in database.</param>
        public void searchClients(bool showAll)
        {
            // only search if there is atleast 1 search parameter specified
            if (listOfSearchParameters.Count > 1 && !showAll)
            {
                MySqlCommand searchQuery = buildSearchQuery();
                clientViewPage.refreshQueryCommand = searchQuery;
                goBack.Execute(this);
                clientViewPage.setClientList(searchQuery);
            }
            else
            {
                string showAllQuery = "SELECT client.*,GROUP_CONCAT(DISTINCT services.serviceName SEPARATOR ',') AS clientServices " +
                    "FROM client LEFT JOIN clientServices ON client.rwcid = clientServices.rwcid LEFT JOIN services ON clientServices.sid = services.sid " +
                    "GROUP BY client.firstName, client.lastName,client.dateOfBirth";

                MySqlCommand showAllCommand = new MySqlCommand(showAllQuery);
                clientViewPage.refreshQueryCommand = showAllCommand;
                goBack.Execute(this);
                clientViewPage.setClientList(showAllCommand);
            }
        }

        /// <summary>
        /// Clears all parameters.
        /// </summary>
        public void ClearSearchParameters()
        {
            listOfSearchParameters.Clear();
            listOfSearchParameters.Add(new SearchParameter());
        }
    }
}
