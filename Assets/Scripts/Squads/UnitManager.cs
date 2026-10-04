using System;
using System.Collections.Generic;
using Assets.Scripts.Units;
using UnityEngine;

namespace Assets.Scripts.Squads
{
    public class UnitManager : MonoBehaviour
    {
        public static UnitManager Instance { get; private set; }

        private List<Unit> unitList;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("There's more than one UnitManager! " + transform + " - " + Instance);
                Destroy(gameObject);
                return;
            }
            Instance = this;

            unitList = new List<Unit>();
            Unit.OnAnyUnitSpawned += Unit_OnAnyUnitSpawned;
        }

        private void Unit_OnAnyUnitSpawned(object sender, EventArgs e)
        {
            Unit unit = sender as Unit;
            unitList.Add(unit);
        }

        public List<Unit> GetUnitList() => unitList;
    }
}
