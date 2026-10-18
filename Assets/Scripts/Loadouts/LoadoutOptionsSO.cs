using Assets.Scripts.Items;
using Assets.Scripts.Units;
using Assets.Scripts.Weapons;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Loadouts
{
    [CreateAssetMenu(fileName = "New Loadout Options", menuName = "Game/Loadout Options")]
    public class LoadoutOptionsSO : ScriptableObject
    {
        [Min(1)][SerializeField] private int maxSquadSize = 4;
        [SerializeField] private List<WeaponSO> availableWeapons = new List<WeaponSO>();
        [SerializeField] private List<UnitLookSO> availableLooks = new List<UnitLookSO>();
        [SerializeField] private List<ItemSO> availableItems = new List<ItemSO>();

        public int MaxSquadSize => maxSquadSize;
        public IReadOnlyList<WeaponSO> AvailableWeapons => availableWeapons;
        public IReadOnlyList<UnitLookSO> AvailableLooks => availableLooks;
        public IReadOnlyList<ItemSO> AvailableItems => availableItems;

        private void OnValidate()
        {
            if (availableWeapons.Contains(null))
                Debug.LogWarning($"{name}: availableWeapons contains an empty slot", this);

            if (availableLooks.Contains(null))
                Debug.LogWarning($"{name}: availableLooks contains an empty slot", this);

            if (availableItems.Contains(null))
                Debug.LogWarning($"{name}: availableItems contains an empty slot", this);
        }
    }
}
