using Assets.Scripts.Units;
using UnityEngine;

namespace Assets.Scripts.Traits
{
    [CreateAssetMenu(fileName = "New Damage Bonus", menuName = "Game/Traits/Damage Bonus")]
    public class DamageBonusEffectSO : TraitEffectSO
    {
        [Min(1)][SerializeField] private int amount = 20;

        public int Amount => amount;

        public override void Apply(Unit unit)
        {
            // TODO: add real apply
        }

        public override string GetDescription()
        {
            return $"+{amount} weapon damage";
        }
    }
}
