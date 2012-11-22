using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wooha.Utils;

namespace Wooha.Objects
{
    class CMonsterObject : CGameObject
    {
        public int Level = 0;
        public String CharacterName = "";
        public int Speed = 0;
        public int Direction = CDirection.DOWN;
        public int Action = CAction.STOP;
        public int TargetX = int.MinValue;
        public int TargetY = int.MinValue;
        public int HealthMax = int.MinValue;
        public int Health = int.MinValue;
        public int ManaMax = int.MinValue;
        public int Mana = int.MinValue;

        public String TargetId = "";
        public String SkillId = "";

        public int AttackRange = 0;
        public float AttackSpeed = 0;

        public Boolean PassitiveMonster = false;

        public CMonsterObject()
            : base()
        {

        }
    }
}
