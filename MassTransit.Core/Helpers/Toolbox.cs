using System;
using System.Collections.Generic;
using MassTransit.Core.HubServiceInterfaces;

namespace MassTransit.Core.Helpers
{
    public static class Toolbox
    {
        public static List<ArrivalReponse> GetArrivalTime(List<TimeSpan> nextArrivals, string routeName, string stopName, TimeSpan requestedTime)
        {
            var arrivalReponses = new List<ArrivalReponse>();

            foreach (var time in nextArrivals)
            {
                arrivalReponses.Add(new ArrivalReponse
                {
                    RouteName = routeName,
                    StopName = stopName,
                    NextArrivalTime = (time - requestedTime).ToString()
                });
            }

            return arrivalReponses;
        }
    }
}
