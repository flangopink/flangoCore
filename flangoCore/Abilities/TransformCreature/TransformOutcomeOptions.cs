using RimWorld;
using Verse;
using UnityEngine;

namespace flangoCore
{
    public class TransformOutcomeOptions
    {
        public ThingDef thingDef;
        public string label;
        public string iconPath;
        Texture2D _icon;
        public Texture2D Icon
        {
            get
            {
                if (_icon == null)
                {
                    _icon = ContentFinder<Texture2D>.Get(iconPath);
                }
                return _icon;
            }
        }
    }
}
