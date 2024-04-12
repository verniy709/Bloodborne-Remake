using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Sound;

namespace Bloodborne_Remake
{
    public class BloodborneWeapon : ThingWithComps
    {
        public Dictionary<string, WeaponMode> WeaponModes
        {
            get
            {
                if (weaponModes == null)
                {
                    InitializeWeaponMods();
                }
                return weaponModes;
            }
        }
        public override Graphic Graphic
        {
            get
            {
                if(verb == null)
                {
                    verb = GetComp<CompEquippable>().PrimaryVerb;
                }
                if(verb.Caster == null)
                {
                    return base.Graphic;
                }
                if(currentMode == null)
                {
                    currentMode = WeaponModes.RandomElement().Value;
                }
                if(!verb.IsMeleeAttack && verb.WarmingUp)
                {
                    if (WeaponModes.ContainsKey("RANGE")) 
                    {
                        TryChangeMode("RANGE"); 
                    }
                    else
                    {
                        return base.Graphic;
                    }
                }
                return currentMode.graphicInt;
            }
        }
        private void InitializeWeaponMods()
        {
            weaponModes = new Dictionary<string, WeaponMode>();
            foreach(WeaponMode mode in def.GetModExtension<DefModExtension_WeaponMod>().modes)
            {
                if(mode.graphicInt == null)
                {
                    mode.graphicInt = mode.graphicData.GraphicColoredFor(this);
                }
                foreach(string label in mode.labels)
                {
                    weaponModes.Add(label, mode);
                }
            }
        }
        public void TryChangeMode(string newMode)
        {
            if(currentMode == WeaponModes[newMode])
            {
                return;
            }
            currentMode = WeaponModes[newMode];
            Thing caster = verb.Caster;
            TargetInfo targetInfo = new TargetInfo(caster.Position, caster.Map);
            if (currentMode.fleck != null)
            {
                FleckMaker.Static(caster.DrawPos, caster.Map, currentMode.fleck);
            }
            if(currentMode.effecter != null)
            {
                Effecter effecter = currentMode.effecter.Spawn();
                effecter.Trigger(targetInfo, targetInfo);
            }
            currentMode.sound?.PlayOneShot(targetInfo);
        }
        private WeaponMode currentMode;
        private Dictionary<string,WeaponMode> weaponModes;
        private Verb verb;
    }
}
