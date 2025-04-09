using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class ChatHub : Hub
{
    public async Task SendMessage(string toUser, string message)
    {
        var fromUser = Context.User.Identity.Name;
        var group = GetGroupName(fromUser, toUser);
        await Clients.Group(group).SendAsync("ReceiveMessage", fromUser, message);
    }

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var fromUser = Context.User.Identity.Name;
        var toUser = httpContext.Request.Query["targetUser"];

        if (!string.IsNullOrEmpty(fromUser) && !string.IsNullOrEmpty(toUser))
        {
            
            var group = GetGroupName(fromUser, toUser);
            Console.WriteLine($"✅ Connected: fromUser = {fromUser}, toUser = {toUser}, group = {group}");
            await Groups.AddToGroupAsync(Context.ConnectionId, group);

        }

        await base.OnConnectedAsync();
    }

    private string GetGroupName(string user1, string user2)
    {
        return string.Compare(user1, user2) < 0 ? $"{user1}_{user2}" : $"{user2}_{user1}";
    }

    public async Task Typing(string toUser)
    {
        var fromUser = Context.User.Identity.Name;
        var group = GetGroupName(fromUser, toUser);
        await Clients.Group(group).SendAsync("ShowTyping", fromUser);
    }

}
