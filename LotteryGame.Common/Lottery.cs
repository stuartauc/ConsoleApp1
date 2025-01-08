using System.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

namespace LotteryGame.Common
{
    
    public class Lottery : ILottery
    {

        private readonly ITicketFactory _factory;
        private readonly IUserFactory _userFactory;
        public Lottery(ITicketFactory factory, IUserFactory userFactory)
        {
            _factory = factory;
            _userFactory = userFactory;
        }
        public required List<User> Users { get; set; }

        public required List<Ticket> LotteryTickets { get; set; } //collection of all tickets minus winning tickets

        public required List<Ticket> WinningTickets { get; set; } //collection of winning tickets

        public required Ticket LotteryTicket { get; set; } //winning ticket numbers

        public required decimal HouseProfit { get; set; } = 0.00M;

        public Ticket CreateTicket(Guid ticketId, Guid userId, TicketType ticketType)
        {
            switch (ticketType)
            {
                case TicketType.LuckyDip:
                    return (Ticket)_factory.CreateTicket(ticketId, userId);
                case TicketType.WinningTicket:
                    LotteryTicket = (Ticket)_factory.CreateWinningTicket(ticketId);
                    return LotteryTicket;
                default:
                    return (Ticket)_factory.CreateTicket(ticketId, userId);
            }

        }

        public decimal FirstPrizePayoutEach { get; set; }
        public decimal SecondPrizePayoutEach { get; set; }
        public decimal ThirdPrizePayoutEach { get; set; }
        public decimal FirstPrize { get;} = 0.5M;
        public Tuple<decimal, decimal> SecondPrize { get;} = Tuple.Create(0.1M, 0.3M);
        public Tuple<decimal, decimal> ThirdPrize { get; } = Tuple.Create(0.2M, 0.1M);

        public void initialiseLotteryTickets()
        {
            WinningTickets = new List<Ticket>();
            LotteryTickets = new List<Ticket>();
            foreach(var u in Users)
            {
                LotteryTickets.AddRange(u.Tickets);
            }
        }
        public void CreatePrize(ref List<Ticket> tickets, ref List<Ticket> winningTickets)
        {
            
            HouseProfit = tickets.Count * 1;
            Ticket firstPrizeTicket;
            var totalRemainder = 0.00M;
            foreach (PrizeType p in Enum.GetValues(typeof(PrizeType)))
            {
                switch (p)
                {
                    case PrizeType.First:
                        firstPrizeTicket = CalculateFirstPrize(ref tickets, FirstPrize, ref winningTickets);
                        break;
                    case PrizeType.Second:
                        totalRemainder += CalculatePrizes(ref tickets, SecondPrize.Item2, SecondPrize.Item1, ref winningTickets, PrizeType.Second);
                        break;
                    case PrizeType.Third:
                        totalRemainder += CalculatePrizes(ref tickets, ThirdPrize.Item2, ThirdPrize.Item1, ref winningTickets, PrizeType.Third);
                        break;
                    default:
                        break;
                }
            }
            HouseProfit += totalRemainder;
        }

        private void CalculateThirdPrize(ref List<Ticket> tickets, decimal prizePerentage, decimal ticketPercentage, ref List<Ticket> remainingTickets)
        {
            var numOfTickets = remainingTickets.Count;
            var numberOfWinningTickets = (int)Math.Round(tickets.Count * ticketPercentage);
            var totalPrizeValue = prizePerentage * tickets.Count * 1;//get the count from a config, pass into this method
            var totalPrizeValueEach = totalPrizeValue / numberOfWinningTickets;

            Random rand = new Random();
            for (int i = 0; i < numberOfWinningTickets; i++)
            {
                int index = rand.Next(1, remainingTickets.Count);

                Ticket ticket = tickets[index];
                var prizeStruct = new Prize();
                prizeStruct.PrizeType = PrizeType.Third;
                prizeStruct.PrizeValue = totalPrizeValueEach; //calculate the prize based on the percentage
                ticket.Prize = prizeStruct;

                remainingTickets.RemoveAt(index);

            }
        }
        private decimal RoundDown(decimal i, double decimalPlaces)
        {
            var power = Convert.ToDecimal(Math.Pow(10, decimalPlaces));
            return Math.Floor(i * power) / power;
        }

