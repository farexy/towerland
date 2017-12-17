using GameServer.Common.Models.GameField;
using Newtonsoft.Json;

namespace GameServer.Common.Models.GameObjects
{
  public struct CreationOptions
  {
    [JsonProperty("p")] public Point Position;
    [JsonProperty("i")] public int? PathId;
  }
}
