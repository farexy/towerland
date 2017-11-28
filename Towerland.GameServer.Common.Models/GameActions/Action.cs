using GameServer.Common.Models.GameField;
using Newtonsoft.Json;

namespace GameServer.Common.Models.GameActions
{
  public struct GameAction
  {
    [JsonProperty("i")] public ActionId ActionId;
    [JsonProperty("u")] public int UnitId;
    [JsonProperty("t")] public int TowerId;
    [JsonProperty("p")] public Point Position;
    [JsonProperty("d")] public int Damage;
    [JsonProperty("w")] public int WaitTicks;
    [JsonProperty("m")] public int Money;
  }
}