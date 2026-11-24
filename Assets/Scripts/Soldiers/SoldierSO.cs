using Assets.Scripts.Items;
using Assets.Scripts.Squads;
using Assets.Scripts.Traits;
using Assets.Scripts.Units;
using Assets.Scripts.Weapons;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Soldiers
{
    [CreateAssetMenu(fileName = "New Soldier", menuName = "Game/Soldier")]
    public class SoldierSO : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("Display name for UI")]
        [SerializeField] private string displayName;

        [Tooltip("Unit config (soldier class)")]
        [SerializeField] private UnitConfigSO unitConfig;

        [Tooltip("Passive traits this soldier always has")]
        [SerializeField] private List<TraitEffectSO> innateTraits = new List<TraitEffectSO>();

        [Header("Defaults")]
        [Tooltip("Weapon held by default")]
        [SerializeField] private WeaponSO defaultWeapon;

        [Tooltip("Look applied by default")]
        [SerializeField] private UnitLookSO defaultLook;

        [Tooltip("Item held by default. Leave empty if none")]
        [SerializeField] private ItemSO defaultItem;

        public string DisplayName => displayName;
        public UnitConfigSO UnitConfig => unitConfig;
        public IReadOnlyList<TraitEffectSO> InnateTraits => innateTraits;
        public WeaponSO DefaultWeapon => defaultWeapon;
        public UnitLookSO DefaultLook => defaultLook;
        public ItemSO DefaultItem => defaultItem;

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(displayName))
                Debug.LogWarning($"{name}: displayName is empty", this);

            if (unitConfig == null)
                Debug.LogWarning($"{name}: unitConfig is not assigned", this);

            if (innateTraits.Contains(null))
                Debug.LogWarning($"{name}: innateTraits contains an empty slot", this);
        }
    }
}
