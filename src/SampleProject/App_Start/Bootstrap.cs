using System;
using System.IO;
using System.Net;
using System.Web.Http;
using EventStore.ClientAPI;
using log4net;
using log4net.Config;
using SampleProject.Domain;
using SampleProject.Domain.Samples;
using SampleProject.Infrastructure;
using SampleProject.ReadModel.Observers;
using SampleProject.ReadModel.Providers;
using StructureMap;
using WebApiContrib.IoC.StructureMap;

namespace SampleProject
{
    public static class Bootstrap
    {
        private static IRepository _repository;
        private static EventsDispatcher _dispatcher;

        public static void Init()
        {
            InitLogging();
            InitContainer();
            InitMetrics();

            GlobalConfiguration.Configuration.DependencyResolver =
                new StructureMapResolver(ObjectFactory.Container);

            var applicationSettings = ObjectFactory.GetInstance<IApplicationSettings>();
            InitGetEventStore(applicationSettings);
            InitEventsDispatcher(applicationSettings, ObjectFactory.GetInstance<ILog>());
        }

        private static void InitMetrics()
        {
            //**** Sample code how to enable metrics collection
            //var metricsConfig = new MetricsConfig
            //{
            //    StatsdServerName = "statsd.hostedgraphite.com",
            //    Prefix = "9f05a9e6-ebc5-49bd-90fa-0c8689e7fbbf.CM.Workshop.DEV.IterationZero"
            //};
            //Metrics.Configure(metricsConfig);
        }

        private static void InitContainer()
        {
            ObjectFactory.Initialize(init =>
            {
                init.For<IRepository>().Use(c => _repository);
                init.For<IApplicationSettings>().Use<ApplicationSettings>();
                init.For<IUniqueKeyGenerator>().Use<UniqueKeyGenerator>();
                init.For<ISampleApplicationService>().Use<SampleApplicationService>();
                init.For<ISamplesProvider>().Use<SamplesProvider>();
                init.For<ILog>().Singleton().Use(c => LogManager.GetLogger("SampleProject"));
            });
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

            // use MongoDB for the read model
            var factory = new MongoDbAtomicWriterFactory(applicationSettings.MongoDbConnectionString, applicationSettings.MongoDbName);

            // if you want to use SQL Server for the read model then use this factory instead
            //var factory = new SqlServerAtomicWriterFactory(applicationSettings.SqlServerConnectionString);

            // if you want to use ElasticSearch for the read model then use this factory instead
            //var factory = new ElasticSearchAtomicWriterFactory(applicationSettings.ElasticSearchBaseUrl);

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