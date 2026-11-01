using Assets.Scripts.Items;
using Assets.Scripts.Soldiers;
using Assets.Scripts.Traits;
using Assets.Scripts.Units;
using Assets.Scripts.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Loadouts
{
    public class SoldierLoadout
    {
        public SoldierSO Soldier { get; }
        public WeaponSO Weapon { get; private set; }
        public UnitLookSO Look { get; private set; }
        public ItemSO Item { get; private set; }

        private readonly LoadoutOptionsSO options;

        public SoldierLoadout(SoldierSO soldier, LoadoutOptionsSO options)
        {
            if (soldier == null)
            {
                throw new ArgumentNullException(nameof(soldier));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            Soldier = soldier;
            this.options = options;

            ResetToDefaults();
        }

        /// <summary>
        /// Weapon is required: null or a weapon outside AvailableWeapons is rejected.
        /// </summary>
        public bool TrySetWeapon(WeaponSO weapon)
        {
            if (weapon == null)
                return false;

            if (!options.AvailableWeapons.Contains(weapon))
                return false;

            Weapon = weapon;
            return true;
        }

        /// <summary>
        /// Look is required: null or a look outside AvailableLooks is rejected.
        /// </summary>
        public bool TrySetLook(UnitLookSO look)
        {
            if (look == null)
                return false;

            if (!options.AvailableLooks.Contains(look))
                return false;

            Look = look;
            return true;
        }

        public bool TrySetItem(ItemSO item)
        {
            if (item == null)
            {
                Item = null;
                return true;
            }

            if (!options.AvailableItems.Contains(item))
                return false;

            Item = item;
            return true;
        }

        public void ResetToDefaults()
        {
            Weapon = Soldier.DefaultWeapon;
            Look = Soldier.DefaultLook;
            Item = Soldier.DefaultItem;
        }

        public List<TraitEffectSO> GetAllTraits()
        {
            List<TraitEffectSO> listTraits = new List<TraitEffectSO>();

            listTraits.AddRange(Soldier.InnateTraits.Where(t => t != null));

            if (Item != null)
            {
                listTraits.AddRange(Item.Traits.Where(t => t != null));
            }

            return listTraits;
        }
    }

}
