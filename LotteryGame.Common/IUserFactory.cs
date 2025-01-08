using System.Collections.Generic;

namespace LotteryGame.Common
{
    public interface IUserFactory
    {

        List<User> CreateUsers(int numberOfCPUUsers = 0, decimal balance = 10.00M);

        User CreateUser(decimal balance = 10.00M, int userId = 1);

        User CreateUser(Guid guid, decimal balance = 10.00M, int userId = 1);

    }
}