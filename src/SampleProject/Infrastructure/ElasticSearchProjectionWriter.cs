using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SampleProject.Infrastructure
{
    public class ElasticSearchProjectionWriter<TId, T> : IProjectionWriter<TId, T> where T : class
    {
        private readonly string _baseUrl;
        private readonly HttpClient _client;

        public ElasticSearchProjectionWriter(string baseUrl)
        {
            _baseUrl = string.Format("{0}/{1}", baseUrl, typeof(T).Name.ToLower());
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<T> AddOrUpdate(TId key, Func<T> addFactory, Func<T, T> update, bool probablyExists = true)
        {
            if (probablyExists)
            {
                return await Update(key, addFactory, update);
            }
            var item = addFactory();
            await Add(key, item);
            return item;
        }

        private async Task Add(TId id, T item)
        {
            var uri = GetUri(id);
            await PutItem(uri, item);
        }

        private async Task<T> Update(TId id, Func<T> addFactory, Func<T, T> update)
        {
            var uri = GetUri(id);
            T item = null;
            try
            {
                var body = _client.GetStringAsync(uri).Result;
                var indexItem = JsonConvert.DeserializeObject<IndexItem<T>>(body);
                item = indexItem._source;
            }
            catch { }
            if (item != null)
            {
                update(item);
                await PutItem(uri, item);
                return item;
            }

            if (addFactory == null)
                throw new InvalidOperationException("Item does not exists");

            item = addFactory();
            update(item);
            await PutItem(uri, item);
            return item;
        }

        private Uri GetUri(TId id)
        {
            return new Uri(string.Format("{0}/external/{1}?pretty", _baseUrl, id));
        }

        private async Task PutItem(Uri uri, T item)
        {
            var json = JsonConvert.SerializeObject(item);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await _client.PutAsync(uri, content);
        }
    }

    public class IndexItem<T>
    {
        public T _source { get; set; }
    }
}