#if UNITY_EDITOR
using Assets.Scripts.Items;
using Assets.Scripts.Loadouts;
using Assets.Scripts.Soldiers;
using Assets.Scripts.Weapons;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.EditorTools
{
    public static class LoadoutDataValidator
    {
        private const string LogPrefix = "[LoadoutValidator]";

        private static int issueCount;

        [MenuItem("Tools/Validate Loadout Data")]
        private static void Validate()
        {
            issueCount = 0;

            List<RosterSO> rosters = LoadAll<RosterSO>();
            List<LoadoutOptionsSO> optionsList = LoadAll<LoadoutOptionsSO>();
            List<SoldierSO> soldiers = LoadAll<SoldierSO>();
            List<ItemSO> items = LoadAll<ItemSO>();

            if (rosters.Count != 1)
                Warn($"expected exactly 1 RosterSO, found {rosters.Count}", rosters.FirstOrDefault());

            if (optionsList.Count != 1)
                Warn($"expected exactly 1 LoadoutOptionsSO, found {optionsList.Count}", optionsList.FirstOrDefault());

            LoadoutOptionsSO options = optionsList.FirstOrDefault();

            foreach (RosterSO roster in rosters)
                ValidateRoster(roster, options);

            foreach (SoldierSO soldier in soldiers)
                ValidateSoldier(soldier, options, rosters);

            if (options != null)
                ValidateOptions(options);

            foreach (ItemSO item in items)
                ValidateItem(item);

            if (issueCount == 0)
                Debug.Log($"{LogPrefix} OK - no issues found");
            else
                Debug.LogWarning($"{LogPrefix} {issueCount} issue(s) found");
        }

        private static void ValidateRoster(RosterSO roster, LoadoutOptionsSO options)
        {
            HashSet<SoldierSO> seen = new HashSet<SoldierSO>();
            HashSet<string> seenNames = new HashSet<string>();

            foreach (SoldierSO soldier in roster.Soldiers)
            {
                if (soldier == null)
                {
                    Warn("roster contains an empty slot", roster);
                    continue;
                }

                if (!seen.Add(soldier))
                {
                    Warn($"{soldier.name} is listed more than once", roster);
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(soldier.DisplayName) && !seenNames.Add(soldier.DisplayName))
                    Warn($"display name '{soldier.DisplayName}' is used by more than one soldier", roster);
            }

            if (options != null && seen.Count < options.MaxSquadSize)
                Warn($"roster has {seen.Count} soldier(s), fewer than MaxSquadSize ({options.MaxSquadSize})", roster);
        }

        private static void ValidateSoldier(SoldierSO soldier, LoadoutOptionsSO options, List<RosterSO> rosters)
        {
            if (string.IsNullOrWhiteSpace(soldier.DisplayName))
                Warn($"{soldier.name}: displayName is empty", soldier);

            if (soldier.UnitConfig == null)
                Warn($"{soldier.name}: UnitConfig is not assigned", soldier);
            else if (soldier.UnitConfig.prefab == null)
                Warn($"{soldier.name}: UnitConfig '{soldier.UnitConfig.name}' has no prefab", soldier);

            if (soldier.DefaultWeapon == null)
                Warn($"{soldier.name}: DefaultWeapon is not assigned", soldier);

            if (soldier.DefaultLook == null)
                Warn($"{soldier.name}: DefaultLook is not assigned", soldier);

            if (soldier.InnateTraits.Contains(null))
                Warn($"{soldier.name}: InnateTraits contains an empty slot", soldier);

            if (!rosters.Any(r => r.Soldiers.Contains(soldier)))
                Warn($"{soldier.name}: not listed in any roster", soldier);

            if (options == null) return;

            if (soldier.DefaultWeapon != null && !options.AvailableWeapons.Contains(soldier.DefaultWeapon))
                Warn($"{soldier.name}: DefaultWeapon '{soldier.DefaultWeapon.name}' is not in {options.name}.AvailableWeapons", soldier);

            if (soldier.DefaultLook != null && !options.AvailableLooks.Contains(soldier.DefaultLook))
                Warn($"{soldier.name}: DefaultLook '{soldier.DefaultLook.name}' is not in {options.name}.AvailableLooks", soldier);

            if (soldier.DefaultItem != null && !options.AvailableItems.Contains(soldier.DefaultItem))
                Warn($"{soldier.name}: DefaultItem '{soldier.DefaultItem.name}' is not in {options.name}.AvailableItems", soldier);
        }

        private static void ValidateOptions(LoadoutOptionsSO options)
        {
            if (options.AvailableWeapons.Count == 0)
                Warn("AvailableWeapons is empty", options);

            if (options.AvailableLooks.Count == 0)
                Warn("AvailableLooks is empty", options);

            foreach (WeaponSO weapon in options.AvailableWeapons)
            {
                if (weapon == null)
                {
                    Warn("AvailableWeapons contains an empty slot", options);
                    continue;
                }

                if (string.IsNullOrWhiteSpace(weapon.WeaponName))
                    Warn($"{weapon.name}: WeaponName is empty", weapon);

                if (weapon.WeaponPrefab == null)
                    Warn($"{weapon.name}: WeaponPrefab is not assigned", weapon);

                if (!weapon.Damage.IsValid())
                    Warn($"{weapon.name}: damage min ({weapon.Damage.min}) > max ({weapon.Damage.max})", weapon);
            }

            if (options.AvailableLooks.Contains(null))
                Warn("AvailableLooks contains an empty slot", options);

            if (options.AvailableItems.Contains(null))
                Warn("AvailableItems contains an empty slot", options);

            // Display names must be unique within each pool, otherwise UI entries look identical
            WarnDuplicateNames(options.AvailableWeapons, w => w.WeaponName, "AvailableWeapons", options);
            WarnDuplicateNames(options.AvailableLooks, l => l.DisplayName, "AvailableLooks", options);
            WarnDuplicateNames(options.AvailableItems, i => i.DisplayName, "AvailableItems", options);
        }

        private static void WarnDuplicateNames<T>(IReadOnlyList<T> assets, System.Func<T, string> getName, string listName, Object context) where T : Object
        {
            HashSet<string> seenNames = new HashSet<string>();

            foreach (T asset in assets)
            {
                if (asset == null) continue;

                string displayName = getName(asset);
                if (string.IsNullOrWhiteSpace(displayName)) continue;

                if (!seenNames.Add(displayName))
                    Warn($"{listName}: display name '{displayName}' is used by more than one entry ({asset.name})", context);
            }
        }

        private static void ValidateItem(ItemSO item)
        {
            if (string.IsNullOrWhiteSpace(item.DisplayName))
                Warn($"{item.name}: displayName is empty", item);

            if (item.Traits.Count == 0)
                Warn($"{item.name}: has no traits", item);
            else if (item.Traits.Contains(null))
                Warn($"{item.name}: traits contains an empty slot", item);
        }

        private static List<T> LoadAll<T>() where T : Object
        {
            return AssetDatabase.FindAssets($"t:{typeof(T).Name}")
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .Select(path => AssetDatabase.LoadAssetAtPath<T>(path))
                .Where(asset => asset != null)
                .ToList();
        }

        private static void Warn(string message, Object context)
        {
            issueCount++;
            Debug.LogWarning($"{LogPrefix} {message}", context);
        }
    }
}
#endif
