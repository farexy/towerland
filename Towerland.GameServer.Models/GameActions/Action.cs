using Newtonsoft.Json;
using Towerland.GameServer.Models.Effects;
using Towerland.GameServer.Models.GameField;
using Towerland.GameServer.Models.GameObjects;

namespace Towerland.GameServer.Models.GameActions
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
    [JsonProperty("s")] public SkillId SkillId;
    [JsonProperty("e")] public EffectId EffectId;
    [JsonProperty("ev")] public double EffectValue;
    [JsonProperty("gu")] public Unit GoUnit;
    [JsonProperty("gt")] public Tower GoTower;
  }
}