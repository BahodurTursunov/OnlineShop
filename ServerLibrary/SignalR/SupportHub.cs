using Microsoft.AspNetCore.SignalR;

namespace ServerLibrary.SignalR
{
    public class ChatHub : Hub
    {
        public async Task Send(string message, string userName)
        {
            await Clients.All.SendAsync("Receive", message, userName);
        }

        /*  public override async Task OnConnectedAsync()
          {
              // Отправляем сообщение всем клиентам о новом подключении
              await Clients.All.SendAsync("Notify", $"{Context.ConnectionId} вошел в час.");
              await base.OnConnectedAsync();
          }*/

        public override async Task OnConnectedAsync()
        {
            var context = Context.GetHttpContext();
            if (context != null)
            {
                if (context.Request.Cookies.ContainsKey("name"))
                {
                    if (context.Request.Cookies.TryGetValue("name", out var userName))
                    {
                        Console.WriteLine($"name = {userName}");
                    }
                }
                Console.WriteLine($"UserAgent = {context.Request.Headers["User-Agent"]}");
                Console.WriteLine($"RemoteIpAddress = {context.Connection?.RemoteIpAddress?.ToString()}");
                await base.OnConnectedAsync();
            }
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            // Отправляем сообщение всем клиентам о отключении
            await Clients.All.SendAsync("Notify", $"{Context.ConnectionId} вышел из чата.");
            await base.OnDisconnectedAsync(ex);
        }
    }
}
