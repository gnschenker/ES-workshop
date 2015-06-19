using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using NUnit.Framework;
using Projects.Infrastructure;

namespace IntegrationTests.Infrastructure
{
    public class elastic_search_projection_writer_spec : SpecificationBase
    {
        protected ElasticSearchProjectionWriter<Foo> sut;
        protected Foo foo;
        protected Guid id;
        protected const string baseUrl = "http://localhost:9200";

        protected override void Given()
        {
            sut = new ElasticSearchProjectionWriter<Foo>(baseUrl);
            id = Guid.NewGuid();
            foo = new Foo { Id = id, Name = "Foo name", DueDate = DateTime.Now, Counter = 4 };
        }

        protected Foo GetItem()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var uri = new Uri(string.Format("{0}/foo/external/{1}?pretty", baseUrl, id));
            var body = client.GetStringAsync(uri).Result;
            var indexItem = JsonConvert.DeserializeObject<IndexItem<Foo>>(body);
            var item = indexItem._source;
            return item;
        }
    }

    [Explicit("Environment dependent")]
    public class when_adding_a_new_item_to_es_projection : elastic_search_projection_writer_spec
    {
        protected override void When()
        {
            sut.Add(id, foo).Wait();
        }

        [Then]
        public void it_should_add_item_to_index()
        {
            var item = GetItem();
            Assert.That(item, Is.Not.Null);
            Assert.That(item.Name, Is.EqualTo(foo.Name));
            Assert.That(item.DueDate.ToShortDateString(), Is.EqualTo(foo.DueDate.ToShortDateString()));
            Assert.That(item.DueDate.ToShortTimeString(), Is.EqualTo(foo.DueDate.ToShortTimeString()));
            Assert.That(item.Counter, Is.EqualTo(foo.Counter));
        }
    }

    [Explicit("Environment dependent")]
    public class when_updating_an_existing_item_in_es_projection : elastic_search_projection_writer_spec
    {
        private DateTime newDueDate;
        private string newName;
        private int newCounter;

        protected override void Given()
        {
            base.Given();
            sut.Add(id, foo).Wait();
            newName = foo.Name + " changed";
            newCounter += 3;
            newDueDate = foo.DueDate.AddDays(1).AddHours(2);
        }

        protected override void When()
        {
            sut.Update(foo.Id, x =>
            {
                x.Name = newName;
                x.DueDate = newDueDate;
                x.Counter = newCounter;
            }).Wait();
        }

        [Then]
        public void it_should_work()
        {
            var item = GetItem();
            Assert.That(item, Is.Not.Null);
            Assert.That(item.Name, Is.EqualTo(newName));
            Assert.That(item.DueDate.ToShortDateString(), Is.EqualTo(newDueDate.ToShortDateString()));
            Assert.That(item.DueDate.ToShortTimeString(), Is.EqualTo(newDueDate.ToShortTimeString()));
            Assert.That(item.Counter, Is.EqualTo(newCounter));
        }
    }
}