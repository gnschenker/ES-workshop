using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using log4net;

namespace SampleProject.Infrastructure
{
    public class EventsDispatcher
    {
        private readonly ILog _log;
        private readonly IApplicationSettings _applicationSettings;
        private Dictionary<Type, Info[]> _dictionary = new Dictionary<Type, Info[]>();
        private EventStoreAllCatchUpSubscription _subscription;

        public EventsDispatcher(ILog log, IApplicationSettings applicationSettings)
        {
            _log = log;
            _applicationSettings = applicationSettings;
        }

        public void Start(IEventStoreConnection connection, IEnumerable<object> observers)
        {
            WireUpObservers(observers);
            var credentials = new UserCredentials(_applicationSettings.GesUserName, _applicationSettings.GesPassword);
            var lastCheckpoint = Position.Start;

            _subscription = connection.SubscribeToAllFrom(lastCheckpoint, false,
                EventAppeared,
                OnLiveProcessingStarted,
                OnSubscriptionDropped,
                credentials
                );
        }

        private void OnLiveProcessingStarted(EventStoreCatchUpSubscription subscription)
        {
            _log.Debug("Live processing of events started");
        }

        private void OnSubscriptionDropped(EventStoreCatchUpSubscription subscription, SubscriptionDropReason reason, Exception exception)
        {
            _log.Error(string.Format("Event subscription stopped. Reason: {0}", reason), exception);
        }

        private void EventAppeared(EventStoreCatchUpSubscription subscription, ResolvedEvent re)
        {
            if (re.OriginalEvent.EventType.StartsWith("$")) return; //skip internal events
            if (re.OriginalEvent.Metadata == null || re.OriginalEvent.Metadata.Any() == false) return;
            try
            {
                var e = re.DeserializeEvent();
                Dispatch(e).Wait();
            }
            catch (Exception exception)
            {
                _log.Error(string.Format("Could not deserialize event {0}", re.OriginalEvent.EventType), exception);
            }
        }

        private async Task Dispatch(object e)
        {
            var eventType = e.GetType();
            if (_dictionary.ContainsKey(eventType) == false)
                return;

            foreach (var item in _dictionary[eventType])
            {
                try
                {
                    await ((Task)item.MethodInfo.Invoke(item.Observer, new[] { e }));
                }
                catch (Exception ex)
                {
                    _log.Error(string.Format("Could not dispatch event {0} to projection {1}", 
                        eventType.Name, item.Observer.GetType().Name), ex);
                }
            }
        }

        private void WireUpObservers(IEnumerable<object> projections)
        {
            _dictionary = projections.Select(p => new { Projection = p, Type = p.GetType() })
                .Select(x => new
                {
                    x.Projection,
                    MethodInfos = x.Type
                        .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                        .Where(m => m.Name == "When" && m.GetParameters().Count() == 1)
                })
                .SelectMany(x => x.MethodInfos.Select(
                    y => new { x.Projection, MethodInfo = y, y.GetParameters().First().ParameterType }))
                .GroupBy(x => x.ParameterType)
                .ToDictionary(g => g.Key,
                    g => g.Select(y => new Info { Observer = y.Projection, MethodInfo = y.MethodInfo }).ToArray());
        }

        public class Info
        {
            public MethodInfo MethodInfo;
            public object Observer;
        }
    }
}