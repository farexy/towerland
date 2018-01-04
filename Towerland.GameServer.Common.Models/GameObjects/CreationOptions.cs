using Newtonsoft.Json;
using Towerland.GameServer.Common.Models.GameField;

namespace Towerland.GameServer.Common.Models.GameObjects
{
  public struct CreationOptions
  {
    [JsonProperty("p")] public Point Position;
    [JsonProperty("i")] public int? PathId;
  }
}
