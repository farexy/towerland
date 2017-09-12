using Newtonsoft.Json;

namespace Towerland.GameServer.Domain.Helpers
{
  internal static class StringExtencions
  {
    public static T FromJsonString<T>(this string s)
    {
      return JsonConvert.DeserializeObject<T>(s);
    }

    public static string ToJsonString<T>(this T o)
    {
      return JsonConvert.SerializeObject(o);
    }
  }
}