        private decimal CalculatePrizes(ref List<Ticket> tickets, decimal prizePerentage, decimal ticketPercentage, ref List<Ticket> winningTickets, PrizeType prizeType)
        {
            var currentTierWinningTickets = new List<Ticket>(); 
            //add logic so that the same user cant win the same prize more than once
            var numOfTickets = tickets.Count;
            var numberOfWinningTickets = (int)Math.Round(tickets.Count*ticketPercentage);//needs minimum of 5 tickets
            var totalPrizeValue = prizePerentage * tickets.Count * 1;//get the count from a config, pass into this method
            var totalPrizeValueEach = RoundDown(totalPrizeValue / numberOfWinningTickets, 2);
            if (prizeType == PrizeType.Third)
            {
                ThirdPrizePayoutEach = totalPrizeValueEach;
            }
            else if (prizeType == PrizeType.Second)
            {
                SecondPrizePayoutEach = totalPrizeValueEach;
            }
                
            var totalRemainder = totalPrizeValue - (totalPrizeValueEach * numberOfWinningTickets);
            totalPrizeValueEach = totalPrizeValue / numberOfWinningTickets;
            HouseProfit = HouseProfit - totalPrizeValue + totalRemainder;
            Random rand = new Random();
            var listOfIds = new HashSet<Guid>();
            var listOfTickets = new List<Ticket>();
            var runningTotal = 0;

            while(listOfIds.Count() < numberOfWinningTickets)
            {
                var index = rand.Next(tickets.Count);
                Ticket ticket = tickets[index];
                
                if (!listOfIds.Contains(ticket.TicketId))
                {
                    listOfIds.Add(ticket.TicketId);
                    var prizeStruct = new Prize();
                    prizeStruct.PrizeType = prizeType;
                    prizeStruct.PrizeValue = totalPrizeValueEach; //calculate the prize based on the percentage
                    ticket.Prize = prizeStruct;

                    tickets.RemoveAt(index);
                    winningTickets.Add(ticket);
                    listOfIds.Add(ticket.TicketId);
                }
                

            }
            //var ids = Enumerable.Range(0, numberOfWinningTickets).ToArray();

            //for (int i = 0; i < ids.Length; i++)
            //{
            //    int index;
            //    index = rand.Next(ids.Length);
            //    int temp = ids[index];
            //    ids[index] = ids[i];
            //    ids[i] = temp;
            //}
            //for (int i = 0; i < listOfIds.Count; i++)
            //{
            //    Ticket ticket = tickets[i];

            //    var prizeStruct = new Prize();
            //    prizeStruct.PrizeType = prizeType;
            //    prizeStruct.PrizeValue = totalPrizeValueEach; //calculate the prize based on the percentage
            //    ticket.Prize = prizeStruct;

            //    tickets.RemoveAt([i]);
            //    winningTickets.Add(ticket);
            //}
            
            //add profit to remainder
            return totalRemainder;
        }
        public Ticket CalculateFirstPrize(ref List<Ticket> tickets, decimal prizePerentage, ref List<Ticket> winningTickets)
        {
            var numOfTickets = tickets.Count;
            var prizeValue = RoundDown(prizePerentage * numOfTickets, 2);
            FirstPrizePayoutEach = prizeValue;
            
            HouseProfit = HouseProfit - prizeValue;
            Random rand = new Random();
            int index = rand.Next(0, tickets.Count);
            Ticket ticket = tickets[index];
            var prizeStruct = new Prize();
            prizeStruct.PrizeType = PrizeType.First;
            prizeStruct.PrizeValue = prizeValue; //calculate the prize based on the percentage
            ticket.Prize = prizeStruct;
            tickets.RemoveAt(index);
            winningTickets.Add(ticket);
            return ticket;
        }

        public User GetFirstPrizeWinner()
        {
            var winningTicket = WinningTickets.Where(x => x.Prize.PrizeType == PrizeType.First).First();
            var user = Users.Where(u => u.UserIdentifier == winningTicket.UserId).First();
            user.Balance = user.Balance + winningTicket.Prize.PrizeValue;
            return user;
        }
        public List<Tuple<User, decimal>> GetSecondPrizeWinners()
        {
            //var winningTickets = WinningTickets.Where(x => x.Prize.PrizeType == PrizeType.Second).ToList();
            var u = new List<Tuple<User, decimal>>();
            var winningUsers = new List<User>();
            //var users = Users.Where(u => winningTickets.Contains(u.UserIdentifier)).ToList();
            foreach (var user in Users) 
            {
                var currentWinnings = 0.00M;
                var winningTickets = WinningTickets.Where(x => x.Prize.PrizeType == PrizeType.Second && x.UserId == user.UserIdentifier).ToList();
                var tickets = user.Tickets;
                foreach (var tick in winningTickets)
                {
                    if (tick.UserId == user.UserIdentifier)
                    {
                        user.Balance += tick.Prize.PrizeValue;
                        winningUsers.Add(user);
                        currentWinnings += tick.Prize.PrizeValue;
                    }
                }
                if (winningTickets.Count > 0)
                {
                    u.Add(Tuple.Create(user, currentWinnings));
                }
                
            }
            return u;
        }

