using Assets.Scripts.Grids;
using Assets.Scripts.Loadouts;
using Assets.Scripts.Units;
using Assets.Scripts.Weapons;
using UnityEngine;

namespace Assets.Scripts.Squads
{
    public class UnitSpawner : MonoBehaviour
    {
        public static UnitSpawner Instance { get; private set; }


        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("There's more than one UnitSpawner! " + transform + " - " + Instance);
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public bool CanSpawnAt(GridPosition gridPosition)
        {
            if (!LevelGrid.Instance.IsValidGridPosition(gridPosition))
            {
                return false;
            }

            if (LevelGrid.Instance.HasAnyUnitOnGridPosition(gridPosition))
            {
                return false;
            }

            return true;
        }

        public bool TrySpawnUnit(UnitConfigSO config, GridPosition gridPosition, WeaponSO weaponOverride = null)
        {
            return TrySpawnUnit(config, gridPosition, weaponOverride, out _);
        }

        /// <summary>
        /// Core spawn path: every other spawn method goes through here.
        /// Weapon priority: weaponOverride > config.defaultWeapon > prefab's weaponData.
        /// </summary>
        public bool TrySpawnUnit(UnitConfigSO config, GridPosition gridPosition, WeaponSO weaponOverride, out Unit unit)
        {
            unit = null;

            if (config == null || config.prefab == null)
            {
                Debug.LogWarning($"{name}: cannot spawn, config or its prefab is missing", this);
                return false;
            }

            if (!CanSpawnAt(gridPosition))
            {
                return false;
            }

            // Awake runs inside Instantiate -> unit holds the prefab's weapon at this point
            Transform unitTransform = Instantiate(
                config.prefab,
                LevelGrid.Instance.GetWorldPosition(gridPosition),
                Quaternion.identity);

            unit = unitTransform.GetComponent<Unit>();

            // Config may replace the prefab's weapon
            unit.Configure(config);

            // Caller (e.g. loadout) has the final say
            if (weaponOverride != null)
                unit.EquipWeapon(weaponOverride);

            // Start runs next frame -> unit registers to LevelGrid with its final setup
            return true;
        }

        /// <summary>
        /// Spawns a soldier with the player's choices: unit config from the SoldierSO,
        /// weapon and look from the loadout.
        /// </summary>
        public bool TrySpawnSoldier(SoldierLoadout loadout, GridPosition gridPosition, out Unit unit)
        {
            unit = null;

            if (loadout == null)
                return false;

            UnitConfigSO config = loadout.Soldier.UnitConfig;
            if (config == null)
            {
                Debug.LogWarning($"{name}: {loadout.Soldier.name} has no UnitConfig", loadout.Soldier);
                return false;
            }

            if (!TrySpawnUnit(config, gridPosition, loadout.Weapon, out unit))
                return false;

            unit.ApplyLook(loadout.Look);
            unit.name = loadout.Soldier.DisplayName;

            // TODO(phase 5): apply loadout.GetAllTraits() to the unit

            return true;
        }
    }
}
