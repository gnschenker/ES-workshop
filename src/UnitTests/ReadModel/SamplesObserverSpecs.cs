using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using Projects;
using Projects.Contracts.Events;
using Projects.Infrastructure;
using Projects.ReadModel.Observers;
using Projects.ReadModel.Providers;
using Projects.ReadModel.Views;
using Rhino.Mocks;
using Is = NUnit.Framework.Is;

namespace UnitTests.ReadModel
{
    public class SamplesObserverSpecs : SpecificationBase
    {
        protected SamplesObserver Sut;
        protected Dictionary<Guid, SampleView> Samples;

        protected override void Given()
        {
            Samples = new Dictionary<Guid, SampleView>();
            var writer = MockRepository.GenerateMock<IProjectionWriter<SampleView>>();
            writer.Stub(x => x.Add(Arg<Guid>.Is.Anything, Arg<SampleView>.Is.Anything))
                .WhenCalled(mi => Samples[(Guid)mi.Arguments[0]] = (SampleView)mi.Arguments[1])
                .Return(Task.Delay(0));
            writer.Stub(x => x.Update(Arg<Guid>.Is.Anything, Arg<Action<SampleView>>.Is.Anything))
                .WhenCalled(mi =>
                {
                    var sample = Samples[(Guid) mi.Arguments[0]];
                    ((Action<SampleView>) mi.Arguments[1])(sample);
                })
                .Return(Task.Delay(0));
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

        protected override async void Given()
        {
            base.Given();
            _sampleId = Guid.NewGuid();
            await Sut.When(new SampleStarted
            {
                Id = _sampleId,
                Name = "Foo 2"
            });
            _dueDate = new DateTime(2015, 5, 20);
            _ev = new Step1Executed
            {
                Id = _sampleId,
                Quantity = 12,
                DueDate = _dueDate
            };
        }

        protected override async void When()
        {
            await Sut.When(_ev);
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