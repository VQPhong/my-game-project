using Assets.Scripts.Grids;
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

        public bool TrySpawnUnit(UnitConfigSO config, GridPosition gridPosition, WeaponSO weaponOverride = null)
        {
            return TrySpawnUnit(config, gridPosition, weaponOverride, out _);
        }

        public bool TrySpawnUnit(UnitConfigSO config, GridPosition gridPosition, WeaponSO weaponOverride, out Unit unit)
        {
            unit = null;

            if (!LevelGrid.Instance.IsValidGridPosition(gridPosition))
            {
                return false;
            }

            if (LevelGrid.Instance.HasAnyUnitOnGridPosition(gridPosition))
            {
                return false;
            }

            Transform unitTransform = Instantiate(
                config.prefab,
                LevelGrid.Instance.GetWorldPosition(gridPosition),
                Quaternion.identity);

            unit = unitTransform.GetComponent<Unit>();

            unit.Configure(config);

            if (weaponOverride != null)
                unit.EquipWeapon(weaponOverride);

            return true;
        }
    }
}
