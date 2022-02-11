using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AareonTechnicalTest.JsonConfiguration
{
    public static class JsonSerializerExtensions
    {
        public static string Serialise(this object value)
        {
            return JsonConvert.SerializeObject(value);
        }

        // TODO: Move to an http helper class
        public static async Task<T> DeserialiseContentAsync<T>(this HttpResponseMessage response)
        {
            var json = await response.Content.ReadAsStringAsync();
            return json.Deserialise<T>();
        }

        public static T Deserialise<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json, SerializerSettings);
        }

        private static JsonSerializerSettings SerializerSettings { get; } = new JsonSerializerSettings()
        {
            ContractResolver = new PrivateResolver(),
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
        };
    }
}



