using Newtonsoft.Json;

namespace Eropa.Helper.Helpers
{
    public static class CustomJsonIgnore
    {
        public static string JsonIgnore<T>(T instance)
        {
            string jsonString = JsonConvert.SerializeObject(instance, Newtonsoft.Json.Formatting.None,
                            new JsonSerializerSettings
                            {
                                DefaultValueHandling = DefaultValueHandling.Ignore,
                                NullValueHandling = NullValueHandling.Ignore
                            });
            return jsonString;
        }
    }
}
