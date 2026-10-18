using Assets.Scripts.Units;
using UnityEngine;

namespace Assets.Scripts.Traits
{
    [CreateAssetMenu(fileName = "New Max Health Bonus", menuName = "Game/Traits/Max Health Bonus")]
    public class MaxHealthBonusEffectSO : TraitEffectSO
    {
        [Min(1)][SerializeField] private int amount = 20;

        public int Amount => amount;

        public override void Apply(Unit unit)
        {
            // TODO: add real apply
        }

        public override string GetDescription()
        {
            return $"+{amount} max HP";
        }
    }
}
