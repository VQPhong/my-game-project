using System;
using System.Collections.Generic;
using Assets.Scripts.Units;
using UnityEngine;

namespace Assets.Scripts.Squads
{
    /// <summary>
    /// Registry of units that can still fight.
    /// Callers ask by intent ("alive units of a side"), so a later Downed/Dead state
    /// only changes the internals here, not the callers (AI, win condition).
    /// </summary>
    public class UnitManager : MonoBehaviour
    {
        public static UnitManager Instance { get; private set; }

        private readonly List<Unit> aliveUnitList = new List<Unit>();
        private readonly List<Unit> aliveFriendlyUnitList = new List<Unit>();
        private readonly List<Unit> aliveEnemyUnitList = new List<Unit>();

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("There's more than one UnitManager! " + transform + " - " + Instance);
                Destroy(gameObject);
                return;
            }
            Instance = this;

            // Awake, not Start: units fire OnAnyUnitSpawned in their Start
            Unit.OnAnyUnitSpawned += Unit_OnAnyUnitSpawned;
            Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;
        }

        private void OnDestroy()
        {
            Unit.OnAnyUnitSpawned -= Unit_OnAnyUnitSpawned;
            Unit.OnAnyUnitDead -= Unit_OnAnyUnitDead;
        }

        private void Unit_OnAnyUnitSpawned(object sender, EventArgs e)
        {
            Unit unit = (Unit)sender;

            aliveUnitList.Add(unit);
            GetSideList(unit.IsEnemy()).Add(unit);
        }

        private void Unit_OnAnyUnitDead(object sender, EventArgs e)
        {
            Unit unit = (Unit)sender;

            aliveUnitList.Remove(unit);
            GetSideList(unit.IsEnemy()).Remove(unit);
        }

        private List<Unit> GetSideList(bool isEnemy) => isEnemy ? aliveEnemyUnitList : aliveFriendlyUnitList;

        public IReadOnlyList<Unit> GetAliveUnits() => aliveUnitList;

        public IReadOnlyList<Unit> GetAliveFriendlyUnits() => aliveFriendlyUnitList;

        public IReadOnlyList<Unit> GetAliveEnemyUnits() => aliveEnemyUnitList;

        public IReadOnlyList<Unit> GetAliveUnits(bool isEnemy) => GetSideList(isEnemy);
    }
}
