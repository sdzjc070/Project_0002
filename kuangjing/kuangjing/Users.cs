using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace kuangjing
{
    public class Users
    {
        public string name;
        public string admin;
        public Users()
        {
            name = "";
            admin = "";
        }
        public void Logout()
        {
            this.name = "";
            this.admin = "";
        }
    }
}
