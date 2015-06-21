using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using SampleProject.Contracts.Events;
using SampleProject.Infrastructure;
using SampleProject.ReadModel.Observers;
using SampleProject.ReadModel.Views;
using Is = NUnit.Framework.Is;

namespace UnitTests.ReadModel
{
    public class ProjectionWriterMock :IProjectionWriter<Guid, SampleView>
    {
        private readonly Dictionary<Guid, SampleView> _samples = new Dictionary<Guid, SampleView>();

        public ProjectionWriterMock(Dictionary<Guid, SampleView> samples)
        {
            _samples = samples;
        }

        public async Task<SampleView> AddOrUpdate(Guid key, Func<SampleView> addFactory, Func<SampleView, SampleView> update, bool probablyExists = true)
        {
            if (addFactory != null)
            {
                _samples[key] = addFactory();
                return await Task<SampleView>.Run(() => _samples[key]);
            }
            var item = _samples[key];
            return await Task<SampleView>.Run(() => update(item));
        }
    }

    public class SamplesObserverSpecs : SpecificationBase
    {
        protected SamplesObserver Sut;
        protected Dictionary<Guid, SampleView> Samples;

        protected override void Given()
        {
            Samples = new Dictionary<Guid, SampleView>();
            var writer = new ProjectionWriterMock(Samples);
            Sut = new SamplesObserver(writer);
        }
    }

    public class when_handling_sample_created_event : SamplesObserverSpecs
    {
        private SampleStarted _ev;
        private Guid _sampleId;

        protected override void Given()
        {
            base.Given();
            _sampleId = Guid.NewGuid();
            _ev = new SampleStarted
            {
                Id = _sampleId,
                Name = "Foo"
            };
        }

        protected override async void When()
        {
            await Sut.When(_ev);
        }

        [Then]
        public void it_should_create_a_new_item()
        {
            Assert.That(Samples[_sampleId], Is.Not.Null);
            Assert.That(Samples[_sampleId].Id, Is.EqualTo(_ev.Id));
            Assert.That(Samples[_sampleId].Name, Is.EqualTo(_ev.Name));
        }
    }

    public class when_handling_step1_executed_event : SamplesObserverSpecs
    {
        private Guid _sampleId;
        private Step1Executed _ev;
        private DateTime _dueDate;

        protected override void Given()
        {
            base.Given();
            _sampleId = Guid.NewGuid();
            Sut.When(new SampleStarted
            {
                Id = _sampleId,
                Name = "Foo 2"
            }).Wait();
            _dueDate = new DateTime(2015, 5, 20);
            _ev = new Step1Executed
            {
                Id = _sampleId,
                Quantity = 12,
                DueDate = _dueDate
            };
        }

        protected override void When()
        {
            Sut.When(_ev).Wait();
        }

        [Then]
        public void it_should_update_existing_item()
        {
            Assert.That(Samples[_sampleId], Is.Not.Null);
            Assert.That(Samples[_sampleId].Id, Is.EqualTo(_ev.Id));
            Assert.That(Samples[_sampleId].Quantity, Is.EqualTo(_ev.Quantity));
            Assert.That(Samples[_sampleId].DueDate, Is.EqualTo(_ev.DueDate));
        }
    }
}