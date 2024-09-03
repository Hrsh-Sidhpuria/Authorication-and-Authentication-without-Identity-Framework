using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class ChatHub : Hub
{
    public async Task SendMessageToUser(string userId, string message)
    {
        // Send message to the specific connection
        await Clients.User(userId).SendAsync("ReceiveMessage", message);
    }
}
