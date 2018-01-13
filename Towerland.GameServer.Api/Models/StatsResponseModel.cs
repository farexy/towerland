using Towerland.GameServer.Common.Models.Stats;

namespace Towerland.GameServer.Api.Models
{
    public class StatsResponseModel
    {
        public UnitStats[] UnitStats { get; set; }
        public TowerStats[] TowerStats { get; set; }
        public DefenceCoeff[] DefenceCoeffs { get; set; }
    }
}