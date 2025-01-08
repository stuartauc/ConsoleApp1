using System.Globalization;
using LotteryGame.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Configuration;
using Serilog;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton<ILottery, Lottery>();
builder.Services.AddSingleton<ITicketFactory, TicketFactory>();
builder.Services.AddSingleton<IUserFactory, UserFactory>();
using IHost host = builder.Build();
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File("log.txt", rollingInterval: RollingInterval.Hour, rollOnFileSizeLimit: true)
    .CreateLogger();
Log.Information("Logging Started");
//TODO:set up logging
//Logging needs to be configured, including type and location of txt files

var lottery = host.Services.GetRequiredService<ILottery>();
var userTicketEntry = new List<int>();
//create the winning ticket
//var generateUsers = 15;//get the numberOfUsers from config file//lottery.GenerateNumbers();
var randomGuid = Guid.NewGuid();
var emptyGuid = Guid.Empty;
var winningTicket = lottery.CreateTicket(randomGuid, emptyGuid, TicketType.WinningTicket);
//set player to Player 1 - prompt for number of tickets
//set initial balance for Player1
//prompt for number of tickets
//generate tickets for User based on prompt
//enhancement - user can enter his own numbers
var balance = decimal.Parse(ConfigurationManager.AppSettings.Get("Balance") ?? "10.00");
var ticketCost = decimal.Parse(ConfigurationManager.AppSettings.Get("TicketCost") ?? "1.00");
Random rand = new Random();
int minUsers = int.Parse(ConfigurationManager.AppSettings.Get("MinCPUPlayers") ?? "9");
int maxUsers = int.Parse(ConfigurationManager.AppSettings.Get("MaxCPUPlayers") ?? "14");
int minTickets = int.Parse(ConfigurationManager.AppSettings.Get("MinTickets") ?? "1");
int maxTickets = int.Parse(ConfigurationManager.AppSettings.Get("MaxTickets") ?? "10");

string playAgain = "n";
do
{

    Console.WriteLine($"Your balance is " + string.Format(new CultureInfo("en-SG", false), "{0:c2}", balance));
    Console.WriteLine($"Tickets are " + string.Format(new CultureInfo("en-SG", false), "{0:c2}", ticketCost));
    Console.WriteLine($"How many tickets would you like to buy?");
    var maxTicketsAffordable = balance / ticketCost;
    int tickets = 0;
    bool parsed = Int32.TryParse(Console.ReadLine(), out tickets);
    while ((tickets < minTickets || tickets > maxTickets) || tickets >= maxTicketsAffordable)
    {

        Console.WriteLine("Enter number of tickets between " + minTickets + " and " + maxTickets);
        Int32.TryParse(Console.ReadLine(), out tickets);
    }
    //if playAgain is "y" then remove all tickets from all users, keep balance

    if(playAgain == "n")
    {
        var users = lottery.CreateUsers(rand.Next(minUsers, maxUsers), balance);
        lottery.Users.Add(lottery.CreateUser(balance));
        
    }
    else
    {
        lottery.LotteryTickets.Clear();
        lottery.WinningTickets.Clear();
        lottery.ClearTicketsByUser();
    }
    

    lottery.InitialiseUsers(ticketCost, tickets, minTickets, maxTickets);

    lottery.initialiseLotteryTickets();
    var lotteryTickets = lottery.LotteryTickets;
    var winningTickets = lottery.WinningTickets;
    lottery.CreatePrize(ref lotteryTickets, ref winningTickets);



    var firstPrizeUser = lottery.GetFirstPrizeWinner();

    Console.WriteLine(lottery.Users.Count - 1 + " other CPU players also purchased tickets.");
    Console.WriteLine();
    Console.WriteLine("Ticket Draw Results:");
    Console.WriteLine();
    //Console.WriteLine("* Grand Prize: " + firstPrizeUser.UserName + " wins " + string.Format(new CultureInfo("en-SG", false), "{0:c2}", firstPrizeUser.Tickets.Where(t => t.Prize.PrizeType == PrizeType.First).Single().Prize.PrizeValue));
    Console.WriteLine("* Grand Prize: " + firstPrizeUser.UserName + " wins " + string.Format(new CultureInfo("en-SG", false), "{0:c2}", lottery.FirstPrizePayoutEach));
    lottery.PrintTierText(lottery.GetSecondPrizeWinners, "* Second Tier: ", PrizeType.Second);
    Console.WriteLine();
    lottery.PrintTierText(lottery.GetThirdPrizeWinners, "* Third Tier: ", PrizeType.Third);
    Console.WriteLine();
    Console.WriteLine("Congratulations to the winners!");
    Console.WriteLine();
    Console.WriteLine("House Revenue: " + string.Format(new CultureInfo("en-SG", false), "{0:c2}", lottery.HouseProfit));
    Console.WriteLine();

    Console.WriteLine("Do you want to continue playing? y / n?");
    playAgain = Console.ReadLine() ?? string.Empty;
}
while (playAgain == "y");

host.Run();


