using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryGame.Common
{
    public class User : IUser
    {
        public Guid UserIdentifier { get; set; }
        public int UserID { get; set; }

        public string UserName => "Player " + UserID;

        public decimal Balance { get; set; }
        public List<Ticket> Tickets { get; set; }

        public User()
        {
                Balance = 10; //get from config
            Tickets = new List<Ticket>();
        }
        public User(Guid userIdentifier)
        {
            UserIdentifier = UserIdentifier;
            Balance = 10; //get from config
            Tickets = new List<Ticket>();
        }
    }
}
