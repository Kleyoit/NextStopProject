using System;
using System.Collections.Generic;
using System.Linq;
using RangeTree;
using MassTransit.Core.HubServiceInterfaces;
using System.Collections.Concurrent;

namespace MassTransit.Core
{
    public class ScheduleManager
    {
        private ConcurrentDictionary<string, Dictionary<string, List<TimeSpan>>> _scheduleHub;
        private TimeSpan _dayServiceStartTime;
        private TimeSpan _startTimePerRoute;
        private TimeSpan _dayServiceStopTime;

        private static readonly Lazy<ScheduleManager> lazy =
        new Lazy<ScheduleManager>(() => new ScheduleManager());
        public static ScheduleManager Instance { get { return lazy.Value; } }

        private int _serviceDelayForRoute;
        private int _delayBetweenRoute;
        private int _delayBetweenStop;
        private int _nbOfStop;

        private ScheduleManager()
        {
            _scheduleHub = new ConcurrentDictionary<string, Dictionary<string, List<TimeSpan>>>();

            //those variables could have be setup to be accessible outside of this class but not for this case
            _dayServiceStartTime = new TimeSpan(0, 0, 0);
            _dayServiceStopTime = new TimeSpan(24, 0, 0);
            _startTimePerRoute = new TimeSpan(0, 0, 0);
        }
        public void Init(List<string> Routes, int serviceDelayForRoute = 15, int delayBetweenRoute = 2, int delayBetetweenStop = 2, int nbOfStop = 10)
        {
            _serviceDelayForRoute = serviceDelayForRoute;
            _delayBetweenRoute = delayBetweenRoute;
            _delayBetweenStop = delayBetetweenStop;
            _nbOfStop = nbOfStop;

            ProcessSchedule(Routes);
        }

        private void ProcessSchedule(List<string> Routes)
        {
            if (Routes.Count == 0)
                return;

            AddRoutes(Routes);

            int routeIndex = 0;

            foreach (var route in _scheduleHub)
            {
                //increase start time for any bus/train after processing the first one
                if (routeIndex > 0)
                {
                    _dayServiceStartTime = new TimeSpan(0, 0, 0);
                    _dayServiceStartTime = _dayServiceStartTime.Add(new TimeSpan(0, _delayBetweenRoute, 0));
                    _delayBetweenRoute += 2;
                }

                //each stop
                for (int i = 1; i <= _nbOfStop; i++)
                {
                    route.Value.Add("STOP-" + i, new List<TimeSpan>());

                    if (routeIndex > 1)
                        _startTimePerRoute = _dayServiceStartTime.Add(new TimeSpan(0, 2, 0)); //verify the static 2

                    if (i == 1)
                        _startTimePerRoute = _dayServiceStartTime;

                    if (i > 1)
                    {
                        if (i == 2)
                            _startTimePerRoute = _dayServiceStartTime.Add(new TimeSpan(0, 2, 0));  //verify the static 2

                        if (i >= 1 && i < 11)
                            _startTimePerRoute = _dayServiceStartTime.Add(new TimeSpan(0, _delayBetweenStop, 0));

                        _delayBetweenStop += 2;
                    }
                    //create schedule for stop until daily schedule end
                    while (_startTimePerRoute < _dayServiceStopTime)
                    {
                        route.Value["STOP-" + i].Add(_startTimePerRoute);
                        //change to log
                        Console.WriteLine($" Route: {route.Key} - Stop { "STOP_" + i} -- {_startTimePerRoute.ToString()}");

                        _startTimePerRoute = _startTimePerRoute.Add(new TimeSpan(0, _serviceDelayForRoute, 0));
                    }
                    Console.WriteLine("--");

                    _startTimePerRoute = new TimeSpan(0, 0, 0);
                }
                _delayBetweenStop = 2;
                routeIndex++;

            }
        }

        private void AddRoutes(List<string> Routes)
        {
            foreach (var routeId in Routes)
            {
                _scheduleHub.TryAdd(routeId, new Dictionary<string, List<TimeSpan>>());
            }
        }
        public List<TimeSpan> GetNextArrival(string route, string stop, TimeSpan timeRequested, int nbFutureArrival = 2)
        {
            var tree = new RangeTree<TimeSpan, TimeSpan>();
            var schedule = _scheduleHub[route][stop];
            var arrivalTimes = new List<TimeSpan>();

            if (schedule == null)
                return new List<TimeSpan>();

            foreach (var busStop in schedule)
            {
                var from = busStop.Add(new TimeSpan(0, (-1 * _serviceDelayForRoute), 0));
                var to = busStop.Add(new TimeSpan(0, _serviceDelayForRoute, 0));

                tree.Add(from, to, busStop);
            }

            arrivalTimes = tree.Query(timeRequested).ToList();
            int indexToRemove = 0;
            bool IsPassTime = false;

            //remove past time
            foreach(var time in arrivalTimes)
            {
                if ((time - timeRequested) < new TimeSpan(0, 0, 0))
                {
                    IsPassTime = true;
                    break;
                }
                indexToRemove++;
            }

            if (IsPassTime == true)
            {
                arrivalTimes.RemoveAt(indexToRemove);

                //look for larger arrival marge
                arrivalTimes.Add(tree.Query(timeRequested.Add(new TimeSpan(0,30,0))).FirstOrDefault());
            }

            if (arrivalTimes.Count == 1)
            {
                //Get the first time in the service start 
                arrivalTimes.Add(tree.Query(_dayServiceStartTime).LastOrDefault());
            }
            //because times return in rever order
            arrivalTimes.Sort();

            return arrivalTimes;
        }
    }
}

