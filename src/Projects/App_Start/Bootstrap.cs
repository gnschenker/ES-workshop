using System.Configuration;
using System.Net;
using System.Web.Http;
using EventStore.ClientAPI;
using log4net;
using Projects.Domain;
using Projects.Infrastructure;
using Projects.ReadModel.Observers;
using StructureMap;
using WebApiContrib.IoC.StructureMap;

namespace Projects
{
    public static class Bootstrap
    {
        private static IRepository _repository;
        private static ILog _log;

        public static void Init()
        {
            var endpoint = new IPEndPoint(IPAddress.Loopback, 1113);
            var connection = EventStoreConnection.Create(endpoint);
            connection.ConnectAsync().Wait();
            var factory = new AggregateFactory();
            _repository = new GesRepository(connection, factory);

            ObjectFactory.Initialize(init =>
            {
                init.For<IRepository>().Use(c => _repository);
                init.For<IApplicationSettings>().Use<ApplicationSettings>();
                init.For<IUniqueKeyGenerator>().Use<UniqueKeyGenerator>();
                init.For<ISampleApplicationService>().Use<SampleApplicationService>();
                init.For<ILog>().Use(c => LogManager.GetLogger("Projects"));
            });
            GlobalConfiguration.Configuration.DependencyResolver =
                new StructureMapResolver(ObjectFactory.Container);

            InitEventsDispatcher(endpoint, ObjectFactory.GetInstance<IApplicationSettings>());
        }

        private static void InitEventsDispatcher(IPEndPoint endpoint, IApplicationSettings settings)
        {
            var connection = EventStoreConnection.Create(endpoint);
            connection.ConnectAsync().Wait();
            var dispatcher = new EventsDispatcher(_log);
            var factory = new MongoDbAtomicWriterFactory(settings.MongoDbConnectionString, settings.MongoDbName);
            var observers = new ObserverRegistry().GetObservers(factory);
            dispatcher.Start(connection, observers);
        }
    }

    public interface IApplicationSettings
    {
        string MongoDbConnectionString { get; }
        string MongoDbName { get; }
    }

    public class ApplicationSettings : IApplicationSettings
    {
        public string MongoDbConnectionString { get; private set; }
        public string MongoDbName { get; private set; }
    }
}