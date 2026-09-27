using Assets.Scripts.ActionEconomies;
using Assets.Scripts.Grids;
using Assets.Scripts.Units;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Actions
{
    public abstract class BaseAction : MonoBehaviour
    {
        protected Unit unit;
        protected bool isActive;
        protected Action onActionComplete;

        [SerializeField] private List<ActionCost> actionCosts = new List<ActionCost>();


        protected virtual void Awake()
        {
            unit = GetComponent<Unit>();
        }

        public abstract string GetActionName();

        public abstract void TakeAction(GridPosition gridPosition, Action onActionComplete);

        public virtual bool IsValidActionGridPosition(GridPosition gridPosition)
        {
            List<GridPosition> validGridPositionList = GetValidActionGridPositionList();
            return validGridPositionList.Contains(gridPosition);
        }

        public abstract List<GridPosition> GetValidActionGridPositionList();

        public virtual ActionCost[] GetActionCosts()
        {
            return actionCosts.ToArray();
        }

    }
}
