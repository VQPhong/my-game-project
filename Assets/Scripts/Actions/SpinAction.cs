using Assets.Scripts.Grids;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Actions
{
    public class SpinAction : BaseAction
    {

        private const float SpinSpeed = 360f;
        private const float TotalSpinAngle = 360f;

        private float totalSpinAmount;

        protected override bool TurnBeforeAction => false;

        protected override void OnActionUpdate()
        {
            float spinAddAmount = Mathf.Min(SpinSpeed * Time.deltaTime, TotalSpinAngle - totalSpinAmount);
            transform.eulerAngles += new Vector3(0, spinAddAmount, 0);

            totalSpinAmount += spinAddAmount;
            if (totalSpinAmount >= TotalSpinAngle)
            {
                ActionComplete();
            }
        }

        protected override void OnTakeAction(GridPosition gridPosition)
        {
            totalSpinAmount = 0f;
        }


        public override string GetActionName()
        {
            return "Spin";
        }

        public override List<GridPosition> GetValidActionGridPositionList()
        {
            GridPosition unitGridPosition = unit.GetGridPosition();

            return new List<GridPosition>
            {
                unitGridPosition
            };
        }


    }
}
