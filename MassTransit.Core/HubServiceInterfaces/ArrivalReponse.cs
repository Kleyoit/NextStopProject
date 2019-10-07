using System;

namespace MassTransit.Core.HubServiceInterfaces
{
    public class ArrivalReponse
    {
        public string RouteName { get; set; }
        public string StopName { get; set; }
        public string NextArrivalTime { get; set; }
    }
}
