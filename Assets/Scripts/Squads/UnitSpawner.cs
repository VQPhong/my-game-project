using Assets.Scripts.Grids;
using Assets.Scripts.Units;
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

        public bool TrySpawnUnit(UnitConfigSO config, GridPosition gridPosition)
        {
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

            Unit unit = unitTransform.GetComponent<Unit>();
            unit.Configure(config);

            return true;
        }
    }
}
