using System.Net;
using System.Threading;
using EventStore.ClientAPI;
using log4net;
using NUnit.Framework;
using Projects.Infrastructure;
using Projects.ReadModel.Observers;
using Rhino.Mocks;

namespace UnitTests
{
    public class when_dispatching_events
    {
        private const string connectionString = "mongodb://localhost:27017";
        private const string dbName = "Samples";

        [Test]
        public void it_should_work()
        {
            var endpoint = new IPEndPoint(IPAddress.Loopback, 1113);
            var connection = EventStoreConnection.Create(endpoint);
            connection.ConnectAsync().Wait();
            var factory = new MongoDbAtomicWriterFactory(connectionString, dbName);
            var observers = new ObserverRegistry().GetObservers(factory);
            var dispatcher = new EventsDispatcher(MockRepository.GenerateMock<ILog>());
            dispatcher.Start(connection, observers);
            Thread.Sleep(5000);
        }
    }
}