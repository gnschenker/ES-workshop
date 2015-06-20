using System;
using System.Threading.Tasks;
using SampleProject.Contracts.Events;
using SampleProject.Domain.Samples;
using SampleProject.Infrastructure;
using SampleProject.ReadModel.Views;

namespace SampleProject.ReadModel.Observers
{
    public class SamplesObserver
    {
        private readonly IProjectionWriter<Guid, SampleView> _writer;

        public SamplesObserver(IProjectionWriter<Guid, SampleView> writer)
        {
            _writer = writer;
        }

        public async Task When(SampleStarted e)
        {
            await _writer.Add(e.Id, new SampleView
            {
                Id = e.Id,
                Name = e.Name,
                Status = SampleStatus.Draft
            });
        }

        public async Task When(Step1Executed e)
        {
            await _writer.Update(e.Id, x =>
            {
                x.Quantity = e.Quantity;
                x.DueDate = e.DueDate;
            });
        }

        public async Task When(SampleApproved e)
        {
            await _writer.Update(e.Id, x =>
            {
                x.Status = SampleStatus.Approved;
            });
        }

        public async Task When(SampleCancelled e)
        {
            await _writer.Update(e.Id, x =>
            {
                x.Status = SampleStatus.Cancelled;
            });
        }
    }}