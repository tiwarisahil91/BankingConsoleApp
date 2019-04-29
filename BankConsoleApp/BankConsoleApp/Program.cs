using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Isam.Esent.Collections.Generic;
using Newtonsoft.Json;
using System.Configuration;
using System.Security.Cryptography;
/**
 * Please note I am assuming negative balance too.
 */
namespace BankConsoleApp
{
    class Program
    {
        static string desktop_path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        public static String CreateSalt(int size)
        {
            var r = new System.Security.Cryptography.RNGCryptoServiceProvider();
            var buffer = new byte[size];
            r.GetBytes(buffer);
            return Convert.ToBase64String(buffer);
        }
        public static string GenerateSHA256Hash(String password, String salt)
        {
            var saltedPassword = password.Trim() + salt;

            var saltedPasswordBytes = System.Text.Encoding.Unicode.GetBytes(saltedPassword);
            var sha2_512 = new SHA512CryptoServiceProvider();

            var hashedPasswordBytes = sha2_512.ComputeHash(saltedPasswordBytes);
            var hashedPassword = Convert.ToBase64String(hashedPasswordBytes);

            return hashedPassword;
        }
        public static void Create_Login(List<Login> list, string login_path)
        {
            Console.WriteLine("Create your username:");
            string username = Console.ReadLine();
            Console.WriteLine("Create your password:");
            string password = Console.ReadLine();
            //dict.Add(username, password);
            string salt = CreateSalt(32);
            string hashedPassword = GenerateSHA256Hash(password, salt);
            list.Add(new Login(username, hashedPassword, salt));
            string data = JsonConvert.SerializeObject(list);
            File.WriteAllText(login_path, data);
            Console.WriteLine("Your account has been successfully created.");
            Console.WriteLine("Would you like to do bank related transactions(Y/N)?");
            string ans = Console.ReadLine();
            if (ans.Equals("Y") || ans.Equals("y"))
            {
                QueryRelatedChoice(username);
            }
        }
        static void Main(string[] args)
        {
            string choice=null;
            //var file=null;
            Console.WriteLine("Do you have an account(Y/N)");
            choice = Console.ReadLine();
            string username, password;
            //string dataPath = Path.Combine(Environment.CurrentDirectory, @"..\App_Data\" + "login");
            //var dict= new PersistentDictionary<string, string>(dataPath);
            // tried using PersistentDictionary, didn't work out
            //string login_path = "C:\\login.json";
            string login_path = desktop_path+"\\login.json";
            if (!File.Exists(login_path))
            {
                File.Create(login_path);
            }
            string JsonString = File.ReadAllText(login_path);
            List<Login> list = JsonConvert.DeserializeObject<List<Login>>(JsonString);
            if (list == null)
            {
                list = new List<Login>();
            }
            if (list!=null && choice.Equals("Y"))
            {
                Console.WriteLine("Enter your username:");
                username = Console.ReadLine();
                Console.WriteLine("Enter your password:");
                password = Console.ReadLine();
                Login login = list.Find(x => x.username.Equals(username));
                if(login==null)
                {
                    Console.WriteLine("Username doesn't exist. Please create your username and password.");
                    Create_Login(list, login_path);
                }
                if(login.password.Equals(GenerateSHA256Hash(password,login.salt)))
                { 
                    QueryRelatedChoice(username);
                }
                else
                {
                    while(!login.password.Equals(GenerateSHA256Hash(password, login.salt)))
                    {
                        Console.WriteLine("Invalid credentials");
                        Console.WriteLine("Enter your username:");
                        username = Console.ReadLine();
                        Console.WriteLine("Enter your password:");
                        password = Console.ReadLine();
                        if (login.password.Equals(GenerateSHA256Hash(password, login.salt)))
                        {
                            QueryRelatedChoice(username);
                            break;
                        }
                    }
                }
            }
            else
            {
                Create_Login(list, login_path);
            }
        }
        public static void QueryRelatedChoice(string username)
        {
            string path =desktop_path+ "\\details.json";
            if(!File.Exists(path))
            {
                File.Create(path);
            }
            string JsonString = File.ReadAllText(path);
            List<UserBankHistory> list = JsonConvert.DeserializeObject<List<UserBankHistory>>(JsonString);

            if(list==null)
            {
                list = new List<UserBankHistory>();
            }
            string choice = "";
            decimal balance = 0;
            UserBankHistory u = new UserBankHistory();
            if (list != null)
            {
                UserBankHistory b = list.Find(x => x.loginname.Equals(username));
                if(b==null)
                {
                    list.Add(new UserBankHistory(username, 0));
                    b = list.Find(x => x.loginname.Equals(username));
                }
                while (choice != "L" && b!=null)
                {
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine("Press D/d to Deposit");
                    Console.WriteLine("Press W/w to Withdrawl");
                    Console.WriteLine("Press C/c to Check Balance");
                    Console.WriteLine("Press T/t to View Transaction History");
                    Console.WriteLine("Press L/l to Logout");
                    Console.WriteLine("Enter your choice:");
                    choice = Console.ReadLine().ToUpper();
                    switch (choice)
                    {
                        case "D":
                            Console.WriteLine("Enter Amount to deposit:");
                            string d = Console.ReadLine();
                            decimal deposit = Convert.ToDecimal(d);
                            balance = b.balance;
                            deposit += b.balance;
                            b.balance = deposit;
                            b.transaction_history.Add("Deposited $" + d + " on " + DateTime.Now);
                            Console.WriteLine("$" + d + " successfully deposited");
                            break;
                        case "W":
                            Console.WriteLine("Enter Amount to withdraw:");
                            string w = Console.ReadLine();
                            decimal withdrawl = Convert.ToDecimal(w);
                            balance = b.balance;
                            balance -= withdrawl;
                            b.balance = balance;
                            b.transaction_history.Add("Withdrawn $" + withdrawl + " on " + DateTime.Now);
                            Console.WriteLine("$" + w + " successfully withdrawn");
                            break;
                        case "C":
                            Console.WriteLine("Your current balance is $" + b.balance);
                            break;
                        case "T":
                            Console.WriteLine("Here is the transaction history for username: " + username);
                            if (b.transaction_history.Capacity == 0 || b.transaction_history == null)
                            {
                                Console.WriteLine("No transaction history recorded");
                                break;
                            }
                            foreach(var item in b.transaction_history)
                            {
                                Console.WriteLine(item);
                            }
                            break;
                        case "L":
                            Console.WriteLine("You have successfully logged out");
                            goto logout;
                            break;
                        default:
                            Console.WriteLine("Invalid choice.Try again");
                            break;
                    }

                }
            logout:
                string data = JsonConvert.SerializeObject(list);
                File.WriteAllText(path, data);
                System.Threading.Thread.Sleep(1000);
                Environment.Exit(0);
            }
            else
            {
                list.Add(new UserBankHistory(username, 0));
                string data = JsonConvert.SerializeObject(list);
                File.WriteAllText(path, data);
            }
        }
    }
}
