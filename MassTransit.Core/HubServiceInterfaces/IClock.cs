using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MassTransit.Core.HubServiceInterfaces
{
    public interface IClock
    {
        Task ShowNextArrivateTime(List<ArrivalReponse> arrivalTime);
    }
}
