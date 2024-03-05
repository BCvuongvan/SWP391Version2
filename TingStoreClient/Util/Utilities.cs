using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TingStoreClient.Util
{

    public class Utilities
    {
        public static bool validPhoneNumber(string phonenumber)
        {
            Regex regex = new Regex("^0[1-9]{2}[-.\\s]?[0-9]{3}[-.\\s]?[0-9]{4}$");
            Match match = regex.Match(phonenumber);
            if (match.Success)
            {
                return true;
            }
            return false;
        }
    }
}