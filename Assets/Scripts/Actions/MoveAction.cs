using Assets.Scripts.Grids;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Actions
{
    public class MoveAction : BaseAction
    {
        [SerializeField] private int maxMoveDistance = 4;


        public event EventHandler OnStartMoving;
        public event EventHandler OnStopMoving;


        private Vector3 targetPosition;
        private const float MoveSpeed = 4f;

        protected override void OnActionUpdate()
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, MoveSpeed * Time.deltaTime);
            TurnTowardsPosition(targetPosition);

            if (transform.position == targetPosition)
            {
                OnStopMoving?.Invoke(this, EventArgs.Empty);
                ActionComplete();
            }
        }

        protected override void OnTakeAction(GridPosition gridPosition)
        {
            targetPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
        }

        protected override void OnTurnComplete()
        {
            OnStartMoving?.Invoke(this, EventArgs.Empty);
        }

        public override List<GridPosition> GetValidActionGridPositionList()
        {
            List<GridPosition> validGridPositionList = new List<GridPosition>();

            GridPosition unitGridPosition = unit.GetGridPosition();

            for (int x = -maxMoveDistance; x <= maxMoveDistance; x++)
            {
                for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)
                {
                    GridPosition offsetGridPosition = new GridPosition(x, z);
                    GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                    if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                    {
                        continue;
                    }

                    if (unitGridPosition == testGridPosition)
                    {
                        // Same Grid Position where the unit is already at
                        continue;
                    }

                    if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                    {
                        // Grid Position already occupied with another Unit
                        continue;
                    }

                    validGridPositionList.Add(testGridPosition);
                }
            }

            return validGridPositionList;
        }


        public override string GetActionName()
        {
            return "Move";
        }

    }
}
