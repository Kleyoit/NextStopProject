using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MassTransit.Core.HubServiceInterfaces;
using MassTransit.Web;
using MassTransit.Core;
using MassTransit.Core.Helpers;

namespace MassTransit.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransitController : ControllerBase
    {
        private readonly ILogger<TransitController> _logger;
        private readonly ScheduleManager _scheduleManager;

        public TransitController(ILogger<TransitController> logger)
        {
            _logger = logger;
            _scheduleManager = ScheduleManager.Instance;
        }

        [HttpGet]
        public IEnumerable<ArrivalReponse> Get(string routeName, string stopName, string timeRequested, int nbFutureArrival)
        {
            var rng = new Random();
            TimeSpan timeRequestFromString = new TimeSpan();
            TimeSpan.TryParse(timeRequested, out timeRequestFromString);

            var routesRequested = _scheduleManager.GetNextArrival(routeName, stopName, timeRequestFromString);
            var reponseArrivals = Toolbox.GetArrivalTime(routesRequested, routeName, stopName, timeRequestFromString);
            return reponseArrivals;
        }
    }
}
