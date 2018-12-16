using System;
using Towerland.GameServer.Data.Entities;

namespace Towerland.GameServer.BusinessLogic.Helpers
{
    public static class ComputerPlayer
    {
        public static readonly Guid Id = Guid.Parse("e8d0ec64-d96b-4e77-baf4-35e37fe40a68");

        public static bool IsComputerPlayer(this Guid id) => Id == id;

        public static bool IsSinglePlayer(this Battle battle) =>
            battle.Monsters_UserId == Id || battle.Towers_UserId == Id;
    }
}