#if UNITY_EDITOR
using Assets.Scripts.Items;
using Assets.Scripts.Loadouts;
using Assets.Scripts.Soldiers;
using Assets.Scripts.Traits;
using Assets.Scripts.Units;
using Assets.Scripts.Weapons;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.EditorTools
{
    public static class LoadoutSmokeTest
    {
        private const string LogPrefix = "[LoadoutSmokeTest]";

        private static int passed;
        private static int failed;
        private static int skipped;

        private static readonly List<Object> temps = new List<Object>();

        [MenuItem("Tools/Loadout Smoke Test")]
        private static void Run()
        {
            passed = 0;
            failed = 0;
            skipped = 0;

            RosterSO roster = LoadFirst<RosterSO>();
            LoadoutOptionsSO options = LoadFirst<LoadoutOptionsSO>();

            if (roster == null || options == null)
            {
                Debug.LogError($"{LogPrefix} needs a RosterSO and a LoadoutOptionsSO asset in the project");
                return;
            }

            SoldierSO soldier = roster.Soldiers.FirstOrDefault(s => s != null);
            if (soldier == null)
            {
                Debug.LogError($"{LogPrefix} roster '{roster.name}' has no soldiers", roster);
                return;
            }

            Dictionary<SoldierSO, string> snapshot = TakeSnapshot(roster);

            try
            {
                TestSoldierLoadout(soldier, options);
                TestSquadLoadout(roster, options);
                TestCreateDefault(roster, options);
                TestLoadoutSession(roster, options);
            }
            catch (System.Exception e)
            {
                failed++;
                Debug.LogError($"{LogPrefix} FAIL - unexpected exception: {e}");
            }
            finally
            {
                foreach (Object temp in temps)
                    Object.DestroyImmediate(temp);
                temps.Clear();

                LoadoutSession.Clear();
            }

            // Runtime state must never write back into SO assets
            Check(SnapshotMatches(snapshot), "SoldierSO defaults are unchanged after all tests");

            string summary = $"{LogPrefix} {passed}/{passed + failed} passed" + (skipped > 0 ? $", {skipped} skipped" : "");
            if (failed == 0)
                Debug.Log(summary);
            else
                Debug.LogError(summary);
        }

        // ---------- SoldierLoadout ----------

        private static void TestSoldierLoadout(SoldierSO soldier, LoadoutOptionsSO options)
        {
            Check(Throws<System.ArgumentNullException>(() => new SoldierLoadout(null, options)),
                "SoldierLoadout(null soldier) throws ArgumentNullException");

            Check(Throws<System.ArgumentNullException>(() => new SoldierLoadout(soldier, null)),
                "SoldierLoadout(null options) throws ArgumentNullException");

            SoldierLoadout loadout = new SoldierLoadout(soldier, options);

            Check(loadout.Soldier == soldier, "Soldier is the one passed in");
            Check(loadout.Weapon == soldier.DefaultWeapon, "new loadout copies DefaultWeapon");
            Check(loadout.Look == soldier.DefaultLook, "new loadout copies DefaultLook");
            Check(loadout.Item == soldier.DefaultItem, "new loadout copies DefaultItem");

            TestWeapon(loadout, options);
            TestLook(loadout, options);
            TestItem(loadout, options);
            TestResetToDefaults(loadout, soldier, options);
            TestTraits(loadout, options);
        }

        private static void TestWeapon(SoldierLoadout loadout, LoadoutOptionsSO options)
        {
            WeaponSO other = options.AvailableWeapons.FirstOrDefault(w => w != null && w != loadout.Weapon);
            if (other == null)
                Skip("TrySetWeapon(in pool) - options need a second weapon");
            else
                Check(loadout.TrySetWeapon(other) && loadout.Weapon == other, "TrySetWeapon(in pool) succeeds");

            WeaponSO before = loadout.Weapon;

            WeaponSO notInPool = CreateTemp<WeaponSO>("Temp_Weapon");
            Check(!loadout.TrySetWeapon(notInPool) && loadout.Weapon == before, "TrySetWeapon(not in pool) fails, weapon unchanged");

            Check(!loadout.TrySetWeapon(null) && loadout.Weapon == before, "TrySetWeapon(null) fails, weapon unchanged");
        }

        private static void TestLook(SoldierLoadout loadout, LoadoutOptionsSO options)
        {
            UnitLookSO other = options.AvailableLooks.FirstOrDefault(l => l != null && l != loadout.Look);
            if (other == null)
                Skip("TrySetLook(in pool) - options need a second look");
            else
                Check(loadout.TrySetLook(other) && loadout.Look == other, "TrySetLook(in pool) succeeds");

            UnitLookSO before = loadout.Look;

            UnitLookSO notInPool = CreateTemp<UnitLookSO>("Temp_Look");
            Check(!loadout.TrySetLook(notInPool) && loadout.Look == before, "TrySetLook(not in pool) fails, look unchanged");

            Check(!loadout.TrySetLook(null) && loadout.Look == before, "TrySetLook(null) fails, look unchanged");
        }

        private static void TestItem(SoldierLoadout loadout, LoadoutOptionsSO options)
        {
            ItemSO inPool = options.AvailableItems.FirstOrDefault(i => i != null);
            if (inPool == null)
                Skip("TrySetItem(in pool) - options have no items");
            else
                Check(loadout.TrySetItem(inPool) && loadout.Item == inPool, "TrySetItem(in pool) succeeds");

            ItemSO before = loadout.Item;

            ItemSO notInPool = CreateTemp<ItemSO>("Temp_Item");
            Check(!loadout.TrySetItem(notInPool) && loadout.Item == before, "TrySetItem(not in pool) fails, item unchanged");

            Check(loadout.TrySetItem(null) && loadout.Item == null, "TrySetItem(null) succeeds and unequips");
        }

        private static void TestResetToDefaults(SoldierLoadout loadout, SoldierSO soldier, LoadoutOptionsSO options)
        {
            // Move away from defaults first so the reset is observable
            WeaponSO other = options.AvailableWeapons.FirstOrDefault(w => w != null && w != soldier.DefaultWeapon);
            if (other != null)
                loadout.TrySetWeapon(other);

            loadout.ResetToDefaults();

            Check(loadout.Weapon == soldier.DefaultWeapon
                  && loadout.Look == soldier.DefaultLook
                  && loadout.Item == soldier.DefaultItem,
                "ResetToDefaults restores weapon, look and item");
        }

        private static void TestTraits(SoldierLoadout loadout, LoadoutOptionsSO options)
        {
            ItemSO itemWithTraits = options.AvailableItems.FirstOrDefault(i => i != null && i.Traits.Any(t => t != null));
            if (itemWithTraits == null)
            {
                Skip("GetAllTraits includes item traits - options have no item with traits");
                return;
            }

            loadout.TrySetItem(itemWithTraits);
            List<TraitEffectSO> traits = loadout.GetAllTraits();

            bool hasInnate = loadout.Soldier.InnateTraits.Where(t => t != null).All(traits.Contains);
            bool hasItem = itemWithTraits.Traits.Where(t => t != null).All(traits.Contains);
            Check(hasInnate && hasItem, "GetAllTraits = innate traits + item traits");

            Check(!traits.Contains(null), "GetAllTraits contains no null entries");

            // Caller mutating the result must not leak into the next call (or into SO lists)
            int count = traits.Count;
            traits.Add(null);
            Check(loadout.GetAllTraits().Count == count, "GetAllTraits returns a new list each call");

            loadout.TrySetItem(null);
            Check(!loadout.GetAllTraits().Any(t => itemWithTraits.Traits.Contains(t) && !loadout.Soldier.InnateTraits.Contains(t)),
                "GetAllTraits drops item traits after unequipping the item");
        }

        // ---------- SquadLoadout ----------

        private static void TestSquadLoadout(RosterSO roster, LoadoutOptionsSO options)
        {
            Check(Throws<System.ArgumentNullException>(() => new SquadLoadout(null)),
                "SquadLoadout(null options) throws ArgumentNullException");

            SquadLoadout squad = new SquadLoadout(options);
            Check(squad.IsEmpty && squad.Members.Count == 0, "new squad is empty");
            Check(squad.MaxSize == options.MaxSquadSize, "MaxSize comes from options");

            Check(!squad.TryAdd(null) && squad.IsEmpty, "TryAdd(null) fails");

            List<SoldierSO> candidates = GetCandidates(roster, options.MaxSquadSize + 1);
            SoldierSO first = candidates[0];

            Check(squad.TryAdd(first) && squad.Members.Count == 1, "TryAdd(soldier) succeeds");
            Check(squad.Contains(first), "Contains(added soldier) is true");
            Check(squad.TryGet(first, out SoldierLoadout got) && got.Soldier == first, "TryGet returns the matching loadout");
            Check(!squad.TryAdd(first) && squad.Members.Count == 1, "TryAdd(same soldier) fails");

            // Fill up, then one more must be rejected
            for (int i = 1; i < options.MaxSquadSize; i++)
                squad.TryAdd(candidates[i]);

            Check(squad.IsFull && squad.Members.Count == options.MaxSquadSize, "squad fills up to MaxSize");

            SoldierSO extra = candidates[options.MaxSquadSize];
            Check(!squad.TryAdd(extra) && squad.Members.Count == options.MaxSquadSize, "TryAdd when full fails");

            bool orderKept = true;
            for (int i = 0; i < squad.Members.Count; i++)
                orderKept &= squad.Members[i].Soldier == candidates[i];
            Check(orderKept, "Members keep insertion order");

            // Remove
            int countBefore = squad.Members.Count;
            Check(squad.Remove(first) && squad.Members.Count == countBefore - 1 && !squad.Contains(first),
                "Remove(member) succeeds");
            Check(!squad.Remove(first) && squad.Members.Count == countBefore - 1, "Remove(non-member) fails");
            Check(!squad.Remove(null) && squad.Members.Count == countBefore - 1, "Remove(null) fails");

            // Re-adding gives a fresh loadout with defaults
            TestReAddResetsLoadout(options, first);
        }

        private static void TestReAddResetsLoadout(LoadoutOptionsSO options, SoldierSO soldier)
        {
            WeaponSO other = options.AvailableWeapons.FirstOrDefault(w => w != null && w != soldier.DefaultWeapon);
            if (other == null)
            {
                Skip("Remove then TryAdd resets loadout - options need a second weapon");
                return;
            }

            SquadLoadout squad = new SquadLoadout(options);
            squad.TryAdd(soldier);
            squad.TryGet(soldier, out SoldierLoadout loadout);
            loadout.TrySetWeapon(other);

            squad.Remove(soldier);
            squad.TryAdd(soldier);
            squad.TryGet(soldier, out SoldierLoadout readded);

            Check(readded != loadout && readded.Weapon == soldier.DefaultWeapon, "Remove then TryAdd gives a fresh loadout with defaults");
        }

        private static void TestCreateDefault(RosterSO roster, LoadoutOptionsSO options)
        {
            Check(Throws<System.ArgumentNullException>(() => SquadLoadout.CreateDefault(null, options)),
                "CreateDefault(null roster) throws ArgumentNullException");

            SquadLoadout squad = SquadLoadout.CreateDefault(roster, options);

            List<SoldierSO> expected = roster.Soldiers.Where(s => s != null).Distinct().Take(options.MaxSquadSize).ToList();

            Check(squad.Members.Count == expected.Count,
                $"CreateDefault has {expected.Count} member(s) (MaxSize {options.MaxSquadSize}, roster {roster.Soldiers.Count})");

            bool sameOrder = squad.Members.Select(m => m.Soldier).SequenceEqual(expected);
            Check(sameOrder, "CreateDefault keeps roster order");

            bool allDefaults = squad.Members.All(m => m.Weapon == m.Soldier.DefaultWeapon && m.Look == m.Soldier.DefaultLook);
            Check(allDefaults, "CreateDefault members start with default weapon and look");
        }

        // ---------- Helpers ----------

        /// <summary>
        /// Distinct roster soldiers, topped up with in-memory soldiers if the roster is too small.
        /// </summary>
        private static List<SoldierSO> GetCandidates(RosterSO roster, int count)
        {
            List<SoldierSO> candidates = roster.Soldiers.Where(s => s != null).Distinct().Take(count).ToList();

            while (candidates.Count < count)
                candidates.Add(CreateTemp<SoldierSO>($"Temp_Soldier{candidates.Count}"));

            return candidates;
        }

        private static Dictionary<SoldierSO, string> TakeSnapshot(RosterSO roster)
        {
            return roster.Soldiers
                .Where(s => s != null)
                .Distinct()
                .ToDictionary(s => s, Describe);
        }

        private static bool SnapshotMatches(Dictionary<SoldierSO, string> snapshot)
        {
            return snapshot.All(pair => Describe(pair.Key) == pair.Value);
        }

        private static string Describe(SoldierSO soldier)
        {
            string traits = string.Join(",", soldier.InnateTraits.Select(t => t != null ? t.name : "null"));
            return $"{Name(soldier.DefaultWeapon)}|{Name(soldier.DefaultLook)}|{Name(soldier.DefaultItem)}|{traits}";
        }

        private static string Name(Object asset) => asset != null ? asset.name : "null";

        private static T CreateTemp<T>(string name) where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();
            asset.name = name;
            asset.hideFlags = HideFlags.DontSave;
            temps.Add(asset);
            return asset;
        }

        private static T LoadFirst<T>() where T : Object
        {
            return AssetDatabase.FindAssets($"t:{typeof(T).Name}")
                .Select(guid => AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid)))
                .FirstOrDefault(asset => asset != null);
        }

        private static bool Throws<TException>(System.Action action) where TException : System.Exception
        {
            try
            {
                action();
                return false;
            }
            catch (TException)
            {
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        private static void Check(bool condition, string label)
        {
            if (condition)
            {
                passed++;
                Debug.Log($"{LogPrefix} PASS - {label}");
            }
            else
            {
                failed++;
                Debug.LogError($"{LogPrefix} FAIL - {label}");
            }
        }

        private static void Skip(string label)
        {
            skipped++;
            Debug.LogWarning($"{LogPrefix} SKIP - {label}");
        }

        private static void TestLoadoutSession(RosterSO roster, LoadoutOptionsSO options)
        {
            LoadoutSession.Clear();
            Check(LoadoutSession.Current == null && !LoadoutSession.HasSquad, "session starts empty after Clear");

            SquadLoadout squad = SquadLoadout.CreateDefault(roster, options);
            LoadoutSession.Set(squad);
            Check(LoadoutSession.Current == squad && LoadoutSession.HasSquad, "Set(squad) stores the same squad");

            // Stored by reference: edits after Set are visible through Current
            SoldierSO first = squad.Members[0].Soldier;
            squad.Remove(first);
            Check(!LoadoutSession.Current.Contains(first), "session sees edits made to the squad after Set");

            LoadoutSession.Set(new SquadLoadout(options));
            Check(LoadoutSession.Current != null && !LoadoutSession.HasSquad, "empty squad -> HasSquad is false");

            SquadLoadout before = LoadoutSession.Current;
            Check(Throws<System.ArgumentNullException>(() => LoadoutSession.Set(null)) && LoadoutSession.Current == before,
                "Set(null) throws and keeps the current squad");

            LoadoutSession.Clear();
            Check(LoadoutSession.Current == null && !LoadoutSession.HasSquad, "Clear removes the squad");
        }
    }
}
#endif
