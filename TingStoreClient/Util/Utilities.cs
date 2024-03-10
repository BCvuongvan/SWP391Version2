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

        public static bool validEmail(string email)
        {
            Regex regex = new Regex("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$");
            Match match = regex.Match(email);
            if (match.Success)
            {
                return true;
            }
            return false;
        }

        public static bool validPassword(string password)
        {
            Regex regex = new Regex("^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,}$");
            Match match = regex.Match(password);
            if (match.Success)
            {
                return true;
            }
            return false;
        }

    }
}