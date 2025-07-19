using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Bloodborne_Remake
{
    public class DefModExtension_AOEMelee : DefModExtension
    {
        public float chance;//攻击触发AOE的概率，填小数
        public float range;//AOE攻击范围
        public EffecterDef effecter_Normal;
        public EffecterDef effecter_Mechanoid;
    }
}
