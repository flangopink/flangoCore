using Verse;

namespace flangoCore
{
    public class Verb_LaunchProjectileOneUse : Verb_LaunchProjectile
    {
        protected override bool TryCastShot()
        {
            if (base.TryCastShot())
            {
                if (burstShotsLeft <= 1)
                {
                    SelfConsume();
                }
                return true;
            }
            if (burstShotsLeft < verbProps.burstShotCount)
            {
                SelfConsume();
            }
            return false;
        }

        public override void Notify_EquipmentLost()
        {
            base.Notify_EquipmentLost();
            if (state == VerbState.Bursting && burstShotsLeft < verbProps.burstShotCount)
            {
                SelfConsume();
            }
        }

        private void SelfConsume()
        {
            if (EquipmentSource != null && !EquipmentSource.Destroyed)
            {
                EquipmentSource.Destroy();
            }
        }
    }
}
