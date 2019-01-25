using GTANetworkAPI;
using LiteDB;
using RAGECore;
using System;
using System.Collections.Generic;
using System.Linq;
using static RAGEDatabase.DatabaseModels;

namespace RAGEDatabase
{
    public class DatabaseManager : Script, RAGEModule
    {
        public int Version { get; set; } = 1;
        public string Name { get; set; } = "Database Module";

        public void Load()
        {
            Console.WriteLine("Hello from database module.... epic victory royale...");
        }
        
        public void Unload()
        {

        }

        public void Start()
        {

        }

        private static LiteDatabase Account_Database = new LiteDatabase("Accounts.db");
        private static Dictionary<Client, Account> AttachedAccounts = new Dictionary<Client, Account>();

        [ServerEvent(Event.PlayerDisconnected)]
        public void OnPlayerDisconnected(Client player, DisconnectionType type, string reason)
        {
            if (IsClientAttached(player))
            {
                Account model = AttachedAccounts[player];
                if (!Account_Database.GetCollection<Account>("Accounts").Update(model))
                    Account_Database.GetCollection<Account>("Accounts").Insert(model);
                AttachedAccounts.Remove(player);
            }
        }

        public static bool AttachClient(Client player, Account acc)
        {
            if (!IsClientAttached(player))
            {
                AttachedAccounts[player] = acc;
                acc.LastLogin = DateTime.Now;
                UpdateAccount(acc);
                return true;
            }
            return false;
        }

        public static bool UpdateAccount(Account acc)
        {
            if (acc == null)
                return false;
            if (!Account_Database.GetCollection<Account>("Accounts").Update(acc))
                Account_Database.GetCollection<Account>("Accounts").Insert(acc);
            return true;
        }

        public static Account GetAttachedAccount(Client player)
        {
            Account acc;
            return AttachedAccounts.TryGetValue(player, out acc) ? acc : null;
        }

        public static Account GetAccount(string username)
        {
            Account acc = AttachedAccounts.Values.FirstOrDefault(x => x.Username == username);
            if (acc == default(Account))
                return Account_Database.GetCollection<Account>("Accounts").FindOne(Query.EQ("Username", username));
            else
                return acc;
        }

        public static bool IsClientAttached(Client player)
        {
            return AttachedAccounts.ContainsKey(player);
        }

        public static bool DoesAccountExist(string username)
        {
            LiteCollection<Account> accs = Account_Database.GetCollection<Account>("Accounts");
            if (accs.Count() <= 0)
                return false;
            return accs.FindOne(Query.EQ("Username", username)) != default(Account);
        }

        public static bool CreateAccount(Client player, string username, string password)
        {
            if (!DoesAccountExist(username))
            {
                string salt = BCrypt.BCryptHelper.GenerateSalt();
                string hash = BCrypt.BCryptHelper.HashPassword(password, salt);

                Account acc = new Account
                {
                    Name = player.Name,
                    Username = username,
                    Password = hash,
                    HardwareID = player.Serial,
                    SocialClubID = player.SocialClubName,
                    RegisteredTime = DateTime.Now,
                };

                Account_Database.GetCollection<Account>("Accounts").Insert(acc);

                return true;
            }
            return false;
        }
    }
}
