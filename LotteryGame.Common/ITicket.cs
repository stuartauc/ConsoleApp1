﻿using System.Collections.Generic;

namespace LotteryGame.Common
{
    public interface ITicket
    {

        Guid TicketId { get; set; }

        Guid UserId { get; set; }
        Prize Prize { get; set; }

    }
}