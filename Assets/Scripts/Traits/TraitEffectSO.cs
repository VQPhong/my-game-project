using Assets.Scripts.Units;
using UnityEngine;

namespace Assets.Scripts.Traits
{
    public abstract class TraitEffectSO : ScriptableObject
    {
        /// <summary>
        /// Must cover: how this trait is applied
        /// </summary>
        /// <param name="unit"></param>
        public abstract void Apply(Unit unit);

        public abstract string GetDescription();
    }
}