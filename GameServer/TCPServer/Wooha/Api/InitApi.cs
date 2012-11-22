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
    class InitApi
    {
        [LuaFunction("registerMonsterData")]
        public void registerMonsterData(String monsterId, String name, int level, String resourceId, int speed, int health, int mana, int attackRange, float attackSpeed, Boolean passitiveMonster = false)
        {
            CMonsterObject monster = new CMonsterObject();
            monster.ObjectType = CObjectType.MONSTER;
            monster.Level = level;
            monster.CharacterName = name;
            monster.ResourceId = resourceId;
            monster.Speed = speed;
            monster.HealthMax = health;
            monster.Health = health;
            monster.ManaMax = mana;
            monster.Mana = mana;
            monster.AttackRange = attackRange;
            monster.AttackSpeed = attackSpeed;
            monster.PassitiveMonster = passitiveMonster;

            Program.objectsData.Add(monsterId, monster);
        }
    }
}
