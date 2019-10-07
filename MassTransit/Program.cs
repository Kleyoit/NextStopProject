using System;
using System.Collections.Generic;
using MassTransit.Core;

namespace MassTransit
{
    class Program
    {
        static void Main(string[] args)
        {
            var routeIds = new List<string> { "ROUTE_1", "ROUTE_2", "ROUTE_3"};
            var scheduleManager = ScheduleManager.Instance;
            scheduleManager.Init(routeIds);
            var requestedTime = new TimeSpan(23,50,0);

            var test = scheduleManager.GetNextArrival("ROUTE_1", "STOP-1", requestedTime);

            Console.ReadLine();
        }
    }
}