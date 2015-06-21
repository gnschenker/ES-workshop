using System;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using NUnit.Framework;
using SampleProject.Infrastructure;

namespace IntegrationTests.Infrastructure
{
    /*
     Script to generate table Foo and Baz
     Note: Primary key is expected to be named "Id"
     
     create table Foo(
        Id uniqueidentifier not null,
        Name nvarchar(50) not null,
        DueDate datetime,
        Counter int,
        constraint pk_foo primary key(id)
    )
    GO
     
     create table Baz(
        Id nvarchar(50) not null,
        AverageWeight decimal,
        Counter int,
        constraint pk_baz primary key(id)
    )
    GO
    */
    public class sql_projection_writer_spec : SpecificationBase
    {
        protected SqlServerProjectionWriter<Guid, Foo> sut;
        protected Foo foo;
        protected Guid id;
        protected const string connectionString = @"server=.\SQLEXPRESS2012;database=test;integrated security=true";

        protected override void Given()
        {
            sut = new SqlServerProjectionWriter<Guid, Foo>(connectionString);
            id = Guid.NewGuid();
            foo = new Foo { Id = id, Name = "Foo name", DueDate = DateTime.Now, Counter = 4 };
        }
    }

    [Explicit("Environment dependent")]
    public class when_adding_a_new_item_to_sql_table : sql_projection_writer_spec
    {
        protected override void When()
        {
            sut.Add(id, foo).Wait();
        }

        [Then]
        public void it_should_work()
        {
            using (var conn = new SqlConnection(connectionString))
            {
                var item = conn.Query<Foo>(string.Format("Select * from Foo where id='{0}'", id)).SingleOrDefault();
                Assert.That(item, Is.Not.Null);
                Assert.That(item.Name, Is.EqualTo(foo.Name));
                Assert.That(item.DueDate.ToShortDateString(), Is.EqualTo(foo.DueDate.ToShortDateString()));
                Assert.That(item.DueDate.ToShortTimeString(), Is.EqualTo(foo.DueDate.ToShortTimeString()));
                Assert.That(item.Counter, Is.EqualTo(foo.Counter));
            }
        }
    }

    [Explicit("Environment dependent")]
    public class when_updating_an_existing_item_in_sql_table : sql_projection_writer_spec
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
            using (var conn = new SqlConnection(connectionString))
            {
                var item = conn.Query<Foo>(string.Format("Select * from Foo where id='{0}'", id)).SingleOrDefault();
                Assert.That(item, Is.Not.Null);
                Assert.That(item.Name, Is.EqualTo(newName));
                Assert.That(item.DueDate.ToShortDateString(), Is.EqualTo(newDueDate.ToShortDateString()));
                Assert.That(item.DueDate.ToShortTimeString(), Is.EqualTo(newDueDate.ToShortTimeString()));
                Assert.That(item.Counter, Is.EqualTo(newCounter));
            }
        }
    }

    [Explicit("Environment dependent")]
    public class when_update_enforce_new_item_to_sql_server
        : sql_projection_writer_spec
    {
        private readonly string _species = "Mouse" + Guid.NewGuid();
        private SqlServerProjectionWriter<string, Baz2> _sut;
        protected override void Given()
        {
            base.Given();
            _sut = new SqlServerProjectionWriter<string, Baz2>(connectionString);
        }

        protected override void When()
        {
            _sut.UpdateEnforcingNew(_species, b =>
            {
                b.Id = _species;
                b.AverageWeight += 0.25;
                b.Counter++;
            }).Wait();
            _sut.UpdateEnforcingNew(_species, b =>
            {
                b.Id = _species;
                b.AverageWeight += 0.25;
                b.Counter++;
            }).Wait();
        }

        [Then]
        public void it_should_work()
        {
            using (var conn = new SqlConnection(connectionString))
            {
                var item = conn.Query<Baz2>(string.Format("Select * from Baz2 where id='{0}'", _species)).SingleOrDefault();
                Assert.That(item, Is.Not.Null);
                Assert.That(item.Id, Is.EqualTo(_species));
                Assert.That(item.Counter, Is.EqualTo(2));
                Assert.That(item.AverageWeight, Is.EqualTo(0.5));
            }
        }
    }
}