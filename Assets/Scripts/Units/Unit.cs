using Assets.Scripts;
using Assets.Scripts.ActionEconomies;
using Assets.Scripts.Actions;
using Assets.Scripts.Grids;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Units
{
    public class Unit : MonoBehaviour
    {
        public static event EventHandler OnAnyResourceChanged;
        public static event EventHandler OnAnyUnitSpawned;


        [SerializeField] private List<ResourceTypeSO> unitResourceTypes;


        private ResourcePool resourcePool = new ResourcePool();
        private GridPosition gridPosition;
        private MoveAction moveAction;
        private SpinAction spinAction;
        private BaseAction[] baseActionArray;


        private void Awake()
        {
            moveAction = GetComponent<MoveAction>();
            spinAction = GetComponent<SpinAction>();
            baseActionArray = GetComponents<BaseAction> ();

            resourcePool.Initialize(unitResourceTypes);
        }

        private void Start()
        {
            gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
            transform.position = LevelGrid.Instance.GetWorldPosition(gridPosition);
            moveAction.ResetTargetPosition();
            LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);

            TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;

            OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);
        }

        private void Update()
        {
            GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
            if (newGridPosition != gridPosition)
            {
                // Unit changed Grid Position
                LevelGrid.Instance.UnitMovedGridPosition(this, gridPosition, newGridPosition);
                gridPosition = newGridPosition;
            }
        }


        public MoveAction GetMoveAction()
        {
            return moveAction;
        }

        public SpinAction GetSpinAction()
        {
            return spinAction;
        }

        public GridPosition GetGridPosition()
        {
            return gridPosition;
        }

        public BaseAction[] GetBaseActionArray()
        {
            return baseActionArray;
        }

        private ResourcePool GetPoolForCost(ActionCost cost)
        {
            return cost.resourceType.isShared ? TeamResourceSystem.Instance.ResourcePool : resourcePool;
        }

        public bool CanAfford(BaseAction action)
        {
            foreach (ActionCost cost in action.GetActionCosts())
            {
                if (GetPoolForCost(cost).GetAmount(cost.resourceType) < cost.amount) return false;
            }
            return true;
        }

        public bool TryTakeAction(BaseAction action)
        {
            if (!CanAfford(action)) return false;

            foreach (ActionCost cost in action.GetActionCosts())
            {
                GetPoolForCost(cost).Spend(cost.resourceType, cost.amount);
            }

            OnAnyResourceChanged?.Invoke(this, EventArgs.Empty);
            return true;
        }

        public int GetResourceAmount(ResourceTypeSO type) => resourcePool.GetAmount(type);

        public List<ResourceTypeSO> GetResourceTypes() => unitResourceTypes;


        private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
        {
            resourcePool.ApplyRegen();

            OnAnyResourceChanged?.Invoke(this, EventArgs.Empty);
        }


    }
}
