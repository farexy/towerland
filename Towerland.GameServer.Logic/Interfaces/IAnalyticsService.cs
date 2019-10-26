using System.Threading.Tasks;
using Towerland.GameServer.Models.GameField;
using Towerland.GameServer.Models.State;

namespace Towerland.GameServer.Logic.Interfaces
{
    public interface IAnalyticsService
    {
        Task LogAsync(StateChangeCommand cmd);
        Task LogEndAsync(PlayerSide winner, Field field);
    }
}