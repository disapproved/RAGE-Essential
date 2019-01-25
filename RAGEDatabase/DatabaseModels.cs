using System;
using System.Collections.Generic;
using System.Text;

namespace RAGEDatabase
{
    public class DatabaseModels
    {
        public class Account
        {
            public int Id { get; set; }
            public string HardwareID { get; set; }
            public string SocialClubID { get; set; }
            public string Name { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public DateTime RegisteredTime { get; set; }
            public DateTime LastLogin { get; set; }
        }
    }
}
