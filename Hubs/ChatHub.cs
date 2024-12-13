using Microsoft.AspNetCore.SignalR;

namespace social_oc_api.Hubs
{
    public class ChatHub : Hub
    {

        private readonly ILogger<ChatHub> _logger;

        public ChatHub(ILogger<ChatHub> logger)
        {
            _logger = logger;
        }
        public async Task SendMessage(string user, string message)
        {
/*            _logger.LogInformation("userrr ---");
*/            _logger.LogInformation("User: {User} - Message: {Message}", user, message);
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
