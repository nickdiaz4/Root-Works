using System.ComponentModel;
using System.Collections.Generic;

namespace Catawba
{
    public class Services
    {
        private const string BLANK = "";
        private const int NO_ID = 0;

        public string serviceName { get; set; }

        /// <summary>
        /// Creates an instance of a Services object
        /// Simple class with only one piece of information: Service Name
        /// </summary>
        public Services()
        {
            serviceName = null;
        }
    }
}
