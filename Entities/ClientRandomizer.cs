using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Catawba
{
    public class ClientRandomizer
    {
        public static string[] names = new string[] { "Dominick", "Sandy", "Olivia", "Jessie", "Kenny", "Oliver", "Orville", "Casey", "Lillie", "Guillermo",
                                                    "Francisco ","Sara", "Francis","Damon","Jessica","Matt","Bradley","Max","Floyd","Glenn","Gregory",
                                                    "Dixon","Lewis","Shaw","Terry","Davidson","Nash","Cox","Banks","Hayes","Ronald","Blanche","Beulah","Byrd","Christensen",
                                                    "Nichols","Lucas","Curtis","Levi","Ortiz","Hugo","Hogan","Sims","Marcos","Amos","Stewart","Medina","Fernandez",
                                                    "Angela","Ralph","Ivan","Tate","Tasha","Schneider","Loretta","Reyes","Cassandra","Swanson","Harvey","Schultz","Chandler","Larson","Dave",
                                                    "Eric","Sanchez","Felipe","Graham","Morton","Cynthia","Erik","Jody","Ramon","Leonard","Cody","Mathew","Garcia","Heather","Selina","Reid",
                                                    "Syeda","Hammond","Marianne","Padilla","Fabio","Fintan","Bray","Elaine","Salazar","Maximilian","Berg","Leonie","Snow","Flores","Aneesa",
                                                    "Amaya","Abel","Shayla","Gillespie","Pamela","Freeman","Cline","Fatimah","Roman","Sapphire","Villarreal","Pippa","Yoder","Kamal","Grimes",
                                                    "Bolton","Yasmine","Justin","Lucian","Keenan","Lorna","Kelly","Manning","Fitzpatrick","Hendrix","Kingsley","Sasha","Glover","Janet"};

        //static string[] address = new string[] { "7912 Bohemia Street", "7835 Central Court", "9 Shady St.", "37C Pumpkin Hill Ave.", "233 Vine St.", "40 E. Inverness Drive",
        //                                                   "8307 North Front Street", "693 W. Marshall Ave.", "981 Cross Street", "48 Shipley Ave.",
        //                                                   "9506 Myers Road ","863 Shirley Lane", "9673 Ridgeview Court","655 Augusta Ave.","228 Thompson Dr.",
        //                                                   "589 Greenview Street","936 Eagle Court","95 University Lane","29 Edgemont St.","322 Hanover St.","9170 Hilltop Court"};

        //static string[] city = new string[] { "Portland", "Sunnyside", "Little Rock", "Miamisburg", "Langhorne", "Geneva", "Woburn", "Ashburn", "Media", "Cheshire",
        //                                    "King Of Prussia ","Lake Jackson", "Indian Trail","Pleasanton","Colonial Heights","Norwich","Anchorage","Downers Grove","Chandler",
        //                                    "Bay City","Sicklerville","Saint Louis","Dundalk","Snellville","Greenwood","Cox","Chaska","Sebastian","3862 Warner Street",
        //                                    "1769 Cliffside Drive","1424 Lee Avenue","4599 John Calvin Drive","680 Mercer Street","448 Hornor Avenue","2281 Richland Avenue",
        //                                    "2168 Rodney Street","2056 Neuport Lane","4402 Tuna Street","1829 Redbud Drive","2499 Kildeer Drive","3903 Providence Lane","4847 Moore Avenue","2628 Crestview Manor"};

        static string[] state = new string[] { "SC", "MI","CA","FL","MN","NJ","IN","WI","VA","IL",
                                                           "MD","CT","IA","CT","NY","PA","TN","KY","OH","UT"};

        static string[] county = new string[] { "Iroquois", "Orange", "Knott", "Estill", "Bolivar", "Weld", "Cobb", "Cass", "Elk", "Middlesex",
                                                           "Dillingham ","Crockett", "Berks","Jefferson","Union","York","Volusia","Alameda","San Diego","Chenango","Perry",
                                                            "Washington","Macon","Vilas","Texas","Los Angeles","Kings","Schuylkill","Quitman","Polk"};


        static string[] marital = new string[] {"Single","Married","Divorced"};
        static string[] relation = new string[] {"sister","brother","daughter","son", "in-law" };

        static string[] services = new string[] { "Home Delivered Meals", "Homemaker", "CLTC","Home Health","Hospice","Personal Sitter","Caregiver","Assessments","Ombudsman" };

        static Random rand = new Random();
        public static Client RandomClient()
        {
            Client client = new Client();


            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = httpClient.GetAsync("https://randomuser.me/api/").Result;
            response.EnsureSuccessStatusCode();

            string responseBody = response.Content.ReadAsStringAsync().Result;
            JsonDocument responseJson = JsonDocument.Parse(responseBody);
            JsonElement nameElement = responseJson.RootElement.GetProperty("results")[0].GetProperty("name");
            JsonElement locationElement = responseJson.RootElement.GetProperty("results")[0].GetProperty("location");

            client.FirstName = nameElement.GetProperty("first").GetString();
            client.LastName = nameElement.GetProperty("last").GetString();
            client.MiddleName = ((char)(rand.Next(26) + 97)).ToString().ToUpper();
            client.Address = $"{locationElement.GetProperty("street").GetProperty("number").GetInt32()} " +
                             $"{locationElement.GetProperty("street").GetProperty("name").GetString()}";

            client.City = locationElement.GetProperty("city").GetString();
            client.State = state[rand.Next(0, state.Length)];
            client.County = county[rand.Next(0, county.Length)];
            client.Zipcode = zipGen();
            client.PhoneNumber = phoneGen();
            client.Dob = dobGen();
            client.Income = incomeGen();
            client.HouseholdMembers = (sbyte?)rand.Next(0, 10);
            client.MaritalStatus = marital[rand.Next(0, marital.Length)];
            client.MedicaidStatus = boolGen();
            client.MedicareStatus = boolGen();
            client.SocialWorkStatus = boolGen();
            client.EContact.EmergencyFirstName = names[rand.Next(0, names.Length)];
            client.EContact.EmergencyLastName = names[rand.Next(0, names.Length)];
            client.EContact.EmergencyRelationship = relation[rand.Next(0, relation.Length)];
            client.EContact.EmergencyPhoneNumber = phoneGen();
            addServices(client);
            return client;
        }

        static void addServices(Client client)
        {
            int serviceCount = rand.Next(0, 9);
            for (int i = 1; i <= serviceCount; i++)
            {
                client.addService(services[rand.Next(0,services.Length - 1)]);
            }
        }

        static bool boolGen()
        {
            return Convert.ToBoolean(rand.Next(0, 2));
        }

        static string phoneGen()
        {
            Random random = new Random();
            string phone = "";
            for (int i = 1; i <= 10; i++)
            {
                phone += random.Next(0, 10).ToString();
            }

            return phone;
        }

        static string dobGen()
        {
            string dob = "";
            dob += 1.ToString() + 9.ToString() + rand.Next(3, 8).ToString() + rand.Next(0, 10).ToString() + "/";
            int monthStart = rand.Next(0, 2);
            dob += monthStart.ToString();

            if (monthStart == 0)
            {
                dob += rand.Next(1, 10).ToString();
            }
            else
            {
                dob += rand.Next(0, 3).ToString();
            }
            int dayStart = rand.Next(0, 3);
            dob += "/" + dayStart.ToString();
            if (dayStart == 0)
                dob += rand.Next(1, 9);
            else
            {
                dob += rand.Next(0, 9);
            }

            return dob;
        }

        static string zipGen()
        {
            return rand.Next(10000, 99999).ToString();
        }

        static int incomeGen()
        {
            return rand.Next(10000, 999999);
        }
    }
}
