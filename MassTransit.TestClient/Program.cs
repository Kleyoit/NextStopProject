using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using MassTransit.Core.HubServiceInterfaces;

namespace MassTransit.TestClient
{
    class Program
    {
        static  async Task Main(string[] args)
        {
            var connection = new HubConnectionBuilder()
                .WithUrl(Strings.HubUrl)
                .Build();

            connection.On<List<ArrivalReponse>>(Strings.Events.NextArrival, (arrivalTimePerRoute) =>
            {
                Console.WriteLine("---------------- NEXT ARRIVAL ----------------");
                foreach (var arrivalRoute in arrivalTimePerRoute)
                {
                    Console.WriteLine( $"ROUTE: {arrivalRoute.RouteName} - Stop:   {arrivalRoute.StopName}  is arriving in {arrivalRoute.NextArrivalTime}  ");
                }
                Console.WriteLine("----------------");
            });

            // Loop is here to wait until the server is running
            while (true)
            {
                try
                {
                    await connection.StartAsync();

                    break;
                }
                catch
                {
                    await Task.Delay(60000);
                }
            }

            Console.WriteLine("Client One listening. Hit Ctrl-C to quit.");
            Console.ReadLine();
        }
    }
}
