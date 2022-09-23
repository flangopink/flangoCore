using RimWorld;

namespace flangoCore
{
    public class Command_Transform : Command_Ability
    {
        public Command_Transform(Ability_Transform ability) : base(ability)
        {
        }

        public int curTicks = -1;
        public new Ability_Transform Ability => (Ability_Transform)base.ability;
    }
}
