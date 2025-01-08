﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryGame.Common
{
    public class Ticket : ITicket
    {
        

        public Guid TicketId { get; set; }

        public Guid UserId { get; set; }

        public Prize Prize { get; set; }







        public Ticket(Guid ticketId, Guid userId) // list parameter
        {
            TicketId = ticketId;
            UserId = userId;

        }

        public ITicket CreateTicket()
        {
            throw new NotImplementedException();
        }
    }
}