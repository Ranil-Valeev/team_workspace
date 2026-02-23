using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Сourse_management.model;

namespace Сourse_management.User_Pages
{
    class CurrentUser
    {
        public static Users User { get; set; }
        public static Student Student { get; set; }
        public static void Logout()
        {
            User = null;
            Student = null;
        }
    }

}
