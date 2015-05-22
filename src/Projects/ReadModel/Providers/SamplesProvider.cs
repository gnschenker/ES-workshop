using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Projects.Infrastructure;
using Projects.ReadModel.Views;

namespace Projects.ReadModel.Providers
{
    public interface ISamplesProvider : IProvider<SampleView>
    {
        Task<IEnumerable<SampleView>> Get(string name);
    }

    public class SamplesProvider : MongoDbProvider<SampleView>, ISamplesProvider
    {
        public SamplesProvider(IApplicationSettings applicationSettings) : base(applicationSettings)
        {
        }

        public async Task<IEnumerable<SampleView>> Get(string name)
        {
            var collection = GetCollection();
            var items = await collection.Find(x => x.Name.StartsWith(name)).ToListAsync();
            return items;
        }
    }
}