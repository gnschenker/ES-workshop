using System;
using System.IO;
using System.Net;
using System.Web.Http;
using EventStore.ClientAPI;
using log4net;
using log4net.Config;
using Projects.Domain;
using Projects.Infrastructure;
using Projects.ReadModel.Observers;
using Projects.ReadModel.Providers;
using StructureMap;
using WebApiContrib.IoC.StructureMap;

namespace Projects
{
    public static class Bootstrap
    {
        private static IRepository _repository;
        private static EventsDispatcher _dispatcher;

        public static void Init()
        {
            InitLogging();
            ObjectFactory.Initialize(init =>
            {
                init.For<IRepository>().Use(c => _repository);
                init.For<IApplicationSettings>().Use<ApplicationSettings>();
                init.For<IUniqueKeyGenerator>().Use<UniqueKeyGenerator>();
                init.For<ISampleApplicationService>().Use<SampleApplicationService>();
                init.For<ISamplesProvider>().Use<SamplesProvider>();
                init.For<ILog>().Singleton().Use(c => LogManager.GetLogger("Projects"));
            });
            GlobalConfiguration.Configuration.DependencyResolver =
                new StructureMapResolver(ObjectFactory.Container);

            var applicationSettings = ObjectFactory.GetInstance<IApplicationSettings>();
            InitGetEventStore(applicationSettings);
            InitEventsDispatcher(applicationSettings, ObjectFactory.GetInstance<ILog>());
        }

        private static void InitLogging()
        {
            XmlConfigurator.Configure(new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config")));
        }

        private static void InitGetEventStore(IApplicationSettings applicationSettings)
        {
            var endpoint = GetEventStoreEndpoint(applicationSettings);
            var connection = EventStoreConnection.Create(endpoint);
            connection.ConnectAsync().Wait();
            var factory = new AggregateFactory();
            _repository = new GesRepository(connection, factory);
        }

        private static void InitEventsDispatcher(IApplicationSettings applicationSettings, ILog logger)
        {
            var endpoint = GetEventStoreEndpoint(applicationSettings);
            var connection = EventStoreConnection.Create(endpoint);
            connection.ConnectAsync().Wait();
            _dispatcher = new EventsDispatcher(logger, applicationSettings);
            var factory = new MongoDbAtomicWriterFactory(applicationSettings.MongoDbConnectionString, applicationSettings.MongoDbName);
            var observers = new ObserverRegistry().GetObservers(factory);
            _dispatcher.Start(connection, observers);
        }

        private static IPEndPoint GetEventStoreEndpoint(IApplicationSettings applicationSettings)
        {
            var ipAddress = IPAddress.Parse(applicationSettings.GesIpAddress);
            var endpoint = new IPEndPoint(ipAddress, applicationSettings.GesTcpIpPort);
            return endpoint;
        }
    }
}