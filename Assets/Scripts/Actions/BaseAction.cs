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
        protected const float DefaultTurnSpeed = 720f;
        private const float TurnCompleteAngle = 0.1f;       // Permissible error degrees
        private const float MinTurnDirectionSqr = 0.0001f;


        [SerializeField] private List<ActionCost> actionCosts = new List<ActionCost>();
        [SerializeField] private int order = 100;


        protected Unit unit;
        private bool isActive;
        private Action onActionComplete;


        protected virtual void Awake()
        {
            unit = GetComponent<Unit>();
        }

        protected void Update()
        {
            if (!isActive) return;
            OnActionUpdate();
        }


        public abstract string GetActionName();

        public void TakeAction(GridPosition gridPosition, Action onActionComplete)
        {
            ActionStart(onActionComplete);
            OnTakeAction(gridPosition);
        }

        public abstract List<GridPosition> GetValidActionGridPositionList();

        protected abstract void OnTakeAction(GridPosition gridPosition);

        protected abstract void OnActionUpdate();

        public virtual bool IsValidActionGridPosition(GridPosition gridPosition)
        {
            List<GridPosition> validGridPositionList = GetValidActionGridPositionList();
            return validGridPositionList.Contains(gridPosition);
        }

        public virtual ActionCost[] GetActionCosts()
        {
            return actionCosts.ToArray();
        }

        public void SetActionCosts(List<ActionCost> costs)
        {
            actionCosts = costs;
        }

        private void ActionStart(Action onActionComplete)
        {
            isActive = true;
            this.onActionComplete = onActionComplete;
        }

        protected void ActionComplete()
        {
            isActive = false;
            onActionComplete();
        }

        public int GetOrder() => order;

        public void SetOrder(int newOrder) => order = newOrder;

        protected virtual float GetTurnSpeed() => DefaultTurnSpeed;

        protected bool TurnTowardsPosition(Vector3 targetWorldPosition)
        {
            return TurnTowardsDirection(targetWorldPosition - unit.transform.position);
        }

        protected bool TurnTowardsDirection(Vector3 direction)
        {
            direction.y = 0f;
            if (direction.sqrMagnitude < MinTurnDirectionSqr)
            {
                return true;
            }

            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            return RotateTowardsTarget(targetRotation);
        }

        private bool RotateTowardsTarget(Quaternion targetRotation)
        {
            transform.rotation =
                Quaternion.RotateTowards(transform.rotation, targetRotation, GetTurnSpeed() * Time.deltaTime);

            if (Quaternion.Angle(transform.rotation, targetRotation) <= TurnCompleteAngle)
            {
                transform.rotation = targetRotation;
                return true;
            }
            return false;
        }
    }
}
