using Towerland.GameServer.Models.GameField;
using Towerland.GameServer.Models.GameObjects;

namespace Towerland.GameServer.Logic.Factories
{
    public class FieldFactory
    {  
        public Field Create(int[,] map, FieldTheme theme)
        {
            var cells = new FieldCell[map.GetLength(0), map.GetLength(1)];

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    cells[i, j] = new FieldCell
                    {
                        Position = new Point(i, j),
                        Object = GetObject(map[i, j])
                    };
                }
            }

            var field = new Field(cells);
            field.StaticData.InterfaceTheme = theme;
            field.State.Castle = new Castle
                {
                    Health = 100,
                    Position = field.StaticData.Finish
                };
            field.State.TowerMoney = 120;
            field.State.MonsterMoney = 120;

            return field;
        }

        public static FieldObject GetObject(int objNum)
        {
            var obj = (FieldObject) objNum;
            switch (obj)
            {
                case FieldObject.Entrance:
                    return FieldObject.Road | obj;
                case FieldObject.Stone:
                case FieldObject.Tree:
                    return FieldObject.Ground | obj;
                default:
                    return obj;
            }
        }
    }
}
