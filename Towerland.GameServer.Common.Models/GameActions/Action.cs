using GameServer.Common.Models.GameField;
using Newtonsoft.Json;

namespace GameServer.Common.Models.GameActions
{
  public struct GameAction
  {
    [JsonProperty("ActionId")] public ActionId ActionId;
    [JsonProperty("UnitId")] public int UnitId;
    [JsonProperty("TowerId")] public int TowerId;
    [JsonProperty("Position")] public Point Position;
    [JsonProperty("Damage")] public int Damage;
    [JsonProperty("WaitTicks")] public int WaitTicks;
    [JsonProperty("Money")] public int Money;
  }
}