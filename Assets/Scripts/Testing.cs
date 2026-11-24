using Assets.Scripts.Grids;
using Assets.Scripts.Items;
using Assets.Scripts.Loadouts;
using Assets.Scripts.Soldiers;
using Assets.Scripts.Squads;
using Assets.Scripts.Units;
using Assets.Scripts.Weapons;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class Testing : MonoBehaviour
    {
        [SerializeField] private RosterSO roster;
        [SerializeField] private LoadoutOptionsSO options;

        [Tooltip("Soldier i spawns at point i")]
        [SerializeField] private List<Transform> deployPoints = new List<Transform>();


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                SpawnTestSquad();
            }
        }

        /// <summary>
        /// TEMP: spawns the default squad at the deploy points, with each soldier's loadout
        /// changed away from its defaults, to check that weapon / look / item are applied.
        /// </summary>
        private void SpawnTestSquad()
        {
            if (roster == null || options == null)
            {
                Debug.LogError($"{name}: assign roster and options in the Inspector", this);
                return;
            }

            SquadLoadout squad = SquadLoadout.CreateDefault(roster, options);
            ChangeLoadouts(squad);

            HashSet<GridPosition> usedCells = new HashSet<GridPosition>();

            for (int i = 0; i < squad.Members.Count; i++)
            {
                SoldierLoadout loadout = squad.Members[i];

                if (i >= deployPoints.Count || deployPoints[i] == null)
                {
                    Debug.LogWarning($"{name}: no deploy point {i} for {loadout.Soldier.DisplayName}", this);
                    continue;
                }

                GridPosition gridPosition = LevelGrid.Instance.GetGridPosition(deployPoints[i].position);

                // Units register to LevelGrid next frame, so same-cell spawns in this loop must be caught here
                if (!usedCells.Add(gridPosition))
                {
                    Debug.LogWarning($"{name}: {deployPoints[i].name} shares cell {gridPosition} with another deploy point", deployPoints[i]);
                    continue;
                }

                bool spawned = UnitSpawner.Instance.TrySpawnSoldier(loadout, gridPosition, out Unit unit);

                Debug.Log($"[Testing] {i}: {loadout.Soldier.DisplayName} | " +
                          $"weapon {Name(loadout.Weapon)} (range {(loadout.Weapon != null ? loadout.Weapon.Range : 0)}) | " +
                          $"look {Name(loadout.Look)} | item {Name(loadout.Item)} | " +
                          (spawned ? $"spawned at {gridPosition}" : $"FAILED at {gridPosition} (outside grid or occupied?)"),
                          spawned ? unit : (Object)deployPoints[i]);
            }
        }

        /// <summary>
        /// Gives each soldier a different weapon / look / item than its default,
        /// cycling through the option pools so all entries get used.
        /// </summary>
        private void ChangeLoadouts(SquadLoadout squad)
        {
            List<WeaponSO> weapons = options.AvailableWeapons.Where(w => w != null).ToList();
            List<UnitLookSO> looks = options.AvailableLooks.Where(l => l != null).ToList();
            List<ItemSO> items = options.AvailableItems.Where(it => it != null).ToList();

            for (int i = 0; i < squad.Members.Count; i++)
            {
                SoldierLoadout loadout = squad.Members[i];

                // Offset by 1 so soldier 0 does not simply get the first (usually default) entry
                if (weapons.Count > 0)
                    LogIfRejected(loadout.TrySetWeapon(weapons[(i + 1) % weapons.Count]), loadout, "weapon");

                if (looks.Count > 0)
                    LogIfRejected(loadout.TrySetLook(looks[(i + 1) % looks.Count]), loadout, "look");

                // One item per soldier while items last, the rest go without
                ItemSO item = i < items.Count ? items[i] : null;
                LogIfRejected(loadout.TrySetItem(item), loadout, "item");
            }
        }

        private void LogIfRejected(bool accepted, SoldierLoadout loadout, string slot)
        {
            if (!accepted)
                Debug.LogWarning($"[Testing] {loadout.Soldier.DisplayName}: {slot} was rejected by the loadout rules", this);
        }

        private static string Name(Object asset) => asset != null ? asset.name : "none";
    }
}
