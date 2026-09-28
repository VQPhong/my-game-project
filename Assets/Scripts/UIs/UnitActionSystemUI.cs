using Assets.Scripts.ActionEconomies;
using Assets.Scripts.Actions;
using Assets.Scripts.Units;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.UIs
{
    public class UnitActionSystemUI : MonoBehaviour
    {
        [SerializeField] private Transform actionButtonPrefab;
        [SerializeField] private Transform actionButtonContainerTransform;
        [SerializeField] private TextMeshProUGUI actionPointsText;


        private List<ActionButtonUI> actionButtonUIList;


        private void Awake()
        {
            actionButtonUIList = new List<ActionButtonUI>();
        }

        void Start()
        {
            UnitActionSystem.Instance.OnSelectedUnitChanged +=
                UnitActionSystem_OnSelectedUnitChanged;

            UnitActionSystem.Instance.OnSelectedActionChanged +=
                UnitActionSystem_OnSelectedActionChanged;

            UnitActionSystem.Instance.OnActionStarted +=
                UnitActionSystem_OnActionStarted;

            TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
            Unit.OnAnyResourceChanged += Unit_OnAnyActionPointsChanged;

            CreateUnitActionButtons();
            UpdateSelectedVisual();
            UpdateActionPoints();
        }


        private void CreateUnitActionButtons()
        {
            foreach (Transform buttonTransform in actionButtonContainerTransform)
            {
                Destroy(buttonTransform.gameObject);
            }

            actionButtonUIList.Clear();

            Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();

            if (selectedUnit == null)
            {
                return;
            }

            BaseAction[] actions = selectedUnit.GetBaseActionArray();

            foreach (BaseAction baseAction in actions)
            {
                Transform actionButtonTransform = Instantiate(actionButtonPrefab, actionButtonContainerTransform);
                ActionButtonUI actionButtonUI = actionButtonTransform.GetComponent<ActionButtonUI>();
                actionButtonUI.SetBaseAction(baseAction);

                actionButtonUIList.Add(actionButtonUI);
            }
        }

        private void UpdateSelectedVisual()
        {
            foreach (ActionButtonUI actionButtonUI in actionButtonUIList)
            {
                actionButtonUI.UpdateSelectedVisual();
            }
        }

        private void UpdateActionPoints()
        {
            string text = "";

            Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();

            if (UnitActionSystem.Instance.IsUnitSelectable(selectedUnit))
            {
                foreach (ResourceTypeSO type in selectedUnit.GetResourceTypes())
                {
                    text += type.resourceName + ": " + selectedUnit.GetResourceAmount(type) + " | ";
                }
            }

            TeamResourceSystem teamResourceSystem = TeamResourceSystem.Instance;
            List<ResourceTypeSO> teamResourceTypeSO = teamResourceSystem.GetTeamResourceTypes();

            foreach (ResourceTypeSO type in teamResourceTypeSO)
            {
                text += type.resourceName + ": " + teamResourceSystem.ResourcePool.GetAmount(type) + " | ";
            }

            actionPointsText.text = text;
        }


        private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e)
        {
            CreateUnitActionButtons();
            UpdateSelectedVisual();
            UpdateActionPoints();
        }

        private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
        {
            UpdateSelectedVisual();
        }

        private void UnitActionSystem_OnActionStarted(object sender, EventArgs e)
        {
            UpdateActionPoints();
        }

        private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
        {
            UpdateActionPoints();
        }

        private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)
        {
            UpdateActionPoints();
        }


    }
}