        public List<Tuple<User, decimal>> GetThirdPrizeWinners()
        {
            var u = new List<Tuple<User, decimal>>();
            
            var winningUsers = new List<User>();
            //var users = Users.Where(u => winningTickets.Contains(u.UserIdentifier)).ToList();
            foreach (var user in Users)
            {
                var currentWinnings = 0.00M;
                var winningTickets = WinningTickets.Where(x => x.Prize.PrizeType == PrizeType.Third && x.UserId == user.UserIdentifier).ToList();
                var tickets = user.Tickets;
                foreach (var tick in winningTickets)
                {
                    if (tick.UserId == user.UserIdentifier)
                    {
                        user.Balance += tick.Prize.PrizeValue;
                        currentWinnings += tick.Prize.PrizeValue;


                    }
                }
                u.Add(Tuple.Create(user, currentWinnings));
            }
            return u;
        }
        public string PrintTierText(Func<List<Tuple<User, decimal>>> getPrizeWinners, string introText, PrizeType prizeType)
        {
            var users = getPrizeWinners().ToList();
            //group by prize
            Console.Write(introText);
            for (int u = 0; u < users.Count; u++)
            {
                if (users[u].Item2 > 0)
                {
                    Console.Write(" Player " + users[u].Item1.UserID + " won ");
                    Console.Write(string.Format(new CultureInfo("en-SG", false), "{0:c2}",users[u].Item2) + ".");
                }
                //Console.Write(users[u].Item1.UserID + " ");

            }
            
            //Console.Write(string.Format(new CultureInfo("en-SG", false), "{0:c2}",
            //    users.Last().Tickets.Where(t => t.Prize.PrizeType == prizeType).First().Prize.PrizeValue));
            //Console.Write(" each!");
            return string.Empty;
        }

        public List<User> CreateUsers(int numberOfCPUUsers = 10, decimal balance = 10.00M)
        {
            Users = _userFactory.CreateUsers(numberOfCPUUsers);
            return Users;

        }
        
        public User CreateUser(decimal balance = 10.00M)
        {

            var user = _userFactory.CreateUser(balance);
            return user;
            //int numberOfTickets = 0;
            //try
            //{
            //    //check input is parse as integer
            //    bool isParsed = Int32.TryParse(input, out numberOfTickets);
            //    if (isParsed)
            //    {
            //        var user = _userFactory.CreateUser(numberOfTickets);
            //        Users.Add(user);
            //        //create the tickets
            //        for (int i = 0; i < numberOfTickets; i++)
            //        {
            //            _factory.CreateTicket(Guid.NewGuid(), user.UserIdentifier);
            //        }
            //    }
            //    return user;

            //}
            //catch (Exception)
            //{

            //    throw;
            //}

        }
        public void ClearTicketsByUser()
        {
            foreach (var item in Users)
            {
                item.Tickets.Clear();
            }
        }

        public void InitialiseUsers(decimal ticketCost = 1.00M, int numberOfTickets = 10, int minTickets = 1, int maxTickets = 11)
        {
            Random rand = new Random();
            for (int i = 0; i < Users.Count; i++)
            {
                if(Users[i] != null)
                {
                    if (Users[i].UserID != 1)
                        InitialiseUser(Users[i], rand.Next(minTickets, maxTickets), ticketCost);
                    else
                        InitialiseUser(Users[i], numberOfTickets, ticketCost);
                }
                

            }
        }
        public void InitialiseUser(User user, int numberOfTickets, decimal ticketCost = 1.00M)
        {
                //get the number of tickets bought
                //var tickets = new Random().Next(0, numberOfTickets);
                for (int j = 0; j < numberOfTickets; j++)
                {
                    var ticket = (Ticket)_factory.CreateTicket(Guid.NewGuid(), user.UserIdentifier);
                    //add to the array of tickets for this user
                    user.Tickets.Add(ticket);
                    user.Balance = (user.Balance - ticketCost);
                    //the ticket price should be from config
                }

        }

        public List<int> GenerateNumbers()
        {
            var random = new Random();
            var randomNumbers = Enumerable.Range(10, 15).OrderBy(x => random.Next()).Take(6).ToList();
            return randomNumbers;
        }

    }
}