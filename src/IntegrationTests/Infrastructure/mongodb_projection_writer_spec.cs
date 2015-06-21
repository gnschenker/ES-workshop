using System;
using MongoDB.Driver;
using NUnit.Framework;
using SampleProject.Infrastructure;

namespace IntegrationTests.Infrastructure
{
    public class mongodb_projection_writer_spec<TId, T> : SpecificationBase
        where T: class
    {
        protected MongoDbProjectionWriter<TId, T> sut;
        protected Guid id;
        protected const string connectionString = @"mongodb://localhost:27017";
        protected const string databaseName = @"~~integration-tests~~";

        protected override void Given()
        {
            sut = new MongoDbProjectionWriter<TId, T>(connectionString, databaseName);
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
    public class when_adding_a_new_item_to_mongo_collection : mongodb_projection_writer_spec<Guid, Foo>
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
        : mongodb_projection_writer_spec<string, Bar>
    {
        private Bar bar;
        private readonly string species = "Mouse" + Guid.NewGuid();

        protected override void Given()
        {
            base.Given();
            bar = new Bar { Id = species, AverageWeight = 1.25m, Counter = 12 };
        }

        protected override void When()
        {
            sut.Add(species, bar).Wait();
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

    [Explicit("Environment dependent")]
    public class when_update_enforce_new_item_to_mongo_collection
        : mongodb_projection_writer_spec<string, Baz>
    {
        private readonly string species = "Mouse" + Guid.NewGuid();

        protected override void When()
        {
            sut.UpdateEnforcingNew(species, b =>
            {
                b.Id = species;
                b.AverageWeight += 0.25m;
                b.Counter++;
            }).Wait();
            sut.UpdateEnforcingNew(species, b =>
            {
                b.Id = species;
                b.AverageWeight += 0.25m;
                b.Counter++;
            }).Wait();
        }

        [Then]
        public void it_should_work()
        {
            var coll = GetCollection();
            var builder = Builders<Baz>.Filter;
            var filter = builder.Eq("_id", species);
            var item = coll.Find(filter).FirstOrDefaultAsync().Result;
            Assert.That(item, Is.Not.Null);
            Assert.That(item.Id, Is.EqualTo(species));
            Assert.That(item.AverageWeight, Is.EqualTo(0.5m));
            Assert.That(item.Counter, Is.EqualTo(2));
        }
    }
}