using GameServer.Common.Models.GameField;
using Newtonsoft.Json;

namespace GameServer.Common.Models.GameObjects
{
  public struct CreationOptions
  {
    [JsonProperty("Position")] public Point Position;
    [JsonProperty("PathId")] public int PathId;
  }
}
