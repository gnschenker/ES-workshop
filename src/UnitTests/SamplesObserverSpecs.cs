using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Projects.Contracts.Events;
using Projects.Infrastructure;
using Projects.ReadModel.Observers;
using Projects.ReadModel.Views;

namespace UnitTests
{
    public class SamplesObserverSpecs : SpecificationBase
    {
        protected SamplesObserver Sut;

        protected override void Given()
        {
            var writer = new MongoDbProjectionWriter<SampleView>("mongodb://localhost:27017", "Samples");
            Sut = new SamplesObserver(writer);
        }
    }

    public class when_handling_sample_created_event : SamplesObserverSpecs
    {
        private SampleStarted e;

        protected override void Given()
        {
            base.Given();
            e = new SampleStarted
            {
                Id = 1,
                Name = "Foo"
            };
        }

        protected override void When()
        {
        }

        [Test]
        public async void it_should_work()
        {
            await Sut.When(e);
            Assert.True(true);
        }

        [Test]
        public async void it_should_update_existing_item()
        {
            await Sut.When(new SampleStarted
            {
                Id = 2,
                Name = "Foo 2"
            });
            await Sut.When(new Step1Executed
            {
                Id = 2,
                Quantity = 12,
                DueDate = new DateTime(2015, 5, 20)
            });
            Assert.True(true);
        }
    }
}