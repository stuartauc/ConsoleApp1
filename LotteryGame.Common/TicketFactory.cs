using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryGame.Common
{
    public class TicketFactory : ITicketFactory
    {


        

        public ITicket CreateTicket(Guid ticketId, Guid userId)
        {
            return new Ticket(ticketId, userId);
        }

        public ITicket CreateWinningTicket(Guid ticketId)
        {
             return new Ticket(ticketId, Guid.Empty);
        }

        //public ITicket CreateListOfNumbersTicket(IList<int> numbers)
        //{
        //    return new Ticket(numbers);
        //}

        ITicket CreateIndividualNumbersTicket(int firstNumber, int secondNumber, int thirdNumber, int fourthNumber, int fifthNumber, int sixthNumber)
        {
            throw new NotImplementedException();
        }
    }
}
