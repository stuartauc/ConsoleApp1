using Microsoft.VisualStudio.TestTools.UnitTesting;
using LotteryGame.Common;
using Moq;

namespace LotteryGame.Tests
{
    [TestClass]
    public class Tests
    {
        
        //TODO: write tests
        [TestMethod]
        public void CreateUser_ReturnsUser()
        {
            var user = new User();
            var guid = Guid.NewGuid();
            var balance = 10.00M;
            var id = 1;
            user.UserID = id;
            user.UserIdentifier = guid;
            user.Balance = balance;
            var userFactory = new UserFactory();
            var testUser = userFactory.CreateUser(guid, balance, id);
            Assert.IsTrue(balance == testUser.Balance);
            Assert.IsTrue(id == testUser.UserID);
            Assert.IsTrue(guid == testUser.UserIdentifier);
            Assert.IsInstanceOfType(testUser, typeof(User));
            

        }

        [TestMethod]
        public void CreateTicket_ReturnsTicket() 
        {
           
            var userGuid = Guid.NewGuid();
            var ticketGuid = Guid.NewGuid();
            var ticket = new Ticket(ticketGuid, userGuid);
            
            var ticketFactory = new TicketFactory();
            var testTicket = ticketFactory.CreateTicket(ticketGuid, userGuid);
            Assert.IsTrue(ticketGuid == testTicket.TicketId);
            Assert.IsTrue(userGuid == testTicket.UserId);
            Assert.IsInstanceOfType(testTicket, typeof(Ticket));


        }
    }
}
