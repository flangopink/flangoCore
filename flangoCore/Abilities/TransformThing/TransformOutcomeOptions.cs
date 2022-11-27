using Verse;
using UnityEngine;

namespace flangoCore
{
    public class TransformOutcomeOptions
    {
        public ThingDef thingDef;
        public int resultStackCount = -1;
        public int requiredStackCount = 1;
        public string label;
        public string iconPath;
        public ResultFaction faction;
        public Texture2D Icon => ContentFinder<Texture2D>.Get(iconPath);
    }

    public enum ResultFaction
    {
        Current,
        Neutral,
        Player,
        Enemy
    }
}
