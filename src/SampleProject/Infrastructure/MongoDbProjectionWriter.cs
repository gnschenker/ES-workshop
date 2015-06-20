﻿using System;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace SampleProject.Infrastructure
{
    public class MongoDbProjectionWriter<TId, T> : IProjectionWriter<TId, T>
           where T : class
    {
        private readonly string _connectionString;
        private readonly string _databaseName;
        private static IMongoCollection<T> _collection;

        public MongoDbProjectionWriter(string connectionString, string databaseName)
        {
            _connectionString = connectionString;
            _databaseName = databaseName;
        }

        public async Task Add(TId id, T item)
        {
            var collection = GetCollection();
            await collection.InsertOneAsync(item);
        }

        public async Task Update(TId id, Action<T> update)
        {
            var builder = Builders<T>.Filter;
            var filter = builder.Eq("_id", id);
            var collection = GetCollection();
            var existingItem = await collection.Find(filter).FirstOrDefaultAsync();

            if (existingItem == null)
                throw new InvalidOperationException("Item does not exists");

            update(existingItem);
            await collection.ReplaceOneAsync(filter, existingItem);
        }

        private IMongoCollection<T> GetCollection()
        {
            if (_collection != null) return _collection;
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_databaseName);
            _collection = database.GetCollection<T>(typeof(T).Name);
            return _collection;
        }
    }
}