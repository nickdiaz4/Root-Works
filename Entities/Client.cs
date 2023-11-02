using Catawba.Database;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace Catawba
{
    /// <summary>
    /// Holds all information relevant to an individual client.
    /// </summary>
    [Serializable]
    public class Client
    {

        public string errorMessage { get; set; }
        public string DateMet { get; set; }
        public int rwcid { get; set; }

        // Properties will force database character limit constraints
        private string _firstName;
        public string FirstName
        {
            get => _firstName;
            set
            {
                if (value != null && value.Length > DatabaseConstants.FIRSTNAME_MAX)
                {
                    _firstName = value.Substring(0, DatabaseConstants.FIRSTNAME_MAX);
                }
                else
                {
                    _firstName = value;
                }
            }
        }

        private string _lastName;
        public string LastName
        {
            get => _lastName;
            set
            {
                if (value != null && value.Length > DatabaseConstants.LASTNAME_MAX)
                {
                    _lastName = value.Substring(0, DatabaseConstants.LASTNAME_MAX);
                }
                else
                {
                    _lastName = value;
                }
            }
        }

        private string _middleName;
        public string MiddleName
        {
            get => _middleName;
            set
            {
                if (value != null && value.Length > DatabaseConstants.MIDDLE_INITIAL_MAX)
                {
                    _middleName = value.Substring(0, DatabaseConstants.MIDDLE_INITIAL_MAX);
                }
                else
                {
                    _middleName = value;
                }
            }
        }

        private string _address;
        public string Address
        {
            get => _address;
            set
            {
                if (value != null && value.Length > DatabaseConstants.ADDRESS_MAX)
                {
                    _address = value.Substring(0, DatabaseConstants.ADDRESS_MAX);
                }
                else
                {
                    _address = value;
                }
            }
        }

        private string _poBox;
        public string PoBox
        {
            get => _poBox;
            set
            {
                if (value != null && value.Length > DatabaseConstants.PO_BOX_MAX)
                {
                    _poBox = value.Substring(0, DatabaseConstants.PO_BOX_MAX);
                }
                else
                {
                    _poBox = value;
                }
            }
        }

        private string _aptNum;
        public string AptNum
        {
            get => _aptNum;
            set
            {
                if (value != null && value.Length > DatabaseConstants.APT_NUMBER_MAX)
                {
                    _aptNum = value.Substring(0, DatabaseConstants.APT_NUMBER_MAX);
                }
                else
                {
                    _aptNum = value;
                }
            }
        }

        private string _county;
        public string County
        {
            get => _county;
            set
            {
                if (value != null && value.Length > DatabaseConstants.COUNTY_MAX)
                {
                    _county = value.Substring(0, DatabaseConstants.COUNTY_MAX);
                }
                else
                {
                    _county = value;
                }
            }
        }

        private string _phoneNumber;
        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                if (value != null && value.Length > DatabaseConstants.PHONE_MAX)
                {
                    _phoneNumber = value.Substring(0, DatabaseConstants.PHONE_MAX);
                }
                else
                {
                    _phoneNumber = value;
                }
            }
        }

        private string _zipcode;
        public string Zipcode
        {
            get => _zipcode;
            set
            {
                if (value != null && value.Length > DatabaseConstants.ZIPCODE_MAX)
                {
                    _zipcode = value.Substring(0, DatabaseConstants.ZIPCODE_MAX);
                }
                else
                {
                    _zipcode = value;
                }
            }
        }

        private string _city;
        public string City
        {
            get => _city;
            set
            {
                if (value != null && value.Length > DatabaseConstants.CITY_MAX)
                {
                    _city = value.Substring(0, DatabaseConstants.CITY_MAX);
                }
                else
                {
                    _city = value;
                }
            }
        }
        public string Dob { get; set; }
        private int? _Income;
        public int? Income
        {
            get => _Income;
            set
            {
                if (value != null)
                {
                    if (value >= 0 && value <= DatabaseConstants.INCOME_MAX)
                    {
                        _Income = value;
                    }
                    else
                    {
                        _Income = 0;
                    }
                }
                else
                { 
                    _Income = value;
                }
                
            }
        }
        private sbyte? _HouseholdMembers { get; set; }
        public sbyte? HouseholdMembers
        {
            get => _HouseholdMembers;
            set
            {
                if (value != null)
                {
                    if (value >= 0 && value <= DatabaseConstants.HOUSEHOLD_MAX)
                    {
                        _HouseholdMembers = value;
                    }
                    else
                    {
                        _HouseholdMembers = 0;
                    }
                }
                else
                { 
                    _HouseholdMembers = value;
                }
                
            }
        }
        public string MaritalStatus { get; set; }
        public bool? MedicareStatus { get; set; }
        public bool? MedicaidStatus { get; set; }
        public bool? SocialWorkStatus { get; set; }
        public bool? isDupe { get; set; }
        public bool shouldUpdateServices { get; set; }
        public string _Notes { get; set; }

        public string Notes
        {
            get => _Notes;
            set
            {
                if (value != null && value.Length > DatabaseConstants.NOTES_MAX)
                {
                    _Notes = value.Substring(0, DatabaseConstants.NOTES_MAX);
                }
                else
                {
                    _Notes = value;
                }
            }
        }

        private static readonly Dictionary<string, string> stateAbbrv = new Dictionary<string, string>()
        {
            {"alabama", "AL"}, {"alaska", "AK"}, {"arizona", "AZ"}, {"arkansas", "AR"},
            {"american samoa", "AS"}, {"california", "CA"}, {"colorado", "CO"}, {"connecticut", "CT" },
            {"delaware", "DE"}, {"district of columbia", "DC"}, {"florida","FL"}, {"georgia", "GA"},
            {"guam", "GU"}, {"hawaii", "HI"}, {"idaho","ID"},{"illinois", "IL"}, {"indiana", "IN"},
            {"iowa", "IA"}, {"kansas", "KS"}, {"kentucky", "KY"}, {"louisiana", "LA"},
            {"maine", "ME"}, {"maryland", "MD"}, {"massachusetts", "MA"}, {"michigan", "MI"},
            {"minnesota", "MN"}, {"mississippi", "MS"}, {"missouri", "MO"}, {"montana", "MT"},
            {"nebraska", "NE"}, {"nevada", "NV"}, {"new hampshire", "NH"}, {"new jersey", "NJ"},
            {"new mexico", "NM"}, {"new york", "NY"}, {"north carolina", "NC"}, {"north dakota","ND"},
            {"northern mariana islands", "MP"}, {"ohio", "OH"}, {"oklahoma", "OK"}, {"oregon", "OR"},
            {"pennsylvania", "PA"}, {"puerto rico", "PR"}, {"rhode island", "RI"}, {"south carolina", "SC"},
            {"south dakota", "SD"}, {"tennessee", "TN"}, {"texas", "TX"}, {"trust territories", "TT"},
            {"utah", "UT"}, {"vermont", "VT"}, {"virginia", "VA"}, {"virgin islands", "VI"},
            {"washington", "WA"}, {"west virginia", "WV"}, {"wisconsin", "WI"}, {"wyoming", "WY"}
        };

        private string _State;

        public string State
        {
            get => _State;
            set
            {
                if (value != null)
                {
                    if (value.Length > 2)
                    {
                        if (stateAbbrv.ContainsKey(value.Trim().ToLower()))
                        {
                            _State = stateAbbrv[value.ToLower()];
                        }
                        else
                        {
                            _State = "invalid";
                        }

                    }
                    else if (value.Length == 2 && stateAbbrv.ContainsValue(value.Trim().ToUpper()))
                    {
                        _State = value.Trim().ToUpper();
                    }
                    else
                    {
                        _State = "invalid";
                    }
                }
                else
                {
                    _State = value;
                }
            }
        }

        public List<string> Services { get; set; }
        public EmergencyContact EContact { get; set; }

        [Serializable]
        public class EmergencyContact
        {
            private string _emergencyFirstName;
            public string EmergencyFirstName
            {
                get => _emergencyFirstName;
                set
                {
                    if (value != null && value.Length > DatabaseConstants.FIRSTNAME_MAX)
                    {
                        _emergencyFirstName = value.Substring(0, DatabaseConstants.FIRSTNAME_MAX);
                    }
                    else
                    {
                        _emergencyFirstName = value;
                    }
                    EmergencyFullName = _emergencyFirstName + " " + _emergencyLastName;
                }
            }

            private string _emergencyLastName;
            public string EmergencyLastName
            {
                get => _emergencyLastName;
                set
                {
                    if (value != null && value.Length > DatabaseConstants.LASTNAME_MAX)
                    {
                        _emergencyLastName = value.Substring(0, DatabaseConstants.LASTNAME_MAX);
                    }
                    else
                    {
                        _emergencyLastName = value;
                    }
                    EmergencyFullName = _emergencyFirstName + " " + _emergencyLastName;
                }
            }

            private string _emergencyPhoneNumber;
            public string EmergencyPhoneNumber
            {
                get => _emergencyPhoneNumber;
                set
                {
                    if (value != null && value.Length > DatabaseConstants.PHONE_MAX)
                    {
                        _emergencyPhoneNumber = value.Substring(0, DatabaseConstants.PHONE_MAX);
                    }
                    else
                    {
                        _emergencyPhoneNumber = value;
                    }
                }
            }

            private string _emergencyRelationship;
            public string EmergencyRelationship
            {
                get => _emergencyRelationship;
                set
                {
                    if (value != null && value.Length > DatabaseConstants.RELATIONSHIP_MAX)
                    {
                        _emergencyRelationship = value.Substring(0, DatabaseConstants.RELATIONSHIP_MAX);
                    }
                    else
                    {
                        _emergencyRelationship = value;
                    }
                }
            }
            public string EmergencyFullName { get; set; }
            /// <summary>
            /// Initializes a new instance of the EmergencyContact class with the specified values for its properties.
            /// </summary>
            /// <param name="firstName">The first name of the emergency contact.</param>
            /// <param name="lastName">The last name of the emergency contact.</param>
            /// <param name="PhoneNumber">The phone number of the emergency contact.</param>
            /// <param name="Relationship">The relationship of the emergency contact to the client.</param>
            public EmergencyContact(string firstName, string lastName, string PhoneNumber, string Relationship)
            {
                // Set the properties to the specified values
                EmergencyFirstName = firstName;
                EmergencyLastName = lastName;
                EmergencyFullName = firstName + " " + lastName;
                EmergencyPhoneNumber = PhoneNumber;
                EmergencyRelationship = Relationship;
            }
        }

        /// <summary>
        /// Initializes a new instance of the Client class with default property values (null or false for bools).
        /// </summary>
        public Client()
        {
            // Set all properties to null or default values
            DateMet = null;
            FirstName = null;
            MiddleName = null;
            LastName = null;
            Address = null;
            PoBox = null;
            AptNum = null;
            County = null;
            State = null;
            City = null;
            Zipcode = null;
            PhoneNumber = null;
            Dob = null;
            MaritalStatus = null;
            MedicareStatus = null;
            MedicaidStatus = null;
            SocialWorkStatus = null;
            Income = null;
            HouseholdMembers = null;
            Notes = null;
            isDupe = false;
            AptNum = null;
            PoBox = null;
            shouldUpdateServices = false;

            // Create a new empty list of services and an empty EmergencyContact object
            Services = new List<string>();
            EContact = new EmergencyContact(null, null, null, null);
        }

        /// <summary>
        /// Creates a deep copy of the current client object using serialization.
        /// </summary>
        /// <returns>A new instance of the Client class, which is a deep copy of the current object.</returns>
        public Client Copy()
        {
            // Create a new memory stream to hold the serialized object
            using (MemoryStream stream = new MemoryStream())
            {
                // Create a new binary formatter to serialize the object
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, this);

                // Reset the stream position to the beginning
                stream.Position = 0;

                // Deserialize the stream and return the new instance
                return (Client)formatter.Deserialize(stream);
            }
        }

        /// <summary>
        /// This method adds a service to the list of services.
        /// </summary>
        /// <param name="serviceName">The name of the service to add</param>
        public void addService(string serviceName)
        {
            // If the service is not already in the list, add it
            if (!Services.Contains(serviceName))
                Services.Add(serviceName);
        }

        /// <summary>
        /// This method removes a service from the list of services.
        /// </summary>
        /// <param name="serviceName">The name of the service to remove</param>
        public void removeService(string serviceName)
        {
            // If the service is in the list, remove it
            if (Services.Contains(serviceName))
                Services.Remove(serviceName);
        }


        /// <summary>
        /// This method is used to validate client information.
        /// </summary>
        /// <returns>true if client information is valid, false otherwise</returns>
        public bool validateClient()
        {
            // Clear the error message
            errorMessage = "";

            // Trim all the string properties
            FirstName = FirstName?.Trim();
            MiddleName = MiddleName?.Trim();
            LastName = LastName?.Trim();
            DateMet = DateMet?.Trim();
            Address = Address?.Trim();
            PoBox = PoBox?.Trim();
            AptNum = AptNum?.Trim();
            County = County?.Trim();
            State = State?.Trim();
            City = City?.Trim();
            Zipcode = Zipcode?.Trim();

            // Trim the phone number and date of birth properties
            PhoneNumber = PhoneNumber?.Trim();
            Dob = Dob?.Trim();

            // Trim the marital status and notes properties
            MaritalStatus = MaritalStatus?.Trim();
            Notes = Notes?.Trim();

            // Trim the emergency contact properties
            EContact.EmergencyFirstName = EContact.EmergencyFirstName?.Trim();
            EContact.EmergencyLastName = EContact.EmergencyLastName?.Trim();
            EContact.EmergencyPhoneNumber = EContact.EmergencyPhoneNumber?.Trim();
            EContact.EmergencyRelationship = EContact.EmergencyRelationship?.Trim();

            try
            {
                // Convert the date of birth property to a string in the format "MM/dd/yyyy"
                Dob = Convert.ToDateTime(Dob.Trim()).ToString("MM/dd/yyyy");

                // If the date met property is not null, convert it to a string in the format "MM/dd/yyyy"
                if (DateMet != null)
                {
                    DateMet = Convert.ToDateTime(DateMet.Trim()).ToString("MM/dd/yyyy");
                }
            }
            catch (System.FormatException e)
            {
                // If there is a format exception, set the error message and return false
                errorMessage = "Invalid date format.";
                return false;
            }

            // If any of the required fields are missing, set the error message and return false
            if (String.IsNullOrEmpty(FirstName) || String.IsNullOrEmpty(LastName) || String.IsNullOrEmpty(Dob))
            {
                errorMessage = "Missing required fields.";
                return false;
            }

            // If all validations pass, return true
            return true;
        }
    }
}