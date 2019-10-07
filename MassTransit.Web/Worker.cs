using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MassTransit.Core.HubServiceInterfaces;
using MassTransit.Web.Hubs;
using MassTransit.Core;
using MassTransit.Core.Helpers;

namespace MassTransit.Web
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IHubContext<ClockHub, IClock> _clockHub;
        private readonly ScheduleManager _scheduleManager;
        private readonly List<string> _routes;

        public Worker(ILogger<Worker> logger, IHubContext<ClockHub, IClock> clockHub)
        {
            _logger = logger;
            _clockHub = clockHub;
             _routes = new List<string> { "ROUTE_1", "ROUTE_2", "ROUTE_3" };
            // _scheduleManager = ScheduleManager.Instance;
            _scheduleManager = ScheduleManager.Instance;
             _scheduleManager.Init(_routes);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            List<ArrivalReponse> arrivaGroupedlReponses = new List<ArrivalReponse>();
            //I can make the calls inside this loop more elegant but...
            //routes and stop can be read from config file or service at runtime
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {Time}", DateTime.Now);

                var requestedTime = DateTime.Now.TimeOfDay;
                
                foreach (var route in _routes)
                {
                    var route1Stop1Arrivals = _scheduleManager.GetNextArrival(route, "STOP-1", requestedTime);
                    var reponseArrivals = Toolbox.GetArrivalTime(route1Stop1Arrivals, route, "STOP-1", requestedTime);
                    arrivaGroupedlReponses.AddRange(reponseArrivals);

                    var route1Stop1Arrivals1 = _scheduleManager.GetNextArrival(route, "STOP-2", requestedTime);
                    var reponseArrivals1 = Toolbox.GetArrivalTime(route1Stop1Arrivals1, route, "STOP-2", requestedTime);
                    arrivaGroupedlReponses.AddRange(reponseArrivals1);

                    await _clockHub.Clients.All.ShowNextArrivateTime(arrivaGroupedlReponses);
                    arrivaGroupedlReponses = new List<ArrivalReponse>();
                }
                
                await Task.Delay(60000);//a minutes delay
            }
        }
       
    }
}
