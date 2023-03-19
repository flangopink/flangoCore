#pragma warning disable IDE0060 // shut up
using Verse;

namespace flangoCore
{
    [StaticConstructorOnStartup]
    public class Randomizer : GameComponent
    {
        public Randomizer(Game game) { }

        static Randomizer()
        {
            if (FlangoCore.settings.randomizerEnabled)
            {
                var races = DefDatabase<PawnKindDef>.AllDefs;
                var recipes = DefDatabase<RecipeDef>.AllDefs;

                foreach (PawnKindDef race in races)
                {
                    //if (race.RaceProps.Humanlike) continue;
                    var other = races.RandomElement();

                    race.label = other.label; // randomize name, then randomize lifestages

                    /*other = races.RandomElement();

                    if (race.lifeStages != null && other.lifeStages != null)
                    {
                        for (int i = 0; i < race.lifeStages.Count; i++)
                        {
                            if (i == race.lifeStages.Count || i == other.lifeStages.Count) break;
                            race.lifeStages[i].bodyGraphicData = other.lifeStages[i].bodyGraphicData;
                            race.lifeStages[i].dessicatedBodyGraphicData = other.lifeStages[i].dessicatedBodyGraphicData;
                        }
                        //race.race.graphicData = other.race.graphicData;
                        //race.race.graphic = other.race.graphic;
                    }*/
                }

                foreach (RecipeDef def in recipes)
                {
                    def.label = recipes.RandomElement().label;
                    def.description = recipes.RandomElement().description;
                }
            }
        }

        public override void FinalizeInit()
        {
            if (FlangoCore.settings.randomizerEnabled)
            {
                var defs = DefDatabase<ThingDef>.AllDefs;
                var terrains = DefDatabase<TerrainDef>.AllDefs;

                foreach (ThingDef def in defs)
                {
                    var other = defs.RandomElement();

                    def.label = other.label;
                    def.description = other.description;

                    while (true)
                    {
                        other = defs.RandomElement();

                        /*var other = defs.RandomElement();

                        def.label = other.label;
                        def.description = other.description;*/ // If i want to keep the names of randomized items.

                        if (def.graphicData == null) break;

                        if (other.IsBlueprint || other.projectile != null) continue;

                        if (def.graphicData.graphicClass == other.graphicData?.graphicClass)
                        {
                            def.graphicData = other.graphicData;
                            def.uiIcon = other.uiIcon;
                            def.uiIconScale = other.uiIconScale;
                            def.size = other.size;
                            break;
                        }
                    }
                }

                foreach (TerrainDef def in terrains)
                {
                    var other = terrains.RandomElement();
                    def.label = other.label;

                    while (true)
                    {
                        other = terrains.RandomElement();
                        if (def.graphic == null || other.graphic == null) break;
                        def.graphic = other.graphic;
                        break;
                    }
                }
            }
        }
    }
}
