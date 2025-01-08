using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryGame.Common
{
    public class UserFactory : IUserFactory
    {
        public User CreateUser(decimal balance = 10.00M, int userId = 1)
        {
                var user = new User()
                { 
                    UserIdentifier = Guid.NewGuid(),
                    UserID = userId,
                    Balance = balance
                };
            return user;
        }
        public User CreateUser(Guid guid, decimal balance = 10.00M, int userId = 1)
        {
            var user = new User()
            {
                UserIdentifier = guid,
                UserID = userId,
                Balance = balance
            };
            return user;
        }


        public List<User> CreateUsers(int numberOfCPUUsers = 10, decimal balance = 10.00M)
        {
            var users = new List<User>();
            for (int i = 1; i <= numberOfCPUUsers; i++)
            {
                users.Add(new User()
                {
                    UserIdentifier = Guid.NewGuid(),
                    UserID = i + 1,
                    Balance = balance
                });
            }
            return users;
        }

    }
}
