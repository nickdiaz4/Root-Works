using System;
using System.Collections.Generic;
using System.Text;

namespace Catawba.Database
{
    /// <summary>
    /// The max character length for the columns in the client table in the database.
    /// Used to limit the length of characters a user can either manually enter or import via Excel 
    /// spreadsheet.
    /// </summary>
    public static class DatabaseConstants
    {
        // Max character limits for database columns
        public static int FIRSTNAME_MAX = 50;
        public static int LASTNAME_MAX = 50;
        public static int MIDDLE_INITIAL_MAX = 1;
        public static int ADDRESS_MAX = 50;
        public static int CITY_MAX = 50;
        public static int COUNTY_MAX = 50;
        public static int ZIPCODE_MAX = 5;
        public static int PHONE_MAX = 10;
        public static int INCOME_MAX = 8388607; // income is mediumInt(9) this represents that max integer value
        public static int MARITAL_STATUS_MAX = 9;
        public static int APT_NUMBER_MAX = 8;
        public static int PO_BOX_MAX = 10;
        public static int NOTES_MAX = 500;
        public static int SERVICE_NAME_MAX = 50;
        public static int USERNAME_MAX = 10;
        public static int RELATIONSHIP_MAX = 50;
        public static int HOUSEHOLD_MAX = 127;
    }
}
