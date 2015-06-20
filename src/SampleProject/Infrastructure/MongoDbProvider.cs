using System;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace SampleProject.Infrastructure
{
    public abstract class MongoDbProvider<T> : IProvider<T>
    {
        private static IMongoCollection<T> _collection;
        private readonly IApplicationSettings _applicationSettings;

        protected MongoDbProvider(IApplicationSettings applicationSettings)
        {
            _applicationSettings = applicationSettings;
        }

        protected IMongoCollection<T> GetCollection()
        {
            if (_collection != null) return _collection;
            var client = new MongoClient(_applicationSettings.MongoDbConnectionString);
            var database = client.GetDatabase(_applicationSettings.MongoDbName);
            _collection = database.GetCollection<T>(typeof(T).Name);
            return _collection;
        }

        public async Task<T> GetById(Guid id)
        {
            var builder = Builders<T>.Filter;
            var filter = builder.Eq("_id", id);
            var collection = GetCollection();
            var existingItem = await collection.Find(filter).FirstOrDefaultAsync();
            return existingItem;
        }
    }
}