using Newtonsoft.Json;

namespace RepoLite.Common.Extensions
{
    public static class ObjectExtensions
    {
        public static T Clone<T>(this T input)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(input));
        }
    }
}
