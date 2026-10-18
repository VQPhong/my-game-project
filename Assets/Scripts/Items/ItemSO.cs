using Assets.Scripts.Traits;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Items
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Game/Item")]
    public class ItemSO : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("Display name for UI")]
        [SerializeField] private string displayName;

        [Tooltip("Flavor text. Trait descriptions are generated separately")]
        [TextArea]
        [SerializeField] private string description;

        [Header("Traits")]
        [SerializeField] private List<TraitEffectSO> traits = new List<TraitEffectSO>();

        public string DisplayName => displayName;
        public string Description => description;
        public IReadOnlyList<TraitEffectSO> Traits => traits;

        private void OnValidate()
        {
            if (traits.Contains(null))
                Debug.LogWarning($"{name}: traits contains an empty slot", this);
        }
    }
}
