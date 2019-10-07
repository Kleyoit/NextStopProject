using System;

namespace MassTransit.Core.HubServiceInterfaces
{
    //TODO : change this variable name
    public static class Strings
    {
        public static string HubUrl => "https://localhost:5001/hubs/clock";

        public static class Events
        {
            public static string NextArrival => nameof(IClock.ShowNextArrivateTime);
        }
    }
}
