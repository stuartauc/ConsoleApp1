namespace LotteryGame.Common
{
    public interface ILottery
    {
        Ticket LotteryTicket { get; set; }
        List<Ticket> LotteryTickets { get; set; }

        List<Ticket> WinningTickets { get; set; }

        List<User> Users { get; set; }

        void ClearTicketsByUser();

        Ticket CreateTicket(Guid ticketId, Guid userId, TicketType ticketType);

        void initialiseLotteryTickets();
        decimal FirstPrizePayoutEach { get; set; }

        List<User> CreateUsers(int numberOfCPUUsers = 10, decimal balance = 10.00M);

        void InitialiseUsers(decimal ticketCost = 1.00M, int numberOfTickets = 10, int minTickets = 1, int maxTickets = 11);

        User GetFirstPrizeWinner();
        string PrintTierText(Func<List<Tuple<User, decimal>>> getPrizeWinners, string introText, PrizeType prizeType);
        List<Tuple<User, decimal>> GetSecondPrizeWinners();
        List<Tuple<User, decimal>> GetThirdPrizeWinners();
        List<int> GenerateNumbers();

        decimal HouseProfit { get; set; }

        User CreateUser(decimal balance = 10.00M);
        void CreatePrize(ref List<Ticket> tickets, ref List<Ticket> winningTickets);


    }
}