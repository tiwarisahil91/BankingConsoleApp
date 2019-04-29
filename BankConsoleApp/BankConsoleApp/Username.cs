using System;
using System.Collections.Generic;
using System.Text;

namespace BankConsoleApp
{
    public class UserBankHistory:IEquatable<UserBankHistory>
    {
        public string loginname;
        public decimal balance;
        public List<string> transaction_history;
        public UserBankHistory() { }
        public UserBankHistory(string loginname, decimal balance=0)
        {
            this.loginname = loginname;
            this.balance = balance;
            this.transaction_history = new List<string>();
        }
        public bool Equals(UserBankHistory o)
        {
            if (o == null) return false;
            return this.loginname.Equals(o.loginname);
        }
    }
}
