using System.Collections.Generic;
using Verse;

namespace flangoCore
{
    public static class DebugTools
    {
        [DebugAction("Pawns", "Give skill tree...", false, false, false, 0, false, actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap, displayPriority = 1000)]
        public static List<DebugActionNode> GiveSkillTree()
        {
            List<DebugActionNode> list = new()
            {
                new DebugActionNode("*All", DebugActionType.ToolMapForPawns)
                {
                    pawnAction = delegate (Pawn p)
                    {
                        if (p.TryGetComp<CompSkills>() != null)
                        {
                            foreach (SkillTreeDef allDef in DefDatabase<SkillTreeDef>.AllDefs)
                            {
                                p.TryGetComp<CompSkills>().GiveTree(allDef);
                            }
                        }
                    }
                }
            };
            foreach (SkillTreeDef allDef2 in DefDatabase<SkillTreeDef>.AllDefs)
            {
                SkillTreeDef localTree = allDef2;
                list.Add(new DebugActionNode(allDef2.label, DebugActionType.ToolMapForPawns, null, delegate (Pawn p)
                {
                    p.TryGetComp<CompSkills>()?.GiveTree(localTree);
                }));
            }
            return list;
        }
    }
}
