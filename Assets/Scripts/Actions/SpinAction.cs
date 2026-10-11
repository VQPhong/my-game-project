using Assets.Scripts.Grids;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Actions
{
    public class SpinAction : BaseAction
    {

        private float totalSpinAmount;


        protected override void OnActionUpdate()
        {
            float spinAddAmount = 360f * Time.deltaTime;
            transform.eulerAngles += new Vector3(0, spinAddAmount, 0);

            totalSpinAmount += spinAddAmount;
            if (totalSpinAmount >= 360f)
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
