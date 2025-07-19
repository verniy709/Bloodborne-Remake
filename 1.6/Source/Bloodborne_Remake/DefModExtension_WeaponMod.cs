using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Bloodborne_Remake
{
    public class DefModExtension_WeaponMod : DefModExtension
    {
        public List<WeaponMode> modes;
    }
    public class WeaponMode
    {
        public List<string> labels;//代表远程模式时写"RANGE"，否则填tools的label
        [NoTranslate]
        public GraphicData graphicData;//该模式下的图像数据
        public FleckDef fleck;//切换模式时播放
        public SoundDef sound;//切换模式时播放
        public EffecterDef effecter;//切换时播放

        [Unsaved(false)]
        public Graphic graphicInt;
    }
}
