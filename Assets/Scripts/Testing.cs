using Assets.Scripts.Grids;
using Assets.Scripts.Squads;
using Assets.Scripts.Units;
using UnityEngine;

namespace Assets.Scripts
{
    public class Testing : MonoBehaviour
    {
        [SerializeField] private UnitConfigSO unitConfigSO;


        private void Start()
        {

        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                UnitSpawner.Instance.TrySpawnUnit(unitConfigSO, new GridPosition(3, 3));
            }

        }

    }
}
