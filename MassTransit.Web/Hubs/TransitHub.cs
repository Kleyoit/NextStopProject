using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using MassTransit.Core.HubServiceInterfaces;

namespace MassTransit.Web.Hubs
{
    public class TransitHub : Hub<ITransitClient>
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.ReceiveMessage(user, message);
        }

        public Task SendMessageToCaller(string message)
        {
            return Clients.Caller.ReceiveMessage(message);
        }

        public Task SendMessageToGroups(string message)
        {
            return Clients.Group("SignalR Users").ReceiveMessage(message);
        }

        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR Users");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalR Users");
            await base.OnDisconnectedAsync(exception);
        }
    }

    public class ClockHub : Hub<IClock>
    {
        public async Task SendNextArrival(List<ArrivalReponse> nextArrival)
        {
            await Clients.All.ShowNextArrivateTime(nextArrival);
        }
    }
}
