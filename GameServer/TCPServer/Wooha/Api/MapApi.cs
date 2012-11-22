using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WPFLuaFramework;
using GameServer;
using Wooha.Objects;
using Wooha.Utils;

namespace Wooha.Api
{
    class MapApi
    {
        [LuaFunction("createMonsterRange")]
        public void createMonsterRange(String mapId, String monsterId, int x, int y, int amount, int range)
        {
            CMonsterObject data = Program.objectsData[monsterId] as CMonsterObject;
            for (int i = 0; i < amount; i++)
            {
                CMonsterObject monster = new CMonsterObject();
                monster.ObjectType = CObjectType.MONSTER;
                monster.ObjectId = System.Guid.NewGuid().ToString();
                monster.Level = data.Level;
                monster.CharacterName = data.CharacterName;
                monster.ResourceId = data.ResourceId;
                monster.Action = CAction.STOP;
                monster.Direction = CDirection.RandomDirection();
                Point pos = randomPointInCircle(x, y, range);
                monster.PosX = pos.x;
                monster.PosY = pos.y;
                monster.Speed = data.Speed;
                monster.HealthMax = data.HealthMax;
                monster.Health = data.Health;
                monster.ManaMax = data.ManaMax;
                monster.Mana = data.Mana;
                monster.AttackRange = data.AttackRange;
                monster.AttackSpeed = data.AttackSpeed;
                Program.objectList.Add(monster);
                Console.WriteLine("创建角色 " + monsterId + "，Guid:" + monster.ObjectId + "，X:" + monster.PosX + "，Y:" + monster.PosY);
            }
        }

        private double angleToDegree(float angle)
        {
            if (angle >= 360)
            {
                angle %= 360;
            }
            return angle * (Math.PI / 180);
        }

        private Point randomPointInCircle(int x, int y, int radians)
        {
            Random rand = new Random(unchecked((int)DateTime.Now.Ticks));
            int angle = rand.Next(360);
            int rad = rand.Next(radians);
            double degree = angleToDegree(angle);
            int _x = (int)(Math.Cos(degree) * rad);
            int _y = (int)(Math.Sin(degree) * rad);
            return new Point(x + _x, y + _y);
        }
    }
}
