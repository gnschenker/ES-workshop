using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Projects.Infrastructure
{
    public class ElasticSearchProjectionWriter<T> : IProjectionWriter<T> where T: class
    {
        private readonly string _baseUrl;
        private readonly HttpClient _client;

        public ElasticSearchProjectionWriter(string baseUrl)
        {
            _baseUrl = string.Format("{0}/{1}", baseUrl, typeof(T).Name.ToLower());
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task Add(Guid id, T item)
        {
            var uri = GetUri(id);
            await PutItem(uri, id, item);
        }

        public async Task Update(Guid id, Action<T> update)
        {
            var uri = GetUri(id);
            var body = _client.GetStringAsync(uri).Result;
            var indexItem = JsonConvert.DeserializeObject<IndexItem<T>>(body);
            var item = indexItem._source;
            update(item);
            await PutItem(uri, id, item);
        }

        private Uri GetUri(Guid id)
        {
            return new Uri(string.Format("{0}/external/{1}?pretty", _baseUrl, id));
        }

        private async Task PutItem(Uri uri, Guid id, T item)
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