using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TanglewoodLMS
{
    public class loggedInUser
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public bool Admin { get; set; }

        public loggedInUser(int userId, string name, string surname, bool admin)
        {
            UserId = userId;
            Name = name;
            Surname = surname;
            Admin = admin;
        }
    }
}