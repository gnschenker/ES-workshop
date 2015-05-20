using System.Collections.Generic;
using Projects.Infrastructure;
using Projects.ReadModel.Views;

namespace Projects.ReadModel.Observers
{
    public class ObserverRegistry
    {
        public IEnumerable<object> GetObservers(IProjectionWriterFactory factory)
        {
            yield return new SamplesObserver(factory.GetProjectionWriter<SampleView>());
        }
    }
}