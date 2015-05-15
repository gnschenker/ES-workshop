using System;
using System.Collections.Generic;
using System.Text;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Projects.Infrastructure
{
    public static class MessageExtensions
    {
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None };

        public static EventData ToEventData(this object message, IDictionary<string, object> headers)
        {
            var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message, SerializerSettings));

            var eventHeaders = new Dictionary<string, object>(headers)
            {
                {
                    "EventClrType", message.GetType().AssemblyQualifiedName
                }
            };
            var metadata = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(eventHeaders, SerializerSettings));
            var typeName = message.GetType().Name;

            var eventId = Guid.NewGuid();
            return new EventData(eventId, typeName, true, data, metadata);
        }

        public static object DeserializeEvent(this ResolvedEvent x)
        {
            var eventClrTypeName = JObject.Parse(Encoding.UTF8.GetString(x.OriginalEvent.Metadata)).Property("EventClrType").Value;
            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(x.OriginalEvent.Data), Type.GetType((string)eventClrTypeName));
        }
    }
}