﻿using System.Collections.Generic;

namespace LotteryGame.Common
{
    public interface ITicketFactory
    {

        ITicket CreateTicket(Guid ticketId, Guid userId);
        ITicket CreateWinningTicket(Guid ticketId);
        
    }
}