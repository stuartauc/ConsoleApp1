
namespace LotteryGame.Common
{
    internal interface IUser
    {
        List<Ticket> Tickets { get; }
        int UserID { get; set; }
        Guid UserIdentifier { get; set; }
        string UserName { get; }

        decimal Balance { get; set; }
    }
}