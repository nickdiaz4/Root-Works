using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using MySql.Data.MySqlClient;

namespace Catawba
{
    public class DatabaseConnector
    {
        private static string port = "";
        private static string dataSource = "";
        private static string username = "";
        private static string password = "";
        private static string dataBaseName = "";

        /// <summary>
        /// The connection string used to connect to the database housing CAAA's information.
        /// </summary>
        /// <returns></returns>
        public static MySqlConnection GetConnection()
        {
            String path = "datasource=" + dataSource + ";port=" + port + ";username=" + username + ";password=" + password + ";database=" + dataBaseName + ";";
            return new MySqlConnection(path);
        }
    }
}
