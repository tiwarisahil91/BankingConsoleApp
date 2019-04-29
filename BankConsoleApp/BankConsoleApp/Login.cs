using System;
using System.Collections.Generic;
using System.Text;

namespace BankConsoleApp
{
    public class Login:IEquatable<Login>
    {
        public string username;
        public string password;
        public string salt;
        public Login() { }
        public Login(string username, string password,string salt)
        {
            this.username = username;
            this.password = password;
            this.salt = salt;
        }
        public bool Equals(Login o)
        {
            if (o == null) return false;
            return this.username.Equals(o.username);
        }
    }
}
