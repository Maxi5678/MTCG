using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Models
{
    public class User
    {
        public int id;
        public string username;
        public string password;
        public int currency = 20;
        public int elo = 100;
        public string token;

        public User(int id, String username, String password, int currency, int elo)
        {
            this.id = id;
            this.username = username;
            this.password = password;
            this.currency = currency;
            this.elo = elo; 
        }

        public User(String username, String password)
        {
            this.username = username;
            this.password = password;
        }

    }
}

