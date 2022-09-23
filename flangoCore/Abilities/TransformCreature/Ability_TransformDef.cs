using RimWorld;
using Verse;
using System.Text;
using UnityEngine;

namespace flangoCore
{
    public class Ability_TransformDef : AbilityDef
    {
        public float cooldown = -1;
        public bool requirePsyker = false;

        public string optionsIconPath;
        public string resetIconPath;

        public virtual string GetDescription()
        {
            var coolDesc = GetBasics();
            var desc = new StringBuilder();
            desc.AppendLine(description);
            if (coolDesc != "") desc.AppendLine(coolDesc);
            string result = desc.ToString();
            return result;
        }
        public string GetBasics()
        {
            var result = "";
            var def = verbProperties;
            if (def != null)
            {
                var s = new StringBuilder();
                s.AppendLine("Cooldown: " + this.cooldown.ToString("N0") + " " + "SecondsLower".Translate());
                result = s.ToString();
            }
            return result;
        }
    }
}
