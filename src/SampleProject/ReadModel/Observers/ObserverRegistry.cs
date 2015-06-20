using System;
using System.Collections.Generic;
using SampleProject.Infrastructure;
using SampleProject.ReadModel.Views;

namespace SampleProject.ReadModel.Observers
{
    public class ObserverRegistry
    {
        public IEnumerable<object> GetObservers(IProjectionWriterFactory factory)
        {
            yield return new SamplesObserver(factory.GetProjectionWriter<Guid, SampleView>());
        }
    }
}