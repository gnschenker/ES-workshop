using System;
using MongoDB.Driver;
using NUnit.Framework;
using SampleProject.Infrastructure;

namespace IntegrationTests.Infrastructure
{
    public class mongodb_projection_writer_spec<T> : SpecificationBase
        where T: class
    {
        protected MongoDbProjectionWriter<Guid, T> sut;
        protected Guid id;
        protected const string connectionString = @"mongodb://localhost:27017";
        protected const string databaseName = @"~~integration-tests~~";

        protected override void Given()
        {
            sut = new MongoDbProjectionWriter<Guid, T>(connectionString, databaseName);
            id = Guid.NewGuid();
        }

        protected IMongoCollection<T> GetCollection()
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            var collection = database.GetCollection<T>(typeof(T).Name);
            return collection;
        }
    }

    [Explicit("Environment dependent")]
    public class when_adding_a_new_item_to_mongo_collection : mongodb_projection_writer_spec<Foo>
    {
        private Foo foo;

        protected override void Given()
        {
            base.Given();
            foo = new Foo { Id = id, Name = "Foo name", DueDate = DateTime.UtcNow, Counter = 4 };
        }

        protected override void When()
        {
            sut.Add(id, foo).Wait();
        }

        [Then]
        public void it_should_work()
        {
            var coll = GetCollection();
            var builder = Builders<Foo>.Filter;
            var filter = builder.Eq("_id", id);
            var item = coll.Find(filter).FirstOrDefaultAsync().Result;
            Assert.That(item, Is.Not.Null);
            Assert.That(item.Name, Is.EqualTo(foo.Name));
            Assert.That(item.DueDate.ToShortDateString(), Is.EqualTo(foo.DueDate.ToShortDateString()));
            Assert.That(item.DueDate.ToShortTimeString(), Is.EqualTo(foo.DueDate.ToShortTimeString()));
            Assert.That(item.Counter, Is.EqualTo(foo.Counter));
        }
    }

    [Explicit("Environment dependent")]
    public class when_adding_a_new_item_with_id_of_type_string_to_mongo_collection 
        : mongodb_projection_writer_spec<Bar>
    {
        private Bar bar;
        private const string species = "Mouse";

        protected override void Given()
        {
            base.Given();
            bar = new Bar { Id = species, AverageWeight = 1.25m, Counter = 12 };
        }

        [TearDown]
        public void TearDown()
        {
            GetCollection().Database.DropCollectionAsync("Bar").Wait();
        }

        protected override void When()
        {
            sut.Add(id, bar).Wait();
        }

        [Then]
        public void it_should_work()
        {
            var coll = GetCollection();
            var builder = Builders<Bar>.Filter;
            var filter = builder.Eq("_id", species);
            var item = coll.Find(filter).FirstOrDefaultAsync().Result;
            Assert.That(item, Is.Not.Null);
            Assert.That(item.Id, Is.EqualTo(species));
            Assert.That(item.AverageWeight, Is.EqualTo(bar.AverageWeight));
            Assert.That(item.Counter, Is.EqualTo(bar.Counter));
        }
    }

    public class Bar
    {
        public string Id { get; set; }
        public decimal AverageWeight { get; set; }
        public int Counter { get; set; }
    }
}